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
    public fqdn: pulumi.Output<string>;
    public ingressIp: pulumi.Output<string>;
    constructor(
        private name: string,
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

        // namespaces
        const amphoraNamespace = new k8s.core.v1.Namespace(`${this.name}-amphora`, {
            metadata: {
                name: "amphora",
            },
        }, opts);

        const aadPodIdentityNs = new k8s.core.v1.Namespace(`${this.name}-aad-pod-id`, {
            metadata: {
                name: "aad-pod-identity",
            },
        }, opts);

        const certManNamespace = new k8s.core.v1.Namespace(`${this.name}-certManNs`, {
            apiVersion: "v1",
            metadata: {
                labels: {
                    "cert-manager.io/disable-validation": "true",
                },
                name: "cert-manager",
            },
        }, opts);

        const ingressNginxNamespace = new k8s.core.v1.Namespace(`${this.name}-nginx`, {
            metadata: {
                labels: {
                    "app.kubernetes.io/name": "ingress-nginx",
                    "app.kubernetes.io/part-of": "ingress-nginx",
                },
                name: "ingress-nginx",
            },
        }, opts);

        // aad pod identity
        const aadPods = new k8s.yaml.ConfigFile(
            `${this.name}-aadPods`, {
            file: "components/application/aks/infrastructure-manifests/aad-pod-identity.yml",
            resourcePrefix: this.name,
        }, {
            ...opts,
            dependsOn: aadPodIdentityNs,
        });

        const indentityConfig = {
            apiVersion: "aadpodidentity.k8s.io/v1",
            kind: "AzureIdentity",
            metadata: {
                name: "web-app-identity",
                namespace: amphoraNamespace.metadata.name,
            },
            spec: {
                ClientID: this.params.identity.clientId,
                ResourceID: this.params.identity.id,
                type: 0, // 0 = MSI, 1 = SP
            },
        };

        const identity = new k8s.yaml.ConfigGroup(`${this.name}-identity`, {
            objs: [indentityConfig],
            resourcePrefix: this.name,
        }, opts);

        const bindingConfig = {
            apiVersion: "aadpodidentity.k8s.io/v1",
            kind: "AzureIdentityBinding",
            metadata: {
                name: "demo1-azure-identity-binding",
                namespace: amphoraNamespace.metadata.name,
            },
            spec: {
                AzureIdentity: indentityConfig.metadata.name,
                Selector: "amphora-front",
            },
        };

        const binding = new k8s.yaml.ConfigGroup(`${this.name}-idBinding`, {
            objs: [bindingConfig],
            resourcePrefix: this.name,
        }, opts);

        // ingress ctrler
        this.ingressController = new k8s.yaml.ConfigFile(
            `${this.name}-ingressCtlr`, {
            file: "components/application/aks/infrastructure-manifests/ingress-nginx.yml",
            resourcePrefix: this.name,
        }, opts);

        // cert manager crds
        const certManagerCrds = new k8s.yaml.ConfigFile(
            `${this.name}-certManCrds`, {
            file: "components/application/aks/infrastructure-manifests/cert-manager-crds.yml",
            resourcePrefix: this.name,
        }, opts);

        // let's encrypt
        const certManagerChart = new k8s.helm.v2.Chart(
            `${this.name}-cert-man`,
            {
                chart: "cert-manager",
                fetchOpts: {
                    repo: "https://charts.jetstack.io",
                },
                namespace: certManNamespace.metadata.name,
                values: {},
                version: "v0.13.1",
            }, opts);

        // make sure we prod in prod
        const transformations = [];
        if (pulumi.getStack() === "prod") {
            transformations.push((obj: any) => {
                // transform the issuer into the prod one.
                if (obj.spec && obj.spec.acme) {
                    obj.spec.acme.server = "https://acme-v02.api.letsencrypt.org/directory";
                }
            });
        }

        // certificate issuer
        const caClusterIssuer = new k8s.yaml.ConfigFile(`${this.name}-caClusterIssuer`, {
            file: `components/application/aks/infrastructure-manifests/cert-issuer.yml`,
            resourcePrefix: this.name,
            transformations,
        }, {
            ...opts,
            dependsOn: [certManagerCrds, certManagerChart],
        });

        // amphora frontend config map
        const webAppConfigMap = new k8s.core.v1.ConfigMap(`${this.name}-front-config`, {
            data: this.params.appSettings,
            metadata: {
                name: "amphora-frontend-config",
                namespace: amphoraNamespace.metadata.name,
            },
        }, opts);

        // aksAU1-infra-ingress-nginx/ingress-nginx == with redourcePrefix
        this.ingressIp = this.ingressController
            .getResource("v1/Service", `${this.name}-ingress-nginx`, `ingress-nginx`)
            .apply((service) => service.status.loadBalancer.ingress[0].ip);

        this.fqdnName = pulumi.interpolate`${pulumi.getStack()}-amphoradata`;
        this.fqdn = pulumi.interpolate`${this.fqdnName}.${this.params.location}.cloudapp.azure.com`;
    }
}
