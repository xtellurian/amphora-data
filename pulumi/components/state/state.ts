import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import { injectable, inject } from "inversify";
import {
  IComponentParams,
  COMPONENTS,
  COMPONENT_PARAMS,
  CONSTANTS
} from "../../components";
import { IMonitoring } from "../monitoring/monitoring";
import { IAzureConfig } from "../azure-config/azure-config";

const config = new pulumi.Config("state");
const authConfig = new pulumi.Config("authentication");

@injectable()
export class StateParams implements IComponentParams {
  name: string = "d-state-component";
  opts?: pulumi.ComponentResourceOptions | undefined;
}

export interface IState {
  kv: azure.keyvault.KeyVault;
}

@injectable()
export class State extends pulumi.ComponentResource {
  private _kv: azure.keyvault.KeyVault;
  private _storageAccount: azure.storage.Account;
  private _sqlServer: azure.sql.SqlServer;
  private _sql: azure.sql.Database;
  get kv(): azure.keyvault.KeyVault {
    return this._kv;
  }

  constructor(
    @inject(COMPONENT_PARAMS.StateParams) params: IComponentParams,
    @inject(COMPONENTS.Monitoring) private monitoring: IMonitoring,
    @inject("azure-config") private azConfig: IAzureConfig
  ) {
    super("amphora:State", params.name, {}, params.opts);

    this.create();
    this.resolve();
  }

  resolve() {
    console.log("Done creating state");
  }

  create() {
    // Create an Azure Resource Group
    const stateRg = new azure.core.ResourceGroup(
      config.require("rg"),
      {
        location: config.require("location"),
        tags: {
          source: "pulumi"
        }
      },
      { parent: this }
    );

    this._kv = this.keyvault(stateRg);
    this._storageAccount = this.createStorage(stateRg);

    const secret = this.storeInVault(
      CONSTANTS.AzStorage_KV_CS_SecretName,
      this._storageAccount.primaryConnectionString
    );
  }

  storeInVault(name: string, value: pulumi.Input<string> | string) {
    return new azure.keyvault.Secret(
      name,
      {
        value: value,
        keyVaultId: this._kv.id,
        name: name
      },
      { parent: this }
    );
  }

  createStorage(rg: azure.core.ResourceGroup): azure.storage.Account {
    const storage = new azure.storage.Account(
      "state",
      {
        location: config.require("location"),
        resourceGroupName: rg.name,
        accountReplicationType: "LRS",
        accountTier: "Standard",
        accountKind: "StorageV2"
      },
      {
        parent: this
      }
    );

    // code below is hidden behind a feature flag
    // Original Error: autorest/azure: Service returned an error. Status=400 Code="BadRequest" Message="Subscription 'c5760548-23c2-4223-b41e-5d68a8320a0c' is not whitelisted in the private preview of diagnostic log settings for Azure resource type 'microsoft.storage/storageaccounts', feature flag: 'microsoft.insights/diagnosticsettingpreview'."
    // var storageDiag = new azure.monitoring.DiagnosticSetting(
    //   "state-diag",
    //   {
    //     logAnalyticsWorkspaceId: this.monitoring.logAnalyticsWorkspace.id,
    //     logs: [
    //       {
    //         category: "AuditEvent",
    //         enabled: true,
    //         retentionPolicy: {
    //           enabled: true
    //         }
    //       }
    //     ],
    //     metrics: [
    //       {
    //         category: "AllMetrics",
    //         retentionPolicy: {
    //           enabled: true
    //         }
    //       }
    //     ],
    //     name: "example",
    //     targetResourceId: storage.id
    //   },
    //   { parent: this }
    // );

    return storage;
  }

  private keyvault(rg: azure.core.ResourceGroup) {
    const kv = new azure.keyvault.KeyVault(
      "amphoraState",
      {
        accessPolicies: [
          {
            keyPermissions: ["create", "get"],
            objectId: authConfig.require("rian"),
            secretPermissions: ["list", "set", "get", "delete"],
            tenantId: this.azConfig.clientConfig.tenantId
          }
        ],
        resourceGroupName: rg.name,
        tenantId: this.azConfig.clientConfig.tenantId,
        skuName: "standard"
      },
      { parent: this }
    );
    
    
    const kvDiagnostics = new azure.monitoring.DiagnosticSetting(
      "keyVault-Diag",
      {
        logs: [
          {
            category: "AuditEvent",
            enabled: false,
            retentionPolicy: {
              enabled: false
            }
          }
        ],
        metrics: [
          {
            category: "AllMetrics",
            retentionPolicy: {
              enabled: false
            }
          }
        ],
        name: "example",
        logAnalyticsWorkspaceId: this.monitoring.logAnalyticsWorkspace.id,
        targetResourceId: kv.id
      },
      { parent: this }
    );
    return kv;
  }

  linkCosmosToLogAnalytics(
    cosmos: azure.cosmosdb.Account,
    logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace
  ) {
    new azure.monitoring.DiagnosticSetting(
      "cosmos-diag",
      {
        logAnalyticsWorkspaceId: logAnalyticsWorkspace.id,
        targetResourceId: cosmos.id,
        logs: [
          {
            category: "QueryRuntimeStatistics",
            enabled: true,
            retentionPolicy: {
              enabled: true
            }
          },
          {
            category: "DataPlaneRequests",
            enabled: true,
            retentionPolicy: {
              enabled: true
            }
          }
        ],
        metrics: [
          {
            category: "AllMetrics",
            retentionPolicy: {
              enabled: true
            }
          }
        ]
      },
      {
        parent: this
      }
    );
  }
}
