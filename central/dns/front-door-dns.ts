import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const record = "amphora.azurefd.net";

const opts = {
    protect: true,
};

const tags = {
    component: "central-dns",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

export function frontDoorDns(rg: azure.core.ResourceGroup, zone: azure.dns.Zone) {
    // static site
    const wwwCName = new azure.dns.CNameRecord("wwwCName",
        {
            name: "www",
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );
    // backwards compat
    const betaCName = new azure.dns.CNameRecord("betaCName",
        {
            name: "beta",
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );

    // PROD
    const appCName = new azure.dns.CNameRecord("appCName",
        {
            name: "app",
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );

    const identityCName = new azure.dns.CNameRecord("identityCName",
        {
            name: "identity",
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );

    addForEnvironment("develop", rg, zone);
    addForEnvironment("master", rg, zone);
}

function addForEnvironment(env: string, rg: azure.core.ResourceGroup, zone: azure.dns.Zone) {
    const appCName = new azure.dns.CNameRecord(`${env}-app-CN`,
        {
            name: `${env}.app`,
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );

    const identityCName = new azure.dns.CNameRecord(`${env}-id-CN`,
        {
            name: `${env}.identity`,
            record,
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: zone.name,
        },
        opts,
    );
}
