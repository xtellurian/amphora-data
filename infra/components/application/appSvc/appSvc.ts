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
const appsConfig = new pulumi.Config("apps");

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "appSvc",
};

export interface IPlanAndSlot {
    name: string;
    appSvc: azure.appservice.AppService;
    appSvcStaging: azure.appservice.Slot | null;
}
interface IAppServicePlanConfig {
    planSize: string;
    planTier: string;
    location: string;
    name: string;
}

export class AppSvc extends pulumi.ComponentResource {
    public imageName: pulumi.Output<string>;
    public apps: IPlanAndSlot[] = [];

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
        const secretString = cfg.requireSecret("tokenManagement__secret");
        this.params.state.storeInVault("jwtToken", "tokenManagement--secret", secretString);
        const plans = appsConfig.requireObject<IAppServicePlanConfig[]>("plans");
        plans.forEach((p) => {
            this.createPlan(rg, kv, acr, p);
        });
    }

    private createPlan(rg: azure.core.ResourceGroup,
                       kv: azure.keyvault.KeyVault,
                       acr: azure.containerservice.Registry,
                       plan: IAppServicePlanConfig) {
        const appSvcPlan = new azure.appservice.Plan(
            plan.name + "Plan",
            {
                kind: "Linux",
                location: plan.location,
                reserved: true,
                resourceGroupName: rg.name,
                sku: {
                    size: plan.planSize,
                    tier: plan.planTier,
                },
                tags,
            },
            {
                parent: rg,
            },
        );

        this.imageName = pulumi.interpolate`${acr.loginServer}/${CONSTANTS.application.imageName}`;
        const host = config.get("mainHost") ? config.require("mainHost") : "";
        const appSettings = {
            APPINSIGHTS_INSTRUMENTATIONKEY: this.params.monitoring.applicationInsights.instrumentationKey,
            DOCKER_REGISTRY_SERVER_PASSWORD: acr.adminPassword,
            DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${
                acr.loginServer
                }`,
            DOCKER_REGISTRY_SERVER_USERNAME: acr.adminUsername,
            Host__MainHost: host,
            Logging__ApplicationInsights__LogLevel__Default: "Warning",
            Registration__Token: "AmphoraData",
            STACK: pulumi.getStack(),
            WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
            WEBSITES_PORT: "80",
            kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName,
            kvUri: kv.vaultUri,
        };
        const siteConfig = {
            alwaysOn: true,
            linuxFxVersion: pulumi.interpolate`DOCKER|${this.imageName}:latest`, // to make the app service = container
        };

        const appSvc = new azure.appservice.AppService(
            plan.name,
            {
                appServicePlanId: appSvcPlan.id,
                appSettings,
                httpsOnly: true,
                identity: { type: "SystemAssigned" },
                location: appSvcPlan.location,
                resourceGroupName: rg.name,
                siteConfig,
                tags,
            },
            {
                ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"], // don't reset every time
                parent: appSvcPlan,
            },
        );
        let appSvcStaging = null;
        // need second for deep copying reasons
        if (plan.planTier === "Standard") {
            appSvcStaging = new azure.appservice.Slot(plan.name + "Slot", {
                appServiceName: appSvc.name,
                appServicePlanId: appSvcPlan.id,
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
                ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"], // don't reset every time
                parent: rg,
            });
        }

        // section--key
        this.params.network.AddCNameRecord(plan.name, appSvc.defaultSiteHostname);
        this.accessPolicyKeyVault(plan.name + "-access", this.params.state.kv, appSvc);
        if (appSvcStaging) {
            this.accessPolicyKeyVault(plan.name + "StagingAccess", this.params.state.kv, appSvcStaging);
        }
        this.apps.push({name: plan.name, appSvc, appSvcStaging});
    }

    private accessPolicyKeyVault(
        name: string,
        kv: azure.keyvault.KeyVault,
        appSvc: azure.appservice.AppService | azure.appservice.Slot,
    ) {
        return new azure.keyvault.AccessPolicy(
            name,
            {
                keyPermissions: ["unwrapKey", "wrapKey"],
                keyVaultId: kv.id,
                objectId: appSvc.identity.apply(
                    (identity) =>
                        identity.principalId || "11111111-1111-1111-1111-111111111111",
                ), // https://github.com/pulumi/pulumi-azure/issues/192
                secretPermissions: ["get", "list"],
                tenantId: CONSTANTS.authentication.tenantId,
            },
            {
                parent: this,
            },
        );
    }
}
