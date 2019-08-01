import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";

import { getTsiTemplate } from "./tsi_template";

const config = new pulumi.Config("tsi");

export interface TsiParams {
  eh_namespace: azure.eventhub.EventHubNamespace;
  eh: azure.eventhub.EventHub;
}

export class Tsi extends pulumi.ComponentResource {
  constructor(
    private name: string,
    private _params: TsiParams,
    opts?: pulumi.ComponentResourceOptions
  ) {
    super("amphora:Tsi", name, {}, opts);

    this.create();
  }

  create() {
    const rg = new azure.core.ResourceGroup(
      "d-tsi",
      {
        location: config.require("location"),
        tags: {
          source: "pulumi"
        }
      },
      { parent: this }
    );

    const env_name = new random.RandomString(
      "env_name",
      {
        length: 12,
        special: false,
        lower: true,
        number: false,
      },
      { parent: this }
    );

    // var nnn = "aljksdssdfnlsdkgnld"

    const coldStorage = new azure.storage.Account("coldStorage", {
      accountReplicationType: "GRS",
      accountTier: "Standard",
      location: rg.location,
      resourceGroupName: rg.name,
      tags: {
        source: "pulumi",
      },
    }, { parent: this });

    const t = new azure.core.TemplateDeployment(
      this.name + "_template",
      {
        resourceGroupName: rg.name,
        deploymentMode: "Incremental",
        parameters: {
          location: config.require("location"),
          eventHubNamespaceName: this._params.eh_namespace.name,
          eventHubName: this._params.eh.name,
          eventHubResourceId: this._params.eh.id,
          environmentName: env_name.result,
          storageAccountName: coldStorage.name,
          keyName: "RootManageSharedAccessKey",
          sharedAccessKey: this._params.eh_namespace.defaultPrimaryKey
        },
        templateBody: JSON.stringify(getTsiTemplate())
      },
      { parent: this }
    );
  }
}
