import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import {
  IComponentParams,
  CONSTANTS
} from "../../components";

import { IAzureConfig } from "../azure-config/azure-config";
import { Monitoring } from "../monitoring/monitoring";

const config = new pulumi.Config("state");
const authConfig = new pulumi.Config("authentication");
const azTags = {
  source: "pulumi",
  component: "state",
  stack: pulumi.getStack(),
  project: pulumi.getProject(),
}
const rgName = pulumi.getStack() + "-state";

export class StateParams implements IComponentParams {
  name: string = "d-state-component";
  opts?: pulumi.ComponentResourceOptions | undefined;
}

export class State extends pulumi.ComponentResource {
  private _kv: azure.keyvault.KeyVault;
  storageAccount: azure.storage.Account;
  ehns: azure.eventhub.EventHubNamespace;
  eh: azure.eventhub.EventHub;
  get kv(): azure.keyvault.KeyVault {
    return this._kv;
  }

  constructor(
    params: IComponentParams,
    private monitoring: Monitoring,
    private azConfig: IAzureConfig
  ) {
    super("amphora:State", params.name, {}, params.opts);

    this.create();
  }

  create() {
    // Create an Azure Resource Group
    const stateRg = new azure.core.ResourceGroup(
      rgName,
      {
        location: config.require("location"),
        tags: azTags
      },
      { parent: this }
    );

    this._kv = this.keyvault(stateRg);
    this.storageAccount = this.createStorage(stateRg);
    const secret = this.storeInVault(
      CONSTANTS.AzStorage_KV_CS_SecretName,
      this.storageAccount.primaryConnectionString
    );
    this.createEventHubs(stateRg);
  }

  storeInVault(name: string, value: pulumi.Input<string> | string, parent?: pulumi.Resource ) {
    return new azure.keyvault.Secret(
      name,
      {
        value: value,
        keyVaultId: this._kv.id,
        name: name,
        tags: azTags
      },
      { parent: parent || this.kv }
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
        accountKind: "StorageV2",
        enableHttpsTrafficOnly: true,
        tags: azTags
      },
      {
        parent: rg
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
        skuName: "standard",
        tags: azTags
      },
      { parent: rg }
    );

    if( this.azConfig.clientConfig.servicePrincipalObjectId )
    {
      // there needs to be 2 here, because pulumi and dotnet do it differently... 
      const spAccess = new azure.keyvault.AccessPolicy("sp-access", 
      {
        keyVaultId: kv.id,
        applicationId: this.azConfig.clientConfig.servicePrincipalApplicationId,
        objectId: this.azConfig.clientConfig.servicePrincipalObjectId,
        tenantId: this.azConfig.clientConfig.tenantId,
        keyPermissions: ["create", "get"],
        secretPermissions: ["list", "set", "get", "delete"]
      })
      const spAccess_objectId = new azure.keyvault.AccessPolicy("sp-access_objectId", 
      {
        keyVaultId: kv.id,
        objectId: this.azConfig.clientConfig.servicePrincipalObjectId,
        tenantId: this.azConfig.clientConfig.tenantId,
        keyPermissions: ["create", "get"],
        secretPermissions: ["list", "set", "get", "delete"]
      })
    }

    

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
        targetResourceId: kv.id,
      },
      { parent: rg }
    );
    return kv;
  }


  private createEventHubs(rg: azure.core.ResourceGroup) {
    this.ehns = new azure.eventhub.EventHubNamespace(
      "amphoradhns",
      {
        capacity: 1,
        resourceGroupName: rg.name,
        location: rg.location,
        sku: "Standard",
        tags: azTags
      },
      {
        parent: rg
      }
    );

    this.eh = new azure.eventhub.EventHub(
      "eventHub",
      {
        resourceGroupName: rg.name,
        messageRetention: 1,
        namespaceName: this.ehns.name,
        partitionCount: 4,
      },
      {
        parent: this.ehns
      }
    );

    const ehAuthRule = new azure.eventhub.EventHubAuthorizationRule("ehAuthRule", {
      eventhubName: this.eh.name,
      listen: true,
      manage: false,
      namespaceName: this.ehns.name,
      resourceGroupName: rg.name,
      send: true,
    }, {
        parent: this.eh
      });

    this.storeInVault("EventHubConnectionString", ehAuthRule.primaryConnectionString);
    this.storeInVault("EventHubName", this.eh.name);

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
        ],
      },
      {
        parent: this
      }
    );
  }
}
