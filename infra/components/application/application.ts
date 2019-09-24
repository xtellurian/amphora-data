import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { CONSTANTS, IComponentParams } from "../../components";
import { Monitoring } from "../monitoring/monitoring";
import { Network } from "../network/network";
import { State } from "../state/state";
import { AppSvc } from "./appSvc/appSvc";
import { AzureMaps } from "./maps/azure-maps";
import { AzureSearch } from "./search/azure-search";
import { Tsi } from "./tsi/tsi";

const config = new pulumi.Config("acr");

const tags = {
  component: "application",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};
const rgName = pulumi.getStack() + "-app";
const searchRgName = pulumi.getStack() + "-search";

export interface IApplication {
  appSvc: AppSvc;
}

export class Application extends pulumi.ComponentResource
  implements IApplication {
  public appSvc: AppSvc;
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
        tags,
      },
      {
        parent: this,
      },
    );
    this.acr = this.createAcr(rg);

    this.appSvc = new AppSvc("appSvc", {
      acr: this.acr,
      kv: this.state.kv,
      monitoring: this.monitoring,
      network: this.network,
      rg,
      state: this.state });

    this.createAzureMaps(rg);
    this.createTsi();

    const searchRg = new azure.core.ResourceGroup(searchRgName,
      {
        location: CONSTANTS.location.secondary,
        tags,
      }, { parent: this });

    this.createSearch(searchRg);
  }

  private createSearch(rg: azure.core.ResourceGroup) {
    const search = new AzureSearch("azure-search", {
      rg,
    },
      { parent: this });

    this.state.storeInVault("AzureSearchName", "AzureSearch--Name", search.service.name);
    this.state.storeInVault("AzureSearchPrimaryKey", "AzureSearch--PrimaryKey", search.service.primaryKey);
    this.state.storeInVault("AzureSearchSecondaryKey", "AzureSearch--SecondaryKey", search.service.secondaryKey);
  }

  private createTsi() {
    this.tsi = new Tsi("tsi", {
      appSvc: this.appSvc.appSvc,
      eh: this.state.eh,
      eh_namespace: this.state.ehns,
      state: this.state,
    }, {
      parent: this,
    });
  }

  private createAcr(rg: azure.core.ResourceGroup) {
    let sku = config.get("sku");
    if (!sku) { sku = "Basic"; } // default SKU is basic
    const acr = new azure.containerservice.Registry(
      "acr",
      {
        adminEnabled: true,
        location: CONSTANTS.location.primary,
        resourceGroupName: rg.name,
        sku,
        tags,
      },
      { parent: rg },
    );
    return acr;
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
