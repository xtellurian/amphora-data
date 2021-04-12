import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const record = "amphora.azurefd.net";

const opts = {
    protect: false,
};

const ttl = 3600;

const tags = {
    component: "central-dns",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};
export interface IUniqueUrl {
    globalHost: pulumi.Output<string>;
    frontendName: string; // the name of the frontend pool in azure fd
    appName: string; // must be identity or app
}

export interface IGlobalUrl {
    // app: IUniqueUrl;
    api: IUniqueUrl;
    // identity: IUniqueUrl;
}

export interface IFrontendHosts {
    // develop: IGlobalUrl;
    // master: IGlobalUrl;
    prod: IGlobalUrl;
}

export function frontDoorDns(
    rg: azure.core.ResourceGroup,
    zone: azure.dns.Zone
): IFrontendHosts {
    const apiCName = new azure.dns.CNameRecord(
        "apiCName",
        {
            name: "api",
            record,
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: zone.name,
        },
        opts
    );

    // const identityCName = new azure.dns.CNameRecord(
    //     "identityCName",
    //     {
    //         name: "identity",
    //         record,
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: zone.name,
    //     },
    //     opts
    // );

    // const docsCName = new azure.dns.CNameRecord(
    //     "docsCName",
    //     {
    //         name: "docs",
    //         record,
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: zone.name,
    //     },
    //     opts
    // );

    // const docsCName = new azure.dns.CNameRecord(
    //     "docsCName",
    //     {
    //         name: "docs",
    //         record,
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: zone.name,
    //     },
    //     opts
    // );

    // const develop = addForEnvironment("develop", rg, zone);
    // const master = addForEnvironment("master", rg, zone);

    return {
        // develop,
        // master,
        prod: {
            api: {
                appName: "api",
                frontendName: "apiDomain",
                globalHost: apiCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period,
            },
            // app: {
            //     appName: "app",
            //     frontendName: "appDomain",
            //     globalHost: appCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period,
            // },
            // identity: {
            //     appName: "identity",
            //     frontendName: "identityFrontend",
            //     globalHost: identityCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period,
            // },
        },
    };
}

function addForEnvironment(
    env: string,
    rg: azure.core.ResourceGroup,
    zone: azure.dns.Zone
): IGlobalUrl {
    // const appCName = new azure.dns.CNameRecord(
    //     `${env}-app-CN`,
    //     {
    //         name: `${env}.app`,
    //         record,
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: zone.name,
    //     },
    //     opts
    // );

    const apiCName = new azure.dns.CNameRecord(
        `${env}-api-CN`,
        {
            name: `${env}.api`,
            record,
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: zone.name,
        },
        opts
    );

    // const identityCName = new azure.dns.CNameRecord(
    //     `${env}-id-CN`,
    //     {
    //         name: `${env}.identity`,
    //         record,
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: zone.name,
    //     },
    //     opts
    // );

    return {
        api: {
            appName: "api",
            frontendName: `${env}apifront`,
            globalHost: apiCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period
        },
        // app: {
        //     appName: "app",
        //     frontendName: `${env}appfront`,
        //     globalHost: appCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period
        // },
        // identity: {
        //     appName: "identity",
        //     frontendName: `${env}idfront`,
        //     globalHost: identityCName.fqdn.apply((_) => _.slice(0, -1)), // remove a trailing period
        // },
    };
}
