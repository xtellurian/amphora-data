import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { frontDoorArm } from "./front-door-arm";

const authConfig = new pulumi.Config("authentication");

const tags = {
    component: "constant",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

const rg = new azure.core.ResourceGroup("constant",
    {
        location: "AustraliaSoutheast",
        name: "amphora-central",
        tags,
    });

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
    });

const dnsZone = new azure.dns.Zone("centralDnsZone",
    {
        name: "amphoradata.com",
        resourceGroupName: rg.name,
        tags,
    });

const template = new azure.core.TemplateDeployment("frontDoorArm",
    {
        deploymentMode: "Incremental",
        parameters: {
            frontDoorName: "amphora",
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(frontDoorArm()),
    },
    {
        dependsOn: dnsZone,
    });

const wwwCName = new azure.dns.CNameRecord("wwwCName",
    {
        name: "www",
        record: "amphora.azurefd.net",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    });

// these come from O365

const sipDirCName = new azure.dns.CNameRecord("sipDir",
    {
        name: "sip",
        record: "sipdir.online.lync.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    });
const linqDiscoverCName = new azure.dns.CNameRecord("linqDiscover",
    {
        name: "lyncdiscover",
        record: "webdir.online.lync.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 30,
        zoneName: dnsZone.name,
    });

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
    });

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
    });

const autodoscoverCName = new azure.dns.CNameRecord("autodiscoverCName",
    {
        name: "autodiscover",
        record: "autodiscover.outlook.com",
        resourceGroupName: rg.name,
        tags,
        ttl: 3600,
        zoneName: dnsZone.name,
    });

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
    });
