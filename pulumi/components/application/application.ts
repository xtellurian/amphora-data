import * as pulumi from "@pulumi/pulumi";
import * as docker from "@pulumi/docker";
import * as azure from "@pulumi/azure";
import {
  IComponentParams,
  CONSTANTS
} from "../../components";
import { Monitoring } from "../monitoring/monitoring";
import { State } from "../state/state";
import { AzureConfig } from "../azure-config/azure-config";
import { Tsi } from "./tsi/tsi";

const config = new pulumi.Config("application");
const azTags = {
  source: "pulumi",
  component: "application",
  stack: pulumi.getStack(),
  project: pulumi.getProject(),
}
const rgName = pulumi.getStack() + "-app";

// lives in here for now
export class ApplicationParams implements IComponentParams {
  name: string = "d-app-component";
  opts?: pulumi.ComponentResourceOptions | undefined = undefined;
}

export interface IApplication {
  appSvc: azure.appservice.AppService;
  plan: azure.appservice.Plan;
}

export class Application extends pulumi.ComponentResource
  implements IApplication {
  private _appSvc: azure.appservice.AppService;
  private _acr: azure.containerservice.Registry;
  private _plan: azure.appservice.Plan;
  tsi: Tsi;
  get appSvc(): azure.appservice.AppService {
    return this._appSvc;
  }
  get plan(): azure.appservice.Plan {
    return this._plan;
  }

  constructor(
    params: IComponentParams,
    private _monitoring: Monitoring,
    private _state: State,
    private _azConfig: AzureConfig
  ) {
    super("amphora:Application", params.name, {}, params.opts);
    if (_state.kv == null) {
      console.log("WTF");
    }
    this.create();
  }

  create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: config.require("location"),
        tags: azTags
      },
      {
        parent: this
      }
    );
    this._acr = this.createAcr(rg);
    const image = this.buildApp(this._acr);
    this.createAppSvc(rg, this._state.kv, image);
    this.accessPolicyKeyVault(this._state.kv, this._appSvc);
    this.createTsi();
  }
  createTsi() {
    this.tsi = new Tsi("tsi", {
      eh_namespace: this._state.ehns, 
      eh: this._state.eh,
      appSvc: this.appSvc,
      state: this._state
    }, {
        parent: this
      });
  }

  createAcr(rg: azure.core.ResourceGroup) {
    const acr = new azure.containerservice.Registry(
      "acr",
      {
        location: config.require("location"),
        resourceGroupName: rg.name,
        adminEnabled: true,
        sku: "Standard",
        tags: azTags
      },
      { parent: rg }
    );
    return acr;
  }

  private buildApp(registry: azure.containerservice.Registry): docker.Image {
    const customImage = "hello-world";
    const myImage = new docker.Image(
      "acrImage",
      {
        imageName: pulumi.interpolate`${
          registry.loginServer
          }/${customImage}:v1.0.0`,
        build: {
          context: `../apps`
        },
        registry: {
          server: registry.loginServer,
          username: registry.adminUsername,
          password: registry.adminPassword
        }
      },
      { parent: this }
    );
    return myImage;
  }

  private createAppSvc(
    rg: azure.core.ResourceGroup,
    kv: azure.keyvault.KeyVault,
    image: docker.Image
  ) {
    this._plan = new azure.appservice.Plan(
      "appSvcPlan",
      {
        location: rg.location,
        resourceGroupName: rg.name,
        sku: {
          size: config.require("appSvcPlanSize"),
          tier: config.require("appSvcPlanTier")
        },
        kind: "Linux",
        reserved: true,
        tags: azTags
      },
      {
        parent: rg
      }
    );

    this._appSvc = new azure.appservice.AppService(
      "appSvc",
      {
        location: rg.location,
        resourceGroupName: rg.name,
        appServicePlanId: this._plan.id,
        identity: { type: "SystemAssigned" },
        appSettings: {
          kvUri: kv.vaultUri,
          kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName,
          APPINSIGHTS_INSTRUMENTATIONKEY: this._monitoring.appInsights
            .instrumentationKey,
          WEBSITES_ENABLE_APP_SERVICE_STORAGE: "false",
          DOCKER_REGISTRY_SERVER_URL: pulumi.interpolate`https://${
            this._acr.loginServer
            }`,
          DOCKER_REGISTRY_SERVER_USERNAME: this._acr.adminUsername,
          DOCKER_REGISTRY_SERVER_PASSWORD: this._acr.adminPassword,
          WEBSITES_PORT: 80
        },
        siteConfig: {
          alwaysOn: true,
          linuxFxVersion: pulumi.interpolate`DOCKER|${image.imageName}`
        },
        httpsOnly: true,
        tags: azTags
      },
      {
        parent: this._plan
      }
    );
  }

  private accessPolicyKeyVault(
    kv: azure.keyvault.KeyVault,
    appSvc: azure.appservice.AppService
  ) {
    return new azure.keyvault.AccessPolicy(
      "appSvc-access",
      {
        keyVaultId: kv.id,
        secretPermissions: ["get", "list"],
        objectId: appSvc.identity.apply(
          identity =>
            identity.principalId || "11111111-1111-1111-1111-111111111111"
        ), // https://github.com/pulumi/pulumi-azure/issues/192
        tenantId: this._azConfig.clientConfig.tenantId,
      },
      {
        parent: appSvc
      }
    );
  }
}
