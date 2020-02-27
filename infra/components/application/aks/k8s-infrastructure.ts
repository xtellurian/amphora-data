import * as azure from "@pulumi/azure";
import * as k8s from "@pulumi/kubernetes";
import * as pulumi from "@pulumi/pulumi";

export interface IK8sInfrastructureParams {
    provider: k8s.Provider;
    location: pulumi.Output<string>;
    identity: azure.authorization.UserAssignedIdentity;
    appSettings: pulumi.Input<{
        [key: string]: pulumi.Input<string>;
    }>;
}

export class K8sInfrastructure extends pulumi.ComponentResource {
    public ingressController: k8s.yaml.ConfigFile;
    public fqdnName: pulumi.Output<string>;
    constructor(
        name: string,
        private params: IK8sInfrastructureParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:K8sInfrastructure", name, {}, opts);
        this.create();
    }

    private create() {

        const opts = {
            parent: this,
            provider: this.params.provider,
        };

        const aadPods = new k8s.yaml.ConfigFile(
            "aadPods", {
            file: "components/application/aks/infrastructure-manifests/aad-pod-identity.yml",
        }, opts);

        const indentityConfig = {
            apiVersion: "aadpodidentity.k8s.io/v1",
            kind: "AzureIdentity",
            metadata: {
                name: "web-app-identity",
            },
            spec: {
                ClientID: this.params.identity.clientId,
                ResourceID: this.params.identity.id,
                type: 0, // 0 = MSI, 1 = SP
            },
        };

        const identity = new k8s.yaml.ConfigGroup("identity", {
            objs: [indentityConfig],
        }, opts);

        const bindingConfig = {
            apiVersion: "aadpodidentity.k8s.io/v1",
            kind: "AzureIdentityBinding",
            metadata: {
                name: "demo1-azure-identity-binding",
            },
            spec: {
                AzureIdentity: indentityConfig.metadata.name,
                Selector: "hello-world",
            },
        };

        const binding = new k8s.yaml.ConfigGroup("binding", {
            objs: [bindingConfig],
        }, opts);

        const webAppConfigMap = new k8s.core.v1.ConfigMap("webapp-config", {
            data: this.params.appSettings,
        }, opts);

        this.ingressController = new k8s.yaml.ConfigFile(
            "ingressCtrl", {
            file: "components/application/aks/infrastructure-manifests/ingress-nginx.yml",
        }, opts);

        // cert manager crds
        const certManagerCrds = new k8s.yaml.ConfigFile(
            "certManCrds", {
            file: "components/application/aks/infrastructure-manifests/cert-manager-crds.yml",
        }, opts);

        const certManNamespace = new k8s.core.v1.Namespace("certManNs", {
            apiVersion: "v1",
            metadata: {
                labels: {
                    "cert-manager.io/disable-validation": "true",
                },
                name: "cert-manager",
            },
        }, opts);

        // helmy thin
        const certManagerChart = new k8s.helm.v2.Chart(
            "cert-manager",
            {
                chart: "cert-manager",
                fetchOpts: {
                    repo: "https://charts.jetstack.io",
                },
                namespace: "cert-manager",
                values: {},
                version: "v0.13.1",
            }, opts);

        const transformations = [];
        if (pulumi.getStack() === "prod") {
            transformations.push((obj: any) => {
                // transform the issuer into the prod one.
                if (obj.spec && obj.spec.acme) {
                    obj.spec.acme.server = "https://acme-v02.api.letsencrypt.org/directory";
                }
            });
        }

        const caClusterIssuer = new k8s.yaml.ConfigFile("caClusterIssuer", {
            file: `components/application/aks/infrastructure-manifests/cert-issuer.yml`,
            transformations,
        }, {
            ...opts,
            dependsOn: certManagerCrds,
        });

        this.fqdnName = pulumi.interpolate`${pulumi.getStack()}-amphoradata`;

        // TODO: Remove
        // const amphoraFrontend = new k8s.yaml.ConfigFile(
        //     "amphoraFrontend", {
        //     file: "components/application/aks/infrastructure-manifests/test.yml",
        //     transformations: [
        //         (obj: any) => {
        //             if (obj.metadata.name === "hello-world-ingress") {
        //                 // console.log(obj);
        //                 for (const tls of obj.spec.tls) {
        //                     for (let j = 0; j < tls.hosts.length; j++) {
        // tslint:disable-next-line: max-line-length
        //                         tls.hosts[j] = pulumi.interpolate`${this.fqdnName}.${this.params.location}.cloudapp.azure.com`;
        //                     }
        //                 }

        //                 for (const rule of  obj.spec.rules) {
        // tslint:disable-next-line: max-line-length
        //                     rule.host = pulumi.interpolate`${this.fqdnName}.${this.params.location}.cloudapp.azure.com`;
        //                 }
        //             }
        //             // console.log(obj.spec.tls)
        //             // console.log(obj.spec.rules)
        //         },
        //     ],
        // }, {
        //     ...opts,
        //     dependsOn: caClusterIssuer,
        // });
    }
}