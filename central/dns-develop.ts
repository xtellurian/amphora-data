import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const tags = {
    component: "central-dns-develop",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

export interface IDnsDevelopParams {
    rg: azure.core.ResourceGroup;
    zone: azure.dns.Zone;
}

export class DnsDevelop extends pulumi.ComponentResource {

    constructor(
        private name: string,
        private params: IDnsDevelopParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora-k8s:FrontEnd", name, {}, opts);
        this.create(params.rg, params.zone);
    }

    private create(rg: azure.core.ResourceGroup, zone: azure.dns.Zone) {
        // develop dns things
        const developK8sFqdn = "develop-amphoradata.australiasoutheast.cloudapp.azure.com";
        const devAppCName = new azure.dns.CNameRecord("devAppCName",
            {
                name: "develop.app",
                record: developK8sFqdn,
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            },
        );

        const devIdentityCName = new azure.dns.CNameRecord("devIdCName",
            {
                name: "develop.identity",
                record: developK8sFqdn,
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
