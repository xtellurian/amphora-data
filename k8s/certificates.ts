import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";

interface ICertificateArgs {
    provider: k8s.Provider,
}

export class Certificates extends pulumi.ComponentResource {

    constructor(
        private name: string,
        private params: ICertificateArgs,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora-k8s:Certificates", name, {}, opts);
        this.create();
    }

    private create() {
        const opts = {
            parent: this,
            provider: this.params.provider,
        }

        const certManNamespace = new k8s.core.v1.Namespace(`ns`, {
            apiVersion: "v1",
            metadata: {
                labels: {
                    "cert-manager.io/disable-validation": "true",
                },
                name: "cert-manager",
            },
        }, opts);

        // cert manager crds
        const certManagerVersion = "v0.14.1";
        const certManagerCrds = new k8s.yaml.ConfigFile("cert-man-crds", {
            file: `https://github.com/jetstack/cert-manager/releases/download/${certManagerVersion}/cert-manager.crds.yaml`,
        }, opts);

        // const certManagerCrds = new k8s.yaml.ConfigFile(
        //     `${this.name}-certManCrds`, {
        //     file: "components/application/aks/infrastructure-manifests/cert-manager-crds.yml",
        //     resourcePrefix: this.name,
        // }, opts);

        // let's encrypt
        const releaseName = "cert-man";
        const chartName = "cert-manager";
        const certManagerChart = new k8s.helm.v2.Chart(releaseName,
            {
                chart: chartName,
                fetchOpts: {
                    repo: "https://charts.jetstack.io",
                },
                namespace: certManNamespace.metadata.name,
                values: {},
                version: certManagerVersion,
            },
            {
                ...opts,
                dependsOn: certManagerCrds,
            });

        // you can disable this for dev if you're debugging, but hsts redirects will fail
        const transformations = [];
        transformations.push((obj: any) => {
            // transform the issuer into the prod one.
            if (obj.spec && obj.spec.acme) {
                obj.spec.acme.server = "https://acme-v02.api.letsencrypt.org/directory";
            }
        });

        const webhookDeployment = certManagerChart.getResource("apps/v1/Deployment", `${releaseName}-${chartName}-webhook`);
        // certificate issuer
        const caClusterIssuer = new k8s.yaml.ConfigFile("caClusterIssuer", {
            file: `manifests/cert-issuer.yml`,
            transformations,
        }, {
            ...opts,
            dependsOn: [certManagerCrds, certManagerChart, webhookDeployment],
        });
    }
}