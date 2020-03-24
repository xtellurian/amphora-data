import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { createDns } from "./dns/dns";
import { createFrontDoor } from "./front-door/frontDoor";

const authConfig = new pulumi.Config("authentication");

const tags = {
    component: "central",
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

const frontendFqdns = createDns(rg);

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

createFrontDoor(rg, kv, frontendFqdns);
