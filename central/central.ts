import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { frontDoorArm } from "./front-door-arm";

const authConfig = new pulumi.Config("authentication");

// const demoStack = new pulumi.StackReference(`xtellurian/amphora/demo`);
const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
// const demoHostname = demoStack.getOutput("appHostname");
const prodHostname = prodStack.getOutput("appHostname");

const tags = {
    component: "constant",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

const opts = {
    protect: true,
};

const rg = new azure.core.ResourceGroup("constant",
    {
        location: "AustraliaSoutheast",
        name: "amphora-central",
        tags,
    },
    opts,
);

const kv = new azure.keyvault.KeyVault("central-keyVault",
    {
        accessPolicies: [
            {
                certificatePermissions: ["create", "list", "get", "delete", "listissuers", "import", "manageissuers", "managecontacts"],
                objectId: authConfig.require("rian"),
                secretPermissions: ["list", "set", "get", "delete"],
                tenantId: authConfig.require("tenantId"),
            },
            {
                applicationId: authConfig.require("keyVaultAppId"),
                certificatePermissions: ["list", "get"],
                objectId: authConfig.require("keyVaultObjectId"),
                secretPermissions: ["get"],
                tenantId: authConfig.require("tenantId"),
            },
            {
                certificatePermissions: ["list", "get"],
                objectId: authConfig.require("keyVaultObjectId"),
                secretPermissions: ["get"],
                tenantId: authConfig.require("tenantId"),
            },
            {
                applicationId: authConfig.require("spAppId"),
                certificatePermissions: ["create", "list", "get"],
                objectId: authConfig.require("spObjectId"),
                secretPermissions: ["list", "set", "get", "delete"],
                tenantId: authConfig.require("tenantId"),
            },
            {
                certificatePermissions: ["create", "list", "get"],
                objectId: authConfig.require("spObjectId"),
                secretPermissions: ["list", "set", "get", "delete"],
                tenantId: authConfig.require("tenantId"),
            },
        ],
        name: "amphora-central",
        resourceGroupName: rg.name,
        skuName: "standard",
        tags,
        tenantId: authConfig.require("tenantId"),
    },
    opts,
);

const dnsZone = new azure.dns.Zone("centralDnsZone",
    {
        name: "amphoradata.com",
        resourceGroupName: rg.name,
        tags,
    },
    opts,
);

const template = new azure.core.TemplateDeployment("frontDoorArm",
    {
        deploymentMode: "Incremental",
        parameters: {
            frontDoorName: "amphora",
            prodHostname,
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(frontDoorArm(tags)),
    },
    {
        dependsOn: dnsZone,
        protect: true,
    });

const wwwCName = new azure.dns.CNameRecord("wwwCName",
    {
        name: "www",
        record: "amphora.azurefd.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    },
    opts,
);

const betaCName = new azure.dns.CNameRecord("betaCName",
    {
        name: "beta",
        record: "amphora.azurefd.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    },
    opts,
);

// these come from O365

const sipDirCName = new azure.dns.CNameRecord("sipDir",
    {
        name: "sip",
        record: "sipdir.online.lync.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    },
    opts,
);
const linqDiscoverCName = new azure.dns.CNameRecord("linqDiscover",
    {
        name: "lyncdiscover",
        record: "webdir.online.lync.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    },
    opts,
);

const githubChallengeTxt = new azure.dns.TxtRecord("githubChallenge",
    {
        name: "_github-challenge-amphoradata",
        records: [
            {
                value: "44d6785da4",
            },
        ],
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    },
    opts,
);

// directly from o365
const mxRecord = new azure.dns.MxRecord("outlookMx",
    {
        name: "@",
        records: [
            {
                exchange: "amphoradata-com.mail.protection.outlook.com",
                preference: "0",
            },
        ],
        resourceGroupName: rg.name,
        tags,
        ttl: 3600,
        zoneName: dnsZone.name,
    },
    opts,
);

const autodoscoverCName = new azure.dns.CNameRecord("autodiscoverCName",
    {
        name: "autodiscover",
        record: "autodiscover.outlook.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 3600,
        zoneName: dnsZone.name,
    },
    opts,
);

const txtRecord = new azure.dns.TxtRecord("outlookTxtRecord",
    {
        name: "@",
        records: [
            {
                value: "v=spf1 include:spf.protection.outlook.com -all", // from 365
            },
            {
                value: "a9eb31102744451ca0ad66b3cc7bde06", // from digicert
            },
        ],
        resourceGroupName: rg.name,
        tags,
        ttl: 3600,
        zoneName: dnsZone.name,
    },
    opts,
);

// squarespace
const verifyCName = new azure.dns.CNameRecord("verifySquarespace",
    {
        name: "4cexeacc9y5xc84bxj78",
        record: "verify.squarespace.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 60,
        zoneName: dnsZone.name,
    },
    opts,
);

// Send Grid

const sendGridCName1 = new azure.dns.CNameRecord("sendGridCName1",
    {
        name: "em2252",
        record: "u12987374.wl125.sendgrid.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 60,
        zoneName: dnsZone.name,
    },
    opts,
);
const sendGridCName2 = new azure.dns.CNameRecord("sendGridCName2",
    {
        name: "s1._domainkey",
        record: "s1.domainkey.u12987374.wl125.sendgrid.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 60,
        zoneName: dnsZone.name,
    },
    opts,
);
const sendGridCName3 = new azure.dns.CNameRecord("sendGridCName3",
    {
        name: "s2._domainkey",
        record: "s2.domainkey.u12987374.wl125.sendgrid.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 60,
        zoneName: dnsZone.name,
    },
    opts,
);
