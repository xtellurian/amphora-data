import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { CONSTANTS, IComponentParams } from "../../components";
import { Monitoring } from "../monitoring/monitoring";
import { Network } from "../network/network";
import { State } from "../state/state";
import { AzureMaps } from "./maps/azure-maps";
import { Tsi } from "./tsi/tsi";

const cfg = new pulumi.Config();
const config = new pulumi.Config("application");

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
    private network: Network,
    private state: State,
  ) {
    super("amphora:Application", params.name, {}, params.opts);
    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: CONSTANTS.location.primary,
        tags: azTags,
      },
      {
        parent: this,
      },
    );
    this.acr = this.createAcr(rg);

    this.createAzureMaps(rg);
    this.createAppSvc(rg, this.state.kv);
    this.accessPolicyKeyVault(this.state.kv, this.appSvc);
    this.createTsi();
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
    let sku = config.get("acrSku");
    if (!sku) { sku = "Basic"; } // default SKU is basic
    const acr = new azure.containerservice.Registry(
      "acr",
      {
        adminEnabled: true,
        location: CONSTANTS.location.primary,
        resourceGroupName: rg.name,
        sku,
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
    const secretString = cfg.requireSecret("tokenManagement__secret");
    this.imageName = pulumi.interpolate`${this.acr.loginServer}/${CONSTANTS.application.imageName}:latest`;
    this.appSvc = new azure.appservice.AppService(
      "appSvc",
      {
        appServicePlanId: this.plan.id,
        appSettings: {
          APPINSIGHTS_INSTRUMENTATIONKEY: this.monitoring.applicationInsights.instrumentationKey,
          AzureMapsClientId: this.AzureMaps.maps.xMsClientId,
          DOCKER_REGISTRY_SERVER_PASSWORD: this.acr.adminPassword,
          DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${
            this.acr.loginServer
            }`,
          DOCKER_REGISTRY_SERVER_USERNAME: this.acr.adminUsername,
          Registration__Token: "AmphoraData",
          WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
          WEBSITES_PORT: 80,
          kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName,
          kvUri: kv.vaultUri,
          // tokenManagement__secret: secretString,
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

    // section--key
    this.state.storeInVault("jwtToken", "tokenManagement--secret", secretString);
    this.network.AddCNameRecord("primary", this.appSvc.defaultSiteHostname);
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
        tenantId: CONSTANTS.authentication.tenantId,
      },
      {
        parent: appSvc,
      },
    );
  }

  private createAzureMaps(rg: azure.core.ResourceGroup) {
    this.AzureMaps = new AzureMaps("azMaps", { rg }, { parent: this });

    const subId = CONSTANTS.authentication.subscriptionId;
    // tslint:disable-next-line: max-line-length
    const roleId = `/subscriptions/${subId}/providers/Microsoft.Authorization/roleDefinitions/423170ca-a8f6-4b0f-8487-9e4eb8f49bfa`;
    const appRole = new azure.role.Assignment("appRole",
      {
        principalId: CONSTANTS.authentication.spObjectId,
        roleDefinitionId: roleId,
        scope: this.AzureMaps.maps.id,
      },
      {
        parent: this,
      });

    const rianRole = new azure.role.Assignment("rianRole",
      {
        principalId: CONSTANTS.authentication.rian,
        roleDefinitionId: roleId,
        scope: this.AzureMaps.maps.id,
      },
      {
        parent: this,
      });

    this.state.storeInVault("AzureMapsKey", "AzureMaps--Key", this.AzureMaps.maps.primaryAccessKey);
    this.state.storeInVault("AzureMapsClientId", "AzureMaps--ClientId", this.AzureMaps.maps.xMsClientId);
  }
}
