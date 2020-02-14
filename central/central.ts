import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const authConfig = new pulumi.Config("authentication");

const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
const prodHostnames = prodStack.getOutput("appHostnames"); // should be an array
const prodBackendCount = 2; // should match the array length from prod stack as above

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

const appCName = new azure.dns.CNameRecord("appCName",
    {
        name: "app",
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

const backendPools: pulumi.Input<Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>> = [];
// add the squarespace backend
backendPools.push(
    {
        backends: [{
            address: "azalea-orca-x567.squarespace.com",
            hostHeader: "azalea-orca-x567.squarespace.com",
            httpPort: 80,
            httpsPort: 443,
        }],
        healthProbeName: "normal",
        loadBalancingName: "loadBalancingSettings1",
        name: "squarespaceBackend",
    });

const prodBackends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
for (let i = 0; i < prodBackendCount; i++) {
    prodBackends.push({
        address: prodHostnames.apply((h) => h[i]),
        hostHeader: "app.amphoradata.com",
        httpPort: 80,
        httpsPort: 443,
    });
}

const prodBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
    backends: prodBackends,
    healthProbeName: "normal",
    loadBalancingName: "loadBalancingSettings1",
    name: "prodBackend",
};

backendPools.push(prodBackendPool);

const frontDoor = new azure.frontdoor.Frontdoor("fd", {
    backendPoolHealthProbes: [{
        intervalInSeconds: 30,
        name: "normal",
        path: "/",
        protocol: "Https",
    }],
    backendPoolLoadBalancings: [{
        name: "loadBalancingSettings1",
        sampleSize: 4,
        successfulSamplesRequired: 2,
    }],
    backendPools,
    enforceBackendPoolsCertificateNameCheck: true,
    frontendEndpoints: [{
        customHttpsProvisioningEnabled: false,
        hostName: "amphora.azurefd.net",
        name: "defaultFrontend",
        sessionAffinityEnabled: true,
    }, {
        customHttpsConfiguration: {
            azureKeyVaultCertificateSecretName: "static-site",
            azureKeyVaultCertificateSecretVersion: "2cb1416e06e640229d2e28bacb1eb9cd",
            azureKeyVaultCertificateVaultId: kv.id,
            certificateSource: "AzureKeyVault",
        },
        customHttpsProvisioningEnabled: true,
        hostName: "amphoradata.com",
        name: "rootDomain",
        sessionAffinityEnabled: true,
    }, {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: "www.amphoradata.com",
        name: "wwwDomain",
        sessionAffinityEnabled: true,
    }, {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: "beta.amphoradata.com",
        name: "betaDomain",
        sessionAffinityEnabled: true,
    }, {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: "app.amphoradata.com",
        name: "appDomain",
        sessionAffinityEnabled: true,
    }],
    location: "global",
    name: "amphora",
    resourceGroupName: rg.name,
    routingRules: [
        {
            // Squarespace Backend Route
            acceptedProtocols: [
                "Https",
            ],
            forwardingConfiguration: {
                backendPoolName: "squarespaceBackend",
                cacheQueryParameterStripDirective: "StripNone",
                forwardingProtocol: "HttpsOnly",
            },
            frontendEndpoints: ["defaultFrontend", "rootDomain", "wwwDomain"],
            name: "routeToSquarespace",
            patternsToMatches: ["/*"],
        },
        {
            // Prod Backend Route
            acceptedProtocols: [
                "Https",
            ],
            forwardingConfiguration: {
                backendPoolName: "prodBackend",
                cacheEnabled: false,
                cacheQueryParameterStripDirective: "StripNone",
                forwardingProtocol: "HttpsOnly",
            },
            frontendEndpoints: ["betaDomain", "appDomain"],
            name: "routeToProdEnvironment",
            patternsToMatches: ["/*"],
        },
        {
            // Https Redirects Route
            acceptedProtocols: [
                "Http",
            ],
            frontendEndpoints: ["defaultFrontend", "rootDomain", "wwwDomain", "betaDomain", "appDomain"],
            name: "redirectToHttps",
            patternsToMatches: ["/*"],
            redirectConfiguration: {
                redirectProtocol: "HttpsOnly",
                redirectType: "Found",
            },
        },
    ],
});
