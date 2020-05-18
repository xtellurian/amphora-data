import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import * as random from "@pulumi/random";
import { CONSTANTS, IComponentParams } from "../../components";

import { AzureId } from "../../models/azure-id";
import { Monitoring } from "../monitoring/monitoring";

const stack = pulumi.getStack();
const config = new pulumi.Config();
const auth = new pulumi.Config("auth");

const tags = {
  component: "state",
  project: pulumi.getProject(),
  source: "pulumi",
  stack,
};
const rgName = stack + "-state";

export class State extends pulumi.ComponentResource {
  public eh: azure.eventhub.EventHub;
  public ehns: azure.eventhub.EventHubNamespace;
  public kv: azure.keyvault.KeyVault;
  public storageAccount: azure.storage.Account;
  public cosmosDb: azure.cosmosdb.Account;
  public kvAccessPolicies: azure.keyvault.AccessPolicy[] = [];

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
        tags,
        value,
      },
      {
        dependsOn: this.kvAccessPolicies,
        parent: parent || this.kv,
      },
    );
  }

  private create() {
    // Create an Azure Resource Group
    const stateRg = new azure.core.ResourceGroup(
      rgName,
      {
        location: CONSTANTS.location.primary,
        tags,
      },
      { parent: this },
    );

    this.kv = this.keyvault(stateRg);
    this.storageAccount = this.createStorage(stateRg);
    const secret = this.storeInVault(
      CONSTANTS.AzStorage_KV_CS_SecretName,
      `Storage--${CONSTANTS.AzStorage_KV_CS_SecretName}`,
      this.storageAccount.primaryConnectionString,
    );
    const gitHubTokenSecret = this.storeInVault(
      "gitHubToken",
      "GitHubOptions--Token",
      pulumi.interpolate`${config.getSecret("gitHubToken")}`,
    );
    const appTopicPrimaryKey = this.storeInVault(
      "appTopicPrimaryKey",
      "EventGrid--AppTopic--PrimaryKey",
      this.monitoring.appTopic.primaryAccessKey,
      this.monitoring.appTopic,
    );
    const appTopicSecondaryKey = this.storeInVault(
      "appTopicSecondaryKey",
      "EventGrid--AppTopic--SecondaryKey",
      this.monitoring.appTopic.secondaryAccessKey,
      this.monitoring.appTopic,
    );
    const appTopicEndpoint = this.storeInVault(
      "appTopicEndpoint",
      "EventGrid--AppTopic--Endpoint",
      this.monitoring.appTopic.endpoint,
      this.monitoring.appTopic,
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
        location: CONSTANTS.location.primary,
        resourceGroupName: rg.name,
        tags,
      },
      {
        parent: rg,
      },
    );

    return storage;
  }

  private keyvault(rg: azure.core.ResourceGroup) {
    // these are the build identities + Rian
    const identities = auth.requireObject<AzureId[]>("ids");

    // don't add access policy directly here, I think it conflicts with those generated independently.
    const kv = new azure.keyvault.KeyVault(
      "amphoraState",
      {
        resourceGroupName: rg.name,
        skuName: "standard",
        tags,
        tenantId: CONSTANTS.authentication.tenantId,
      },
      { parent: rg },
    );

    identities.forEach((element) => {
      if (element.appId) {
        this.kvAccessPolicies.push(new azure.keyvault.AccessPolicy(
          element.name + "app",
          {
            applicationId: element.appId,
            certificatePermissions: ["get", "create", "list", "update", "delete"],
            keyPermissions: ["create", "get", "list", "delete", "unwrapKey", "wrapKey"],
            keyVaultId: kv.id,
            objectId: element.objectId,
            secretPermissions: ["list", "set", "get", "delete"],
            tenantId: CONSTANTS.authentication.tenantId,
          },
          {
            dependsOn: kv,
            parent: this,
          },
        ));
      }
      this.kvAccessPolicies.push(new azure.keyvault.AccessPolicy(
        element.name + "obj",
        {
          certificatePermissions: ["get", "create", "list", "update", "delete"],
          keyPermissions: ["create", "get", "list", "delete", "unwrapKey", "wrapKey"],
          keyVaultId: kv.id,
          objectId: element.objectId,
          secretPermissions: ["list", "set", "get", "delete"],
          tenantId: CONSTANTS.authentication.tenantId,
        },
        {
          dependsOn: kv,
          parent: this,
        },
      ));
    });

    const dataProtectionKey = new azure.keyvault.Key("dataprotectionkey",
      {
        keyOpts: [
          "decrypt",
          "encrypt",
          "sign",
          "unwrapKey",
          "verify",
          "wrapKey",
        ],
        keySize: 4096,
        keyType: "RSA",
        keyVaultId: kv.id,
        name: "dataprotection",
      },
      {
        dependsOn: this.kvAccessPolicies,
        parent: this,
      });

    const identitySigningCertificate = new azure.keyvault.Certificate("identityCert",
      {
        certificatePolicy: {
          issuerParameters: {
            name: "Self",
          },
          keyProperties: {
            exportable: true,
            keySize: 2048,
            keyType: "RSA",
            reuseKey: true,
          }, lifetimeActions: [
            {
              action: {
                actionType: "AutoRenew",
              },
              trigger: {
                daysBeforeExpiry: 30,
              },
            },
          ],
          secretProperties: {
            contentType: "application/x-pkcs12",
          },
          x509CertificateProperties: {
            extendedKeyUsages: ["1.3.6.1.5.5.7.3.1"],
            keyUsages: [
              "cRLSign",
              "dataEncipherment",
              "digitalSignature",
              "keyAgreement",
              "keyCertSign",
              "keyEncipherment",
            ],
            subject: `CN=${stack}-amphoradata`,
            subjectAlternativeNames: {
              dnsNames: [
                "localhost",
                `${stack}.identity.amphoradata.com`,
              ],
            },
            validityInMonths: 12,
          },
        },
        keyVaultId: kv.id,
        name: "identityCert",
      }, {
      dependsOn: this.kvAccessPolicies,
      parent: this,
    });

    // this.kvDiagnostics(kv, rg);
    return kv;
  }

  private kvDiagnostics(kv: azure.keyvault.KeyVault, rg: azure.core.ResourceGroup) {
    const kvDiagnostics = new azure.monitoring.DiagnosticSetting("keyVault-Diag", {
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
    }, { parent: rg });
  }

  private createEventHubs(rg: azure.core.ResourceGroup) {
    this.ehns = new azure.eventhub.EventHubNamespace(
      "amphoradhns",
      {
        autoInflateEnabled: true,
        location: rg.location,
        maximumThroughputUnits: 3,
        resourceGroupName: rg.name,
        sku: "Standard",
        tags,
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

    const ehAuthRule = new azure.eventhub.AuthorizationRule("ehAuthRule", {
      eventhubName: this.eh.name,
      listen: true,
      manage: false,
      namespaceName: this.ehns.name,
      resourceGroupName: rg.name,
      send: true,
    }, {
      parent: this.eh,
    });

    this.storeInVault("TsiEventHubConnectionString", "TsiEventHub--ConnectionString",
      ehAuthRule.primaryConnectionString);
    this.storeInVault("TsiEventHubName", "TsiEventHub--Name", this.eh.name);

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
    },
      {
      });

    const dbNamePrefix = new random.RandomString(
      "db_name_prefix",
      {
        length: 4,
        lower: true,
        number: false,
        special: false,
      },
      { parent: this.cosmosDb },
    );

    const dbA = new azure.cosmosdb.SqlDatabase("cosmosSql_A", {
      accountName: this.cosmosDb.name,
      // TODO: set this name as -A
      resourceGroupName: rg.name,
    },
      {
        aliases: [
          `urn:pulumi:${pulumi.getStack()}::${pulumi.getProject()}::azure:cosmosdb/sqlDatabase:SqlDatabase::cosmosSql`,
          { name: "cosmosSql", parent: this.cosmosDb }],
        parent: this.cosmosDb,
      });

    // const dbB = new azure.cosmosdb.SqlDatabase("cosmosSql_B", {
    //   accountName: this.cosmosDb.name,
    //   name: pulumi.interpolate`${dbNamePrefix.result}-B`,
    //   resourceGroupName: rg.name,
    // },
    //   {
    //     parent: this.cosmosDb,
    //   });

    this.storeInVault("cosmosAccountPrimaryKey", "Cosmos--PrimaryKey", this.cosmosDb.primaryMasterKey);
    this.storeInVault("cosmosAccountSecondaryKey", "Cosmos--SecondaryKey", this.cosmosDb.secondaryMasterKey);
    this.storeInVault("cosmosAccountPrimaryReadonlyKey", "Cosmos--PrimaryReadonlyKey",
      this.cosmosDb.primaryReadonlyMasterKey);
    this.storeInVault("cosmosAccountSecondaryReadonlyKey", "Cosmos--SecondaryReadonlyKey",
      this.cosmosDb.secondaryReadonlyMasterKey);

    this.storeInVault("cosmosAccountEndpoint", "Cosmos--Endpoint", this.cosmosDb.endpoint);
    this.storeInVault("cosmosSqlDbName", "Cosmos--Database", dbA.name);

    // TODO: use A/B databases
    this.storeInVault("cosmosSqlDbAName", "Cosmos--DatabaseA", dbA.name);
    // this.storeInVault("cosmosSqlDbBName", "Cosmos--DatabaseB", dbB.name);

    this.storeInVault("SendGridApiKey", "SendGrid--ApiKey", config.require("SendGridApiKey"));

    // this.linkCosmosToLogAnalytics(this.cosmosDb, this.monitoring.logAnalyticsWorkspace);
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
            category: "DataPlaneRequests",
            enabled: true,
            retentionPolicy: {
              days: 0,
              enabled: true,
            },
          },
          {
            category: "CassandraRequests",
            enabled: false,
            retentionPolicy: {
              days: 0,
              enabled: false,
            },
          },
          {
            category: "QueryRuntimeStatistics",
            enabled: true,
            retentionPolicy: {
              days: 0,
              enabled: true,
            },
          },
          {
            category: "PartitionKeyStatistics",
            enabled: false,
            retentionPolicy: {
              days: 0,
              enabled: false,
            },
          },
          {
            category: "ControlPlaneRequests",
            enabled: false,
            retentionPolicy: {
              days: 0,
              enabled: false,
            },
          },
          {
            category: "MongoRequests",
            enabled: false,
            retentionPolicy: {
              days: 0,
              enabled: false,
            },
          },
          {
            category: "PartitionKeyRUConsumption",
            enabled: false,
            retentionPolicy: {
              days: 0,
              enabled: false,
            },
          },
        ],
        metrics: [
          {
            category: "Requests",
            retentionPolicy: {
              days: 0,
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
