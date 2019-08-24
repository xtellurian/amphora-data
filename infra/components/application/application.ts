import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { CONSTANTS, IComponentParams } from "../../components";
import { Monitoring } from "../monitoring/monitoring";
import { State } from "../state/state";
import { AzureMaps } from "./maps/azure-maps";
import { Tsi } from "./tsi/tsi";

const config = new pulumi.Config("application");
const authConfig = new pulumi.Config("authentication");
const azTags = {
  component: "application",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};
const rgName = pulumi.getStack() + "-app";

export interface IApplication {
  appSvc: azure.appservice.AppService;
  plan: azure.appservice.Plan;
}

export class Application extends pulumi.ComponentResource
  implements IApplication {
  public appSvc: azure.appservice.AppService;
  public plan: azure.appservice.Plan;
  public acr: azure.containerservice.Registry;
  public tsi: Tsi;
  public imageName: pulumi.Output<string>;
  public AzureMaps: AzureMaps;

  constructor(
    params: IComponentParams,
    private monitoring: Monitoring,
    private state: State,
  ) {
    super("amphora:Application", params.name, {}, params.opts);
    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: config.require("location"),
        tags: azTags,
      },
      {
        parent: this,
      },
    );
    this.acr = this.createAcr(rg);

    this.createAppSvc(rg, this.state.kv);
    this.accessPolicyKeyVault(this.state.kv, this.appSvc);
    this.createTsi();
    this.createAzureMaps(rg);
  }
  private createTsi() {
    this.tsi = new Tsi("tsi", {
      appSvc: this.appSvc,
      eh: this.state.eh,
      eh_namespace: this.state.ehns,
      state: this.state,
    }, {
        parent: this,
      });
  }

  private createAcr(rg: azure.core.ResourceGroup) {
    const acr = new azure.containerservice.Registry(
      "acr",
      {
        adminEnabled: true,
        location: config.require("location"),
        resourceGroupName: rg.name,
        sku: "Standard",
        tags: azTags,
      },
      { parent: rg },
    );
    return acr;
  }

  private createAppSvc(
    rg: azure.core.ResourceGroup,
    kv: azure.keyvault.KeyVault,
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
        tags: azTags,
      },
      {
        parent: rg,
      },
    );

    this.imageName = pulumi.interpolate`${this.acr.loginServer}/${config.require("imageName")}:latest`;
    this.appSvc = new azure.appservice.AppService(
      "appSvc",
      {
        appServicePlanId: this.plan.id,
        appSettings: {
          APPINSIGHTS_INSTRUMENTATIONKEY: this.monitoring.applicationInsights.instrumentationKey,
          AzureMapsClientId: this.AzureMaps.clientId,
          DOCKER_ENABLE_CI: "true",
          DOCKER_REGISTRY_SERVER_PASSWORD: this.acr.adminPassword,
          DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${
            this.acr.loginServer
            }`,
          DOCKER_REGISTRY_SERVER_USERNAME: this.acr.adminUsername,
          WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
          WEBSITES_PORT: 80,
          kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName,
          kvUri: kv.vaultUri,
        },
        httpsOnly: true,
        identity: { type: "SystemAssigned" },
        location: rg.location,
        resourceGroupName: rg.name,
        siteConfig: {
          alwaysOn: true,
          linuxFxVersion: pulumi.interpolate`DOCKER|${this.imageName}`,
        },
        tags: azTags,
      },
      {
        parent: this.plan,
      },
    );
  }

  private accessPolicyKeyVault(
    kv: azure.keyvault.KeyVault,
    appSvc: azure.appservice.AppService,
  ) {
    return new azure.keyvault.AccessPolicy(
      "appSvc-access",
      {
        keyVaultId: kv.id,
        objectId: appSvc.identity.apply(
          (identity) =>
            identity.principalId || "11111111-1111-1111-1111-111111111111",
        ), // https://github.com/pulumi/pulumi-azure/issues/192
        secretPermissions: ["get", "list"],
        tenantId: authConfig.require("tenantId"),
      },
      {
        parent: appSvc,
      },
    );
  }

  private createAzureMaps(rg: azure.core.ResourceGroup) {
    this.AzureMaps = new AzureMaps("azMaps", { rg }, { parent: this });

    const subId = authConfig.require("subscriptionId");
    // tslint:disable-next-line: max-line-length
    const roleId = `/subscriptions/${subId}/providers/Microsoft.Authorization/roleDefinitions/423170ca-a8f6-4b0f-8487-9e4eb8f49bfa`;
    const appRole = new azure.role.Assignment("appRole",
      {
        principalId: authConfig.require("spObjectId"),
        roleDefinitionId: roleId,
        scope: this.AzureMaps.resourceId,
      },
      {
        parent: this,
      });

    const rianRole = new azure.role.Assignment("rianRole",
      {
        principalId: authConfig.require("rian"),
        roleDefinitionId: roleId,
        scope: this.AzureMaps.resourceId,
      },
      {
        parent: this,
      });
  }
}
