import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS } from "../../../components";
import { Monitoring } from "../../monitoring/monitoring";
import { Network } from "../../network/network";
import { State } from "../../state/state";

export interface IAppSvcParams {
    rg: azure.core.ResourceGroup;
    kv: azure.keyvault.KeyVault;
    acr: azure.containerservice.Registry;
    state: State;
    monitoring: Monitoring;
    network: Network;
}

const cfg = new pulumi.Config();
const config = new pulumi.Config("application");

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "appSvc",
};

export class AppSvc extends pulumi.ComponentResource {
    public plan: azure.appservice.Plan;
    public imageName: pulumi.Output<string>;
    public appSvc: azure.appservice.AppService;
    public appSvcStaging: azure.appservice.Slot;

    constructor(
        name: string,
        private params: IAppSvcParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:AppSvc", name, {}, opts);
        this.create(params.rg, params.kv, params.acr);
    }
    private create(
        rg: azure.core.ResourceGroup,
        kv: azure.keyvault.KeyVault,
        acr: azure.containerservice.Registry,
    ) {
        this.plan = new azure.appservice.Plan(
            "appSvcPlan",
            {
                kind: "Linux",
                location: rg.location,
                reserved: true,
                resourceGroupName: rg.name,
                sku: {
                    size: config.require("appSvcPlanSize"),
                    tier: config.require("appSvcPlanTier"),
                },
                tags,
            },
            {
                parent: rg,
            },
        );
        const secretString = cfg.requireSecret("tokenManagement__secret");
        this.imageName = pulumi.interpolate`${acr.loginServer}/${CONSTANTS.application.imageName}:latest`;

        const appSettings = {
            APPINSIGHTS_INSTRUMENTATIONKEY: this.params.monitoring.applicationInsights.instrumentationKey,
            DOCKER_REGISTRY_SERVER_PASSWORD: acr.adminPassword,
            DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${
                acr.loginServer
                }`,
            DOCKER_REGISTRY_SERVER_USERNAME: acr.adminUsername,
            Registration__Token: "AmphoraData",
            STACK: pulumi.getStack(),
            WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
            WEBSITES_PORT: 80,
            kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName,
            kvUri: kv.vaultUri,
        };
        const siteConfig = {
            alwaysOn: true,
            linuxFxVersion: pulumi.interpolate`DOCKER|${this.imageName}`,
        };

        this.appSvc = new azure.appservice.AppService(
            "appSvc",
            {
                appServicePlanId: this.plan.id,
                appSettings,
                httpsOnly: true,
                identity: { type: "SystemAssigned" },
                location: rg.location,
                resourceGroupName: rg.name,
                siteConfig,
                tags,
            },
            {
                parent: this.plan,
            },
        );
        if (config.require("appSvcPlanTier") === "Standard") {
            appSettings.STACK += "(staging)";
            this.appSvcStaging = new azure.appservice.Slot("stagingSlot", {
                appServiceName: this.appSvc.name,
                appServicePlanId: this.plan.id,
                appSettings,
                httpsOnly: true,
                identity: { type: "SystemAssigned" },
                location: rg.location,
                name: "staging",
                resourceGroupName: rg.name,
                siteConfig,
                tags,
            },
            {
                parent: rg,
            });
        }

        // section--key
        this.params.state.storeInVault("jwtToken", "tokenManagement--secret", secretString);
        this.params.network.AddCNameRecord("primary", this.appSvc.defaultSiteHostname);
        this.accessPolicyKeyVault("appSvc-access", this.params.state.kv, this.appSvc);
        if (this.appSvcStaging) {
            this.accessPolicyKeyVault("appSvcStaging-access", this.params.state.kv, this.appSvcStaging);
        }
    }

    private accessPolicyKeyVault(
        name: string,
        kv: azure.keyvault.KeyVault,
        appSvc: azure.appservice.AppService | azure.appservice.Slot,
    ) {
        return new azure.keyvault.AccessPolicy(
            name,
            {
                keyVaultId: kv.id,
                objectId: appSvc.identity.apply(
                    (identity) =>
                        identity.principalId || "11111111-1111-1111-1111-111111111111",
                ), // https://github.com/pulumi/pulumi-azure/issues/192
                secretPermissions: ["get", "list"],
                tenantId: CONSTANTS.authentication.tenantId,
            },
            {
                parent: appSvc,
            },
        );
    }
}
