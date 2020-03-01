import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import * as kx from "@pulumi/kubernetesx";

export interface IFrontendArgs {
    fqdn: pulumi.Output<string>,
    provider: k8s.Provider,
}

const config = new pulumi.Config();
const image = config.require("image");
export class FrontEnd extends pulumi.ComponentResource {

    constructor(
        name: string,
        private params: IFrontendArgs,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora-k8s:FrontEnd", name, {}, opts);
        this.create();
    }

    private create() {

        const opts = {
            parent: this,
            provider: this.params.provider,
        };

        const name = "amphora-front";

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
                        name: "amphora-frontend-config"
                    }
                }],
            }],
        });

        const deployment = new kx.Deployment("frontend-deploy", {
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
                            aadpodidbinding: "amphora-front",
                        }
                    },
                    spec: pb.podSpec,
                },

            },
        }, opts);

        const service = new kx.Service("frontend-svc", {
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

        const ingress = new k8s.extensions.v1beta1.Ingress("amphora-front-ingress", {
            kind: "Ingress",
            metadata: {
                name: "amphora-front-ingress",
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
                        hosts: [this.params.fqdn],
                        secretName: "tls-secret"
                    }
                ],
                rules: [
                    {
                        host: this.params.fqdn,
                        http: {
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
                    }
                ]
            }
        }, opts);
    }
}
