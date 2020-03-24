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

interface IEnvironmentFqdns {
    app: pulumi.Output<string>;
    identity: pulumi.Output<string>;
}

export interface IFrontendFqdns {
    develop: IEnvironmentFqdns;
    master: IEnvironmentFqdns;
    prod: IEnvironmentFqdns;
}

export function frontDoorDns(rg: azure.core.ResourceGroup, zone: azure.dns.Zone): IFrontendFqdns {
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

    const develop = addForEnvironment("develop", rg, zone);
    const master = addForEnvironment("master", rg, zone);

    return {
        develop,
        master,
        prod: {
            app: appCName.fqdn,
            identity: identityCName.fqdn,
        },
    };
}

function addForEnvironment(env: string, rg: azure.core.ResourceGroup, zone: azure.dns.Zone): IEnvironmentFqdns {
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

    return {
        app: appCName.fqdn,
        identity: identityCName.fqdn,
    };
}
