import * as azure from "@pulumi/azure";
import * as azuread from "@pulumi/azuread";
import * as k8s from "@pulumi/kubernetes";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS } from "../../../components";
import { Monitoring } from "../../monitoring/monitoring";
import { Network } from "../../network/network";
import { State } from "../../state/state";

export interface IAksParams {
    acr: azure.containerservice.Registry;
    rg: azure.core.ResourceGroup;
    kv: azure.keyvault.KeyVault;
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

const password = cfg.requireSecret("aksSpPw");
const sshPublicKey = cfg.requireSecret("sshPublicKey");

const location = CONSTANTS.location.primary;
const failoverLocation = CONSTANTS.location.secondary;
const nodeCount = config.getNumber("nodeCount") || 2;
const nodeSize = config.get("nodeSize") || "Standard_D2_v2";

export class Aks extends pulumi.ComponentResource {
    public k8sCluster: azure.containerservice.KubernetesCluster;
    public k8sProvider: k8s.Provider;

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

        // Step 2: Create the AD service principal for the k8s cluster.
        const adApp = new azuread.Application("aks", {}, { parent: this });
        const adSp = new azuread.ServicePrincipal("aksSp", { applicationId: adApp.applicationId }, { parent: this });
        const adSpPassword = new azuread.ServicePrincipalPassword("aksSpPassword", {
            endDate: "2099-01-01T00:00:00Z",
            servicePrincipalId: adSp.id,
            value: password,
        }, {
            parent: this,
        });

        // Step 3: This step creates an AKS cluster.
        // this.k8sCluster = new azure.containerservice.KubernetesCluster("aksCluster", {
        //     addonProfile: {
        //         omsAgent: {
        //             enabled: true,
        //             logAnalyticsWorkspaceId: logAnalytics.id,
        //         },
        //     },
        //     defaultNodePool: {
        //         name: "standard",
        //         nodeCount,
        //         vmSize: nodeSize,
        //     },
        //     dnsPrefix: `${pulumi.getStack()}-kube`,
        //     linuxProfile: {
        //         adminUsername: "aksuser",
        //         sshKey: { keyData: sshPublicKey },
        //     },
        //     location,
        //     resourceGroupName: rg.name,
        //     servicePrincipal: {
        //         clientId: adApp.applicationId,
        //         clientSecret: adSpPassword.value,
        //     },
        //     tags,
        // }, {
        //     parent: this,
        // });

        // Expose a k8s provider instance using our custom cluster instance.
        // this.k8sProvider = new k8s.Provider("aksK8s", {
        //     kubeconfig: this.k8sCluster.kubeConfigRaw,
        // }, {
        //     parent: this,
        // });

        // Export the kubeconfig
        // this.kubeconfig = k8sCluster.kubeConfigRaw
    }
}
