import * as azure from "@pulumi/azure";
import * as k8s from "@pulumi/kubernetes";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS } from "../../../components";
import { Monitoring } from "../../monitoring/monitoring";
import { Network } from "../../network/network";
import { State } from "../../state/state";
import { getK8sAppSettings } from "../appSettings";
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
    location: string;
}

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "aks",
};

export interface IK8sIdentities {
    webApp: azure.authorization.UserAssignedIdentity;
    identityServer: azure.authorization.UserAssignedIdentity;
}

// Step 1: Parse and export configuration variables for the AKS stack.

const cfg = new pulumi.Config();
const config = new pulumi.Config("aks");

const appId = config.require("spAppId");
const password = cfg.requireSecret("aksSpAppPassword");

const sshPublicKey = cfg.requireSecret("sshPublicKey");

const nodeCount = config.getNumber("nodeCount") || 2;
const nodeSize = config.get("nodeSize") || "Standard_D2_v2";

export class Aks extends pulumi.ComponentResource {
    public k8sCluster: azure.containerservice.KubernetesCluster;
    public k8sProvider: k8s.Provider;
    // public webAppIdentity: azure.authorization.UserAssignedIdentity;
    public identities: IK8sIdentities;
    public k8sInfra: K8sInfrastructure;
    private kvAccessPolicies: azure.keyvault.AccessPolicy[] = [];

    constructor(
        private name: string,
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
        this.k8sCluster = new azure.containerservice.KubernetesCluster(`${this.name}-cluster`, {
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
            location: this.params.location,
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

        // Expose a k8s provider instance using our custom cluster instance.
        this.k8sProvider = new k8s.Provider(`${this.name}-aksK8s`, {
            kubeconfig: this.k8sCluster.kubeConfigRaw,
        }, {
            parent: this,
        });

        const webApp = new azure.authorization.UserAssignedIdentity(`${this.name}-webApp`, {
            location: rg.location,
            resourceGroupName: this.k8sCluster.nodeResourceGroup,
            tags,
        });

        const identityServer = new azure.authorization.UserAssignedIdentity(`${this.name}-idServer`, {
            location: rg.location,
            resourceGroupName: this.k8sCluster.nodeResourceGroup,
            tags,
        });

        this.identities = { webApp, identityServer };

        this.addMsiToKeyVault(`${this.name}-k8sWebApp`, this.params.kv, webApp);
        this.addMsiToKeyVault(`${this.name}-k8sIdSrv`, this.params.kv, identityServer);

        // Export the kubeconfig
        // this.kubeconfig = k8sCluster.kubeConfigRaw

        // Step 3: Install dependencies into the cluster
        const appSettings = getK8sAppSettings(kv, this.k8sCluster.location)
        this.k8sInfra = new K8sInfrastructure(`${this.name}-infra`, {
            appSettings,
            identities: this.identities,
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
