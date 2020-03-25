import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { IMultiEnvironmentMultiCluster } from "./contracts";
import { createDns } from "./dns/dns";
import { createFrontDoor } from "./front-door/frontDoor";

const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
const masterStack = new pulumi.StackReference(`xtellurian/amphora/master`);
const developStack = new pulumi.StackReference(`xtellurian/amphora/develop`);

const prodHostnames = prodStack.getOutput("appHostnames"); // should be an array

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
const primary = "k8sPrimary";
const secondary = "k8sSecondary";
const clusters: IMultiEnvironmentMultiCluster = {
    develop: {
        primary: {
            ingressIp: developStack.getOutput(primary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: developStack.getOutput(primary).apply((k) => k.location) as pulumi.Output<string>,
        },
        secondary: {
            ingressIp: developStack.getOutput(secondary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: developStack.getOutput(secondary).apply((k) => k.location) as pulumi.Output<string>,
        },
    },
    master: {
        primary: {
            ingressIp: masterStack.getOutput(primary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: masterStack.getOutput(primary).apply((k) => k.location) as pulumi.Output<string>,
        },
        secondary: {
            ingressIp: masterStack.getOutput(secondary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: masterStack.getOutput(secondary).apply((k) => k.location) as pulumi.Output<string>,
        },
    },
    prod: {
        primary: {
            ingressIp: prodStack.requireOutput(primary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: prodStack.requireOutput(primary).apply((k) => k.location) as pulumi.Output<string>,
        },
        secondary: {
            ingressIp: prodStack.requireOutput(secondary).apply((k) => k.ingressIp) as pulumi.Output<string>,
            location: prodStack.requireOutput(secondary).apply((k) => k.location) as pulumi.Output<string>,
        },
    },
};

export const frontendHosts = createDns(rg, clusters);

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

export const backendEnvironments = createFrontDoor({ rg, kv, frontendHosts, prodHostnames });
