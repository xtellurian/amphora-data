import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { IMultiEnvironmentMultiCluster } from "./contracts";
import { createDns } from "./dns/dns";
import { createFrontDoor } from "./front-door/frontDoor";

const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
const masterStack = new pulumi.StackReference(`xtellurian/amphora/master`);
const developStack = new pulumi.StackReference(`xtellurian/amphora/develop`);

const prodAppHostnames = prodStack.getOutput("appHostnames"); // should be an array
const prodIdentityHostnames = prodStack.getOutput("identityHostnames"); // should be an array

const masterAppHostnames = masterStack.getOutput("appHostnames"); // should be an array
const masterIdentityHostnames = masterStack.getOutput("identityHostnames"); // should be an array

const developAppHostnames = developStack.getOutput("appHostnames"); // should be an array
const developIdentityHostnames = developStack.getOutput("identityHostnames"); // should be an array

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

const rg = new azure.core.ResourceGroup(
    "constant",
    {
        location: "AustraliaSoutheast",
        name: "amphora-central",
        tags,
    },
    opts
);
const outputName = "k8s";

// TODO: renable secondary
const clusters: IMultiEnvironmentMultiCluster = {
    develop: {
        // australiaeast: {
        //     ingressIp: developStack.getOutput(outputName)
        //         .apply((k) => k?.australiaeast.ingressIp) as pulumi.Output<string>,
        //     location: developStack.getOutput(outputName)
        //         .apply((k) => k?.australiaeast.location) as pulumi.Output<string>,
        // },
        australiasoutheast: {
            ingressIp: developStack
                .getOutput(outputName)
                .apply((k) => k?.australiasoutheast.ingressIp) as pulumi.Output<
                string
            >,
            location: developStack
                .getOutput(outputName)
                .apply((k) => k?.australiasoutheast.location) as pulumi.Output<
                string
            >,
        },
    },
    master: {
        // australiaeast: {
        //     ingressIp: masterStack.getOutput(outputName)
        //         .apply((k) => k?.australiaeast.ingressIp) as pulumi.Output<string>,
        //     location: masterStack.getOutput(outputName)
        //         .apply((k) => k?.australiaeast.location) as pulumi.Output<string>,
        // },
        australiasoutheast: {
            ingressIp: masterStack
                .getOutput(outputName)
                .apply((k) => k?.australiasoutheast.ingressIp) as pulumi.Output<
                string
            >,
            location: masterStack
                .getOutput(outputName)
                .apply((k) => k?.australiasoutheast.location) as pulumi.Output<
                string
            >,
        },
    },
    prod: {
        // australiaeast: {
        //     ingressIp: prodStack.requireOutput(outputName)
        //         .apply((k) => k.australiaeast.ingressIp) as pulumi.Output<string>,
        //     location: prodStack.requireOutput(outputName)
        //         .apply((k) => k.australiaeast.location) as pulumi.Output<string>,
        // },
        australiasoutheast: {
            ingressIp: prodStack
                .requireOutput(outputName)
                .apply((k) => k.australiasoutheast.ingressIp) as pulumi.Output<
                string
            >,
            location: prodStack
                .requireOutput(outputName)
                .apply((k) => k.australiasoutheast.location) as pulumi.Output<
                string
            >,
        },
    },
};

export const frontendHosts = createDns(rg, clusters);

const kv = new azure.keyvault.KeyVault(
    "central-keyVault",
    {
        accessPolicies: [
            {
                certificatePermissions: [
                    "create",
                    "list",
                    "get",
                    "delete",
                    "listissuers",
                    "import",
                    "manageissuers",
                    "managecontacts",
                ],
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
    opts
);

export const backendEnvironments = createFrontDoor({
    develop: {
        identityUrl: developIdentityHostnames as pulumi.Output<string[]>,
        webAppUrl: developAppHostnames as pulumi.Output<string[]>,
    },
    frontendHosts,
    kv,
    master: {
        identityUrl: masterIdentityHostnames as pulumi.Output<string[]>,
        webAppUrl: masterAppHostnames as pulumi.Output<string[]>,
    },
    prod: {
        identityUrl: prodIdentityHostnames as pulumi.Output<string[]>,
        webAppUrl: prodAppHostnames as pulumi.Output<string[]>,
    },
    rg,
});
