import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import * as kx from "@pulumi/kubernetesx";

export interface IIdentityArgs {
    fqdn: pulumi.Output<string>,
    location: pulumi.Output<string>,
    provider: k8s.Provider,
}

const env = pulumi.getStack();

const identityConfig = new pulumi.Config("identity")
const image = identityConfig.require("image");

export class Identity extends pulumi.ComponentResource {

    constructor(
        private name: string,
        private params: IIdentityArgs,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora-k8s:Identity", name, {}, opts);
        this.create();
    }

    private create() {

        const opts = {
            parent: this,
            provider: this.params.provider,
        };

        const name = "amphora-identity";

        const pb = new kx.PodBuilder({
            nodeSelector: {
                "beta.kubernetes.io/os": "linux"
            },
            containers: [{
                // name is not required. If not provided, it is inferred from the image.
                name,
                image,
                ports: { http: 80, }, // Simplified ports syntax.
                resources: {
                    requests: {
                        cpu: "100m",
                        memory: "128Mi"
                    },
                    limits: {
                        cpu: "250m",
                        memory: "256Mi"
                    }
                },
                envFrom: [{
                    configMapRef: {
                        name: "amphora-identity-config"
                    }
                }],
                livenessProbe: {
                    httpGet: {
                        path: "/healthz",
                        port: 80,
                    },
                    initialDelaySeconds: 3,
                    periodSeconds: 10
                },
                readinessProbe: {
                    httpGet: {
                        path: "/healthz",
                        port: 80,
                    },
                    initialDelaySeconds: 3,
                    periodSeconds: 3,
                }
            }],
        });

        const deployment = new kx.Deployment(`${this.name}-deploy`, {
            apiVersion: "apps/v1",
            metadata: {
                name,
                namespace: "amphora",
            },
            spec: {
                replicas: 1,
                selector: {
                    matchLabels: {
                        app: name
                    }
                },
                template: {
                    metadata: {
                        labels: {
                            app: name,
                            aadpodidbinding: "amphora-identity",
                        }
                    },
                    spec: pb.podSpec,
                },

            },
        }, opts);

        const service = new kx.Service(`${this.name}-svc`, {
            kind: "Service",
            metadata: {
                name,
                namespace: "amphora"
            },
            spec: {
                ports: [
                    {
                        port: 80
                    }
                ],
                selector: {
                    app: name
                }
            },
        }, opts);

        // http config for each host
        const http = {
            paths: [
                {
                    backend: {
                        serviceName: name,
                        servicePort: 80
                    },
                    path: "/(.*)"
                }
            ]
        }

        const hosts: pulumi.Output<string>[] = [
            pulumi.interpolate `${env}.${this.params.location}.identity.amphoradata.com`,
            pulumi.interpolate `${env}.identity.amphoradata.com` // the front foor
        ]
        // add the rule for each host
        const rules: pulumi.Input<k8s.types.input.extensions.v1beta1.IngressRule>[] = [];

        if (hosts) {
            hosts.forEach(h => {
                rules.push({
                    host: h,
                    http
                });
            });
        }

        if (rules && rules.length > 0) {
            const ingress = new k8s.extensions.v1beta1.Ingress(`${this.name}-ingress`, {
                kind: "Ingress",
                metadata: {
                    name: "amphora-identity-ingress",
                    namespace: "amphora",
                    annotations: {
                        "kubernetes.io/ingress.class": "nginx",
                        "cert-manager.io/cluster-issuer": "letsencrypt",
                        "nginx.ingress.kubernetes.io/rewrite-target": " /$1",
                    }
                },
                spec: {
                    tls: [
                        {
                            hosts: [...hosts, this.params.fqdn],
                            secretName: "identity-tls-secret"
                        }
                    ],
                    rules
                }
            }, {...opts, deleteBeforeReplace: true});
        } else {
            throw new Error("No Igress Rules for Identity")
        }
    }
}
