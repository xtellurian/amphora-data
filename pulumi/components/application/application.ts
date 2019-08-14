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

// lives in here for now
export class ApplicationParams implements IComponentParams {
  name: string = "d-app-component";
  opts?: pulumi.ComponentResourceOptions | undefined = undefined;
}

interface EventHubCollection {
  namespace: azure.eventhub.EventHubNamespace;
  hubs: azure.eventhub.EventHub[];
}
export interface IApplication {
  appSvc: azure.appservice.AppService;
  plan: azure.appservice.Plan;
  eventHubCollections: EventHubCollection[];
}

export class Application extends pulumi.ComponentResource
  implements IApplication {
  private _appSvc: azure.appservice.AppService;
  private _acr: azure.containerservice.Registry;
  private _eventHubCollections: EventHubCollection[] = [];
  private _plan: azure.appservice.Plan;
  tsi: Tsi;
  get appSvc(): azure.appservice.AppService {
    return this._appSvc;
  }
  get eventHubCollections(): EventHubCollection[] {
    return this._eventHubCollections;
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
    this.resolve();
  }

  resolve() {
    this.registerOutputs({
      outputyy: "Done creating Application"
    });
  }
  create() {
    const rg = new azure.core.ResourceGroup(
      config.require("rg"),
      {
        location: config.require("location"),
        tags: {
          source: "pulumi"
        }
      },
      {
        parent: this
      }
    );
    this._acr = this.createAcr(rg);
    const image = this.buildApp(this._acr);
    this.createAppSvc(rg, this._state.kv, image);
    this.createEventHubs(rg);
    this.accessPolicyKeyVault(this._state.kv, this._appSvc);
    this.createTsi();
  }
  createTsi() {
    this.tsi = new Tsi("testtsi", {
      eh_namespace: this.eventHubCollections[0].namespace, // very fragile code
      eh: this.eventHubCollections[0].hubs[0],
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
        sku: "Standard"
      },
      { parent: this }
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
        reserved: true
      },
      {
        parent: this
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
        httpsOnly: true
      },
      {
        parent: this
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
        tenantId: this._azConfig.clientConfig.tenantId
      },
      {
        parent: this
      }
    );
  }

  private createEventHubs(rg: azure.core.ResourceGroup) {
    const ehns = new azure.eventhub.EventHubNamespace(
      "amphoradhns",
      {
        capacity: 1,
        resourceGroupName: rg.name,
        location: rg.location,
        sku: "Standard",
        tags: {
          environment: "Development"
        }
      },
      {
        parent: this
      }
    );

    const eh = new azure.eventhub.EventHub(
      "eventHub",
      {
        resourceGroupName: rg.name,
        messageRetention: 1,
        namespaceName: ehns.name,
        partitionCount: 4
      },
      {
        parent: this
      }
    );

    const ehAuthRule = new azure.eventhub.EventHubAuthorizationRule("ehAuthRule", {
      eventhubName: eh.name,
      listen: true,
      manage: false,
      namespaceName: ehns.name,
      resourceGroupName: rg.name,
      send: true,
    }, {
        parent: this
      });

    this.storeInVault("EventHubConnectionString", ehAuthRule.primaryConnectionString);
    this.storeInVault("EventHubName", eh.name);

    this._eventHubCollections.push({
      namespace: ehns,
      hubs: [eh]
    });
  }

  storeInVault(name: string, value: pulumi.Input<string> | string) {
    return new azure.keyvault.Secret(
      name,
      {
        value: value,
        keyVaultId: this._state.kv.id,
        name: name
      },
      { parent: this }
    );
  }
}
