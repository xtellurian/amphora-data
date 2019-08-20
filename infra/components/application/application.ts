import * as azure from "@pulumi/azure";
import * as docker from "@pulumi/docker";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS, IComponentParams } from "../../components";
import { AzureConfig } from "../azure-config/azure-config";
import { Monitoring } from "../monitoring/monitoring";
import { State } from "../state/state";
import { Tsi } from "./tsi/tsi";

const config = new pulumi.Config("application");
const azTags = {
  component: "application",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};
const rgName = pulumi.getStack() + "-app";

// lives in here for now
// export class ApplicationParams implements IComponentParams {
//   public name: string = "d-app-component";
//   public opts?: pulumi.ComponentResourceOptions | undefined = undefined;
// }

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

  constructor(
    params: IComponentParams,
    private monitoring: Monitoring,
    private state: State,
    private azConfig: AzureConfig,
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

    // const image = this.buildApp(this.acr);

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

  // private buildApp(registry: azure.containerservice.Registry): docker.Image {
  //   const myImage = new docker.Image(
  //     "acrImage",
  //     {
  //       imageName: pulumi.interpolate`${
  //         registry.loginServer
  //         }/${customImage}:v1.0.0`,
  //       build: {
  //         context: `../apps`
  //       },
  //       registry: {
  //         server: registry.loginServer,
  //         username: registry.adminUsername,
  //         password: registry.adminPassword
  //       }
  //     },
  //     { parent: this }
  //   );
  //   this.image = myImage;
  //   return myImage;
  // }

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
        tenantId: this.azConfig.clientConfig.tenantId,
      },
      {
        parent: appSvc,
      },
    );
  }
}
