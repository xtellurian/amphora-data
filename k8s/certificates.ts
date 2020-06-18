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

        // let's encrypt
        const certManagerVersion = "v0.15.0";

        const certManager = new k8s.yaml.ConfigFile("cert-manager", {
            file: `https://github.com/jetstack/cert-manager/releases/download/${certManagerVersion}/cert-manager.yaml`,
        }, {
            ...opts
        })

        // you can disable this for dev if you're debugging, but hsts redirects will fail
        const transformations = [];
        transformations.push((obj: any) => {
            // transform the issuer into the prod one.
            if (obj.spec && obj.spec.acme) {
                obj.spec.acme.server = "https://acme-v02.api.letsencrypt.org/directory";
            }
        });

        const webhookDeployment = certManager.getResource("apps/v1/Deployment", "cert-manager-webhook");
        // certificate issuer
        const caClusterIssuer = new k8s.yaml.ConfigFile("caClusterIssuer", {
            file: `manifests/cert-issuer.yml`,
            transformations,
        }, {
            ...opts,
            dependsOn: [ certManager, webhookDeployment],
        });
    }
}