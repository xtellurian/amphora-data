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

interface IPlanAndSlot {
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
    public plan: azure.appservice.Plan;
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

        const plans = appsConfig.requireObject<IAppServicePlanConfig[]>("plans");
        plans.forEach((p) => {
            this.createPlan(rg, kv, acr, p);
        });
    }

    private createPlan(rg: azure.core.ResourceGroup,
                       kv: azure.keyvault.KeyVault,
                       acr: azure.containerservice.Registry,
                       plan: IAppServicePlanConfig) {
        this.plan = new azure.appservice.Plan(
            "appSvcPlan",
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
        const secretString = cfg.requireSecret("tokenManagement__secret");
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
                ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"], // don't reset every time
                parent: this.plan,
            },
        );
        let appSvcStaging = null;
        // need second for deep copying reasons
        if (plan.planTier === "Standard") {
            appSvcStaging = new azure.appservice.Slot("stagingSlot", {
                appServiceName: appSvc.name,
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
                ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"], // don't reset every time
                parent: rg,
            });
        }

        // section--key
        this.params.state.storeInVault("jwtToken", "tokenManagement--secret", secretString);
        this.params.network.AddCNameRecord("primary", appSvc.defaultSiteHostname);
        this.accessPolicyKeyVault("appSvc-access", this.params.state.kv, appSvc);
        if (appSvcStaging) {
            this.accessPolicyKeyVault("appSvcStaging-access", this.params.state.kv, appSvcStaging);
        }
        this.apps.push({appSvc, appSvcStaging});
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
