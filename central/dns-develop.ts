import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { throws } from "assert";

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
        super("amphora:DevDns", name, {}, opts);
        this.create(params.rg, params.zone);
    }

    private create(rg: azure.core.ResourceGroup, zone: azure.dns.Zone) {
        // develop dns things
        this.createForEnvironment("develop", rg, zone);
        this.createForEnvironment("master", rg, zone);
    }

    private createForEnvironment(env: string, rg: azure.core.ResourceGroup, zone: azure.dns.Zone) {
        if (env === "master" || env === "develop") {
            // we good
            const developK8sFqdn = `${env}-amphoradata.australiasoutheast.cloudapp.azure.com`;
            const devAppCName = new azure.dns.CNameRecord(`${env}AppCName`,
                {
                    name: `${env}.app`,
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

            const devIdentityCName = new azure.dns.CNameRecord(`${env}IdCName`,
                {
                    name: `${env}.identity`,
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
        } else {
            throw new Error("Not master or develop");
        }
    }
}
