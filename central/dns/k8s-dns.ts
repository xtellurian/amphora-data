import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const tags = {
    component: "central-dns-develop",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

export interface IK8sDnsParams {
    rg: azure.core.ResourceGroup;
    zone: azure.dns.Zone;
    environment: string;
}

const mel = "australiasoutheast";
const syd = "australiaeast";

export class K8sDns extends pulumi.ComponentResource {

    constructor(
        private name: string,
        private params: IK8sDnsParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:K8sDns", name, {}, opts);
        this.create(params.rg, params.zone, params.environment);
    }

    private create(rg: azure.core.ResourceGroup, zone: azure.dns.Zone, env: string) {
        // k8s dns things
        this.createForLocation(rg, zone, env, syd);
        this.createForLocation(rg, zone, env, mel);
    }

    private createForLocation(rg: azure.core.ResourceGroup, zone: azure.dns.Zone, env: string, loc: string) {
        const fqdn = `${env}-amphoradata.${loc}.cloudapp.azure.com`;
        const melAppCName = new azure.dns.CNameRecord(`${env}-${loc}-AppCN`,
            {
                name: `${env}.${loc}.app`,
                record: fqdn,
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            },
        );

        const melIdentityCName = new azure.dns.CNameRecord(`${env}-${loc}-IdCN`,
            {
                name: `${env}.${loc}.identity`,
                record: fqdn,
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            },
        );
    }
}
