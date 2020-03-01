import * as azure from "@pulumi/azure";
import * as k8s from "@pulumi/kubernetes";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS } from "../../../components";
import { Monitoring } from "../../monitoring/monitoring";
import { Network } from "../../network/network";
import { State } from "../../state/state";
import { K8sInfrastructure } from "./k8s-infrastructure";

export interface IAksParams {
    acr: azure.containerservice.Registry;
    rg: azure.core.ResourceGroup;
    kv: azure.keyvault.KeyVault;
    appSettings: pulumi.Input<{
        [key: string]: pulumi.Input<string>;
    }>;
    monitoring: Monitoring;
    network: Network;
    state: State;
}

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "aks",
};

// Step 1: Parse and export configuration variables for the AKS stack.

const cfg = new pulumi.Config();
const config = new pulumi.Config("aks");

const appId = config.require("spAppId");
const objectId = config.require("spObjectId");
const password = cfg.requireSecret("aksSpAppPassword");

const sshPublicKey = cfg.requireSecret("sshPublicKey");

const location = CONSTANTS.location.primary;
const failoverLocation = CONSTANTS.location.secondary;
const nodeCount = config.getNumber("nodeCount") || 2;
const nodeSize = config.get("nodeSize") || "Standard_D2_v2";

export class Aks extends pulumi.ComponentResource {
    public k8sCluster: azure.containerservice.KubernetesCluster;
    public k8sProvider: k8s.Provider;
    public webAppIdentity: azure.authorization.UserAssignedIdentity;
    public k8sInfra: K8sInfrastructure;
    private kvAccessPolicies: azure.keyvault.AccessPolicy[] = [];

    constructor(
        name: string,
        private params: IAksParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:Aks", name, {}, opts);
        this.create(params.rg, params.kv, params.acr, params.monitoring.logAnalyticsWorkspace);
    }

    private create(
        rg: azure.core.ResourceGroup,
        kv: azure.keyvault.KeyVault,
        acr: azure.containerservice.Registry,
        logAnalytics: azure.operationalinsights.AnalyticsWorkspace) {

        // Step 2: This step creates an AKS cluster.
        this.k8sCluster = new azure.containerservice.KubernetesCluster("aksCluster", {
            addonProfile: {
                omsAgent: {
                    enabled: true,
                    logAnalyticsWorkspaceId: logAnalytics.id,
                },
            },
            defaultNodePool: {
                name: "standard",
                nodeCount,
                vmSize: nodeSize,
            },
            dnsPrefix: `${pulumi.getStack()}-kube`,
            linuxProfile: {
                adminUsername: "aksuser",
                sshKey: { keyData: sshPublicKey },
            },
            location,
            resourceGroupName: rg.name,
            roleBasedAccessControl: {
                enabled: true,
            },
            servicePrincipal: {
                clientId: appId,
                clientSecret: password,
            },
            tags,
        }, {
            parent: this,
        });

        const roleAssignment = new azure.role.Assignment("acrPullAccess", {
            principalId: objectId,
            roleDefinitionName: "AcrPull",
            scope: this.params.acr.id,
        }, {
            parent: this,
        });

        // Expose a k8s provider instance using our custom cluster instance.
        this.k8sProvider = new k8s.Provider("aksK8s", {
            kubeconfig: this.k8sCluster.kubeConfigRaw,
        }, {
            parent: this,
        });

        this.webAppIdentity = new azure.authorization.UserAssignedIdentity("webApp", {
            location: rg.location,
            resourceGroupName: this.k8sCluster.nodeResourceGroup,
            tags,
        });

        this.addMsiToKeyVault("k8sWebApp", this.params.kv, this.webAppIdentity);

        // Export the kubeconfig
        // this.kubeconfig = k8sCluster.kubeConfigRaw

        // Step 3: Install dependencies into the cluster

        this.k8sInfra = new K8sInfrastructure("k8sInfra", {
            appSettings: this.params.appSettings,
            identity: this.webAppIdentity,
            location: this.k8sCluster.location,
            provider: this.k8sProvider,
        }, { parent: this });

    }

    private addMsiToKeyVault(
        name: string,
        kv: azure.keyvault.KeyVault,
        identity: azure.authorization.UserAssignedIdentity) {
        const ap = new azure.keyvault.AccessPolicy(
            name,
            {
                // applicationId: identity.clientId,
                keyPermissions: ["unwrapKey", "wrapKey"],
                keyVaultId: kv.id,
                objectId: identity.principalId,
                secretPermissions: ["get", "list"],
                tenantId: CONSTANTS.authentication.tenantId,
            },
            {
                parent: this,
            },
        );
        this.kvAccessPolicies.push(ap);
    }
}
