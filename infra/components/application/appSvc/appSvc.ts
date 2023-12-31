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

const stack = pulumi.getStack();

const cfg = new pulumi.Config();
const appsConfig = new pulumi.Config("apps");

interface IExternalServices {
  identityBaseUrl: string;
  webAppBaseUrl: string;
}

const externalServices = appsConfig.requireObject<IExternalServices>(
  "externalServices"
);

const tags = {
  component: "application",
  project: pulumi.getProject(),
  source: "pulumi",
  stack,
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
  // public imageName: pulumi.Output<string>;
  public mainApps: IPlanAndSlot[] = [];
  public identityApps: IPlanAndSlot[] = [];
  public appSettings: pulumi.Input<{
    [key: string]: pulumi.Input<string>;
  }>;
  private kvAccessPolicies: azure.keyvault.AccessPolicy[] = [];
  constructor(
    name: string,
    private params: IAppSvcParams,
    opts?: pulumi.ComponentResourceOptions
  ) {
    super("amphora:AppSvc", name, {}, opts);
    this.create(params.rg, params.kv, params.acr);
  }
  private create(
    rg: azure.core.ResourceGroup,
    kv: azure.keyvault.KeyVault,
    acr: azure.containerservice.Registry
  ) {
    const secretString = cfg.requireSecret("tokenManagement__secret");
    this.params.state.storeInVault(
      "jwtToken",
      "tokenManagement--secret",
      secretString
    );
    const MvcClientSecret = cfg.requireSecret("mvcClientSecret");
    this.params.state.storeInVault(
      "MvcClientSecret",
      "MvcClientSecret",
      MvcClientSecret
    );
    const plans = appsConfig.requireObject<IAppServicePlanConfig[]>("plans");
    plans.forEach((p) => {
      this.createPlan(rg, kv, acr, p);
    });
  }

  private createPlan(
    rg: azure.core.ResourceGroup,
    kv: azure.keyvault.KeyVault,
    acr: azure.containerservice.Registry,
    plan: IAppServicePlanConfig
  ) {
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
      }
    );

    const host = appsConfig.get("mainHost")
      ? appsConfig.require("mainHost")
      : "";
    const appSettings = {
      APPINSIGHTS_INSTRUMENTATIONKEY: this.params.monitoring.applicationInsights
        .instrumentationKey, // important
      DOCKER_REGISTRY_SERVER_PASSWORD: acr.adminPassword,
      DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${acr.loginServer}`,
      DOCKER_REGISTRY_SERVER_USERNAME: acr.adminUsername,
      DisableHsts: "",
      Environment__Location: "",
      Environment__Stack: stack,
      ExternalServices__IdentityBaseUrl: externalServices.identityBaseUrl,
      ExternalServices__WebAppBaseUrl: externalServices.webAppBaseUrl,
      Host__MainHost: host, // important
      Logging__ApplicationInsights__LogLevel__Default: "Warning",
      Registration__Token: "AmphoraData",
      // Segment__TrackingId: appsConfig.require("segmentTrackingId"),
      WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
      WEBSITES_PORT: "80",
      kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName, // important
      kvUri: kv.vaultUri, // important
    };

    this.appSettings = appSettings;

    this.mainApps.push(
      this.createApp(
        appSettings,
        CONSTANTS.application.imageName,
        acr.loginServer,
        plan,
        appSvcPlan,
        rg
      )
    );
    this.identityApps.push(
      this.createApp(
        appSettings,
        CONSTANTS.identity.imageName,
        acr.loginServer,
        plan,
        appSvcPlan,
        rg
      )
    );
  }

  private createApp(
    appSettings: any,
    imageName: string,
    acrLoginServer: pulumi.Output<string>,
    plan: IAppServicePlanConfig,
    appSvcPlan: azure.appservice.Plan,
    rg: azure.core.ResourceGroup
  ) {
    if (stack === "develop" || stack === "master") {
      appSettings.DisableHsts = "true"; // disable enforced hsts on develop and master stacks
    }
    const uniqueAppName = plan.name.substr(0, 3) + imageName.substr(0, 3);
    const fullImageName = pulumi.interpolate`${acrLoginServer}/${imageName}`;

    // Add cors for applicationsƒ
    const cors = {
      allowedOrigins: ["*"],
    };

    const siteConfig = {
      alwaysOn: true,
      cors,
      healthCheckPath: "/healthz",
      linuxFxVersion: pulumi.interpolate`DOCKER|${fullImageName}:latest`,
    };

    const appSvc = new azure.appservice.AppService(
      uniqueAppName,
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
        ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"],
        parent: appSvcPlan,
      }
    );
    let appSvcStaging = null;
    // need second for deep copying reasons
    if (plan.planTier === "Standard") {
      appSvcStaging = new azure.appservice.Slot(
        uniqueAppName + "Slot",
        {
          appServiceName: appSvc.name,
          appServicePlanId: appSvcPlan.id,
          appSettings,
          httpsOnly: true,
          identity: { type: "SystemAssigned" },
          location: appSvcPlan.location,
          name: "staging",
          resourceGroupName: rg.name,
          siteConfig,
          tags,
        },
        {
          ignoreChanges: ["appSettings.siteConfig.linuxFxVersion"],
          parent: rg,
        }
      );
    }

    // section--key
    this.accessPolicyKeyVault(
      uniqueAppName + "-acs",
      this.params.state.kv,
      appSvc
    );
    if (appSvcStaging) {
      this.accessPolicyKeyVault(
        uniqueAppName + "StagingAccess",
        this.params.state.kv,
        appSvcStaging
      );
    }
    return { name: uniqueAppName, appSvc, appSvcStaging };
  }

  private accessPolicyKeyVault(
    name: string,
    kv: azure.keyvault.KeyVault,
    appSvc: azure.appservice.AppService | azure.appservice.Slot
  ) {
    const ap = new azure.keyvault.AccessPolicy(
      name,
      {
        certificatePermissions: ["get", "list", "update"],
        keyPermissions: ["unwrapKey", "wrapKey"],
        keyVaultId: kv.id,
        objectId: appSvc.identity.apply(
          (identity) =>
            identity.principalId || "11111111-1111-1111-1111-111111111111"
        ), // https://github.com/pulumi/pulumi-azure/issues/192
        secretPermissions: ["get", "list"],
        tenantId: CONSTANTS.authentication.tenantId,
      },
      {
        parent: this,
      }
    );
    this.kvAccessPolicies.push(ap);
  }
}
