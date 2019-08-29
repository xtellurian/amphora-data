import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS, IComponentParams } from "../../components";

import { Monitoring } from "../monitoring/monitoring";

const config = new pulumi.Config("state");
const authConfig = new pulumi.Config("authentication");
const azTags = {
  component: "state",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};
const rgName = pulumi.getStack() + "-state";

export class State extends pulumi.ComponentResource {
  public eh: azure.eventhub.EventHub;
  public ehns: azure.eventhub.EventHubNamespace;
  public kv: azure.keyvault.KeyVault;
  public storageAccount: azure.storage.Account;
  public cosmosDb: azure.cosmosdb.Account;

  constructor(
    params: IComponentParams,
    private monitoring: Monitoring,
  ) {
    super("amphora:State", params.name, {}, params.opts);

    this.create();
  }

  public storeInVault(name: string, key: string, value: pulumi.Input<string> | string, parent?: pulumi.Resource) {
    return new azure.keyvault.Secret(
      name,
      {
        keyVaultId: this.kv.id,
        name: key,
        tags: azTags,
        value,
      },
      {
        parent: parent || this.kv,
      },
    );
  }

  private create() {
    // Create an Azure Resource Group
    const stateRg = new azure.core.ResourceGroup(
      rgName,
      {
        location: config.require("location"),
        tags: azTags,
      },
      { parent: this },
    );

    this.kv = this.keyvault(stateRg);
    this.storageAccount = this.createStorage(stateRg);
    const secret = this.storeInVault(
      CONSTANTS.AzStorage_KV_CS_SecretName,
      CONSTANTS.AzStorage_KV_CS_SecretName,
      this.storageAccount.primaryConnectionString,
    );
    this.createEventHubs(stateRg);
    this.createCosmosDb(stateRg);
  }

  private createStorage(rg: azure.core.ResourceGroup): azure.storage.Account {
    const storage = new azure.storage.Account(
      "state",
      {
        accountKind: "StorageV2",
        accountReplicationType: "LRS",
        accountTier: "Standard",
        enableHttpsTrafficOnly: true,
        location: config.require("location"),
        resourceGroupName: rg.name,
        tags: azTags,
      },
      {
        parent: rg,
      },
    );

    // code below is hidden behind a feature flag
    // tslint:disable-next-line: max-line-length
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
            tenantId: authConfig.require("tenantId"),
          },
          {
            applicationId: authConfig.require("spAppId"),
            keyPermissions: ["create", "get"],
            objectId: authConfig.require("spObjectId"),
            secretPermissions: ["list", "set", "get", "delete"],
            tenantId: authConfig.require("tenantId"),
          },
          {
            keyPermissions: ["create", "get"],
            objectId: authConfig.require("spObjectId"),
            secretPermissions: ["list", "set", "get", "delete"],
            tenantId: authConfig.require("tenantId"),
          },

        ],
        resourceGroupName: rg.name,
        skuName: "standard",
        tags: azTags,
        tenantId: authConfig.require("tenantId"),
      },
      { parent: rg },
    );

    const kvDiagnostics = new azure.monitoring.DiagnosticSetting(
      "keyVault-Diag",
      {
        logAnalyticsWorkspaceId: this.monitoring.logAnalyticsWorkspace.id,
        logs: [
          {
            category: "AuditEvent",
            enabled: false,
            retentionPolicy: {
              enabled: false,
            },
          },
        ],
        metrics: [
          {
            category: "AllMetrics",
            retentionPolicy: {
              enabled: false,
            },
          },
        ],
        name: "example",
        targetResourceId: kv.id,
      },
      { parent: rg },
    );
    return kv;
  }

  private createEventHubs(rg: azure.core.ResourceGroup) {
    this.ehns = new azure.eventhub.EventHubNamespace(
      "amphoradhns",
      {
        capacity: 1,
        location: rg.location,
        resourceGroupName: rg.name,
        sku: "Standard",
        tags: azTags,
      },
      {
        parent: rg,
      },
    );

    this.eh = new azure.eventhub.EventHub(
      "eventHub",
      {
        messageRetention: 1,
        namespaceName: this.ehns.name,
        partitionCount: 4,
        resourceGroupName: rg.name,
      },
      {
        parent: this.ehns,
      },
    );

    const ehAuthRule = new azure.eventhub.EventHubAuthorizationRule("ehAuthRule", {
      eventhubName: this.eh.name,
      listen: true,
      manage: false,
      namespaceName: this.ehns.name,
      resourceGroupName: rg.name,
      send: true,
    }, {
        parent: this.eh,
      });

    this.storeInVault("EventHubConnectionString", "EventHubConnectionString", ehAuthRule.primaryConnectionString);
    this.storeInVault("EventHubName", "EventHubName", this.eh.name);

  }

  private createCosmosDb(rg: azure.core.ResourceGroup) {
    this.cosmosDb = new azure.cosmosdb.Account("cosmosDb", {
      consistencyPolicy: {
        consistencyLevel: "BoundedStaleness",
        maxIntervalInSeconds: 10,
        maxStalenessPrefix: 200,
      },
      enableAutomaticFailover: true,
      geoLocations: [
        {
          failoverPriority: 0,
          location: rg.location,
        },
      ],
      kind: "GlobalDocumentDB",
      location: rg.location,
      offerType: "Standard",
      resourceGroupName: rg.name,
    });

    const sql = new azure.cosmosdb.SqlDatabase("example", {
      accountName: this.cosmosDb.name,
      resourceGroupName: rg.name,
  });

    this.storeInVault("cosmosAccountKey", "Cosmos--Key", this.cosmosDb.primaryMasterKey);
    this.storeInVault("cosmosAccountEndpoint", "Cosmos--Endpoint", this.cosmosDb.endpoint);
    this.storeInVault("cosmosSqlDbName", "Cosmos--Database", sql.name);

    this.linkCosmosToLogAnalytics(this.cosmosDb, this.monitoring.logAnalyticsWorkspace);
  }

  private linkCosmosToLogAnalytics(
    cosmos: azure.cosmosdb.Account,
    logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace,
  ) {
    const diags = new azure.monitoring.DiagnosticSetting(
      "cosmos-diag",
      {
        logAnalyticsWorkspaceId: logAnalyticsWorkspace.id,
        logs: [
          {
            category: "QueryRuntimeStatistics",
            enabled: true,
            retentionPolicy: {
              enabled: true,
            },
          },
          {
            category: "DataPlaneRequests",
            enabled: true,
            retentionPolicy: {
              enabled: true,
            },
          },
        ],
        metrics: [
          {
            category: "AllMetrics",
            retentionPolicy: {
              enabled: true,
            },
          },
        ],
        targetResourceId: cosmos.id,
      },
      {
        parent: this,
      },
    );
  }
}
