import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";

import { getTsiTemplate } from "./tsi_template";
import { State } from "../state/state";

const config = new pulumi.Config("tsi");

export interface TsiParams {
  eh_namespace: azure.eventhub.EventHubNamespace;
  eh: azure.eventhub.EventHub;
  appSvc: azure.appservice.AppService;
  state: State;
}

export interface ITsi {
  env_name: random.RandomString;
  dataAccessFqdn: pulumi.Output<any>;
}

export class Tsi extends pulumi.ComponentResource implements ITsi {
  env_name: random.RandomString;
  template: azure.core.TemplateDeployment;
  dataAccessFqdn: pulumi.Output<any>;
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

    this.env_name = new random.RandomString(
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

    this.template = new azure.core.TemplateDeployment(
      this.name + "_template",
      {
        resourceGroupName: rg.name,
        deploymentMode: "Incremental",
        parameters: {
          location: config.require("location"),
          eventHubNamespaceName: this._params.eh_namespace.name,
          eventHubName: this._params.eh.name,
          eventHubResourceId: this._params.eh.id,
          environmentName: this.env_name.result,
          storageAccountName: coldStorage.name,
          keyName: "RootManageSharedAccessKey",
          sharedAccessKey: this._params.eh_namespace.defaultPrimaryKey,
          accessPolicyReaderObjectId: this._params.appSvc.identity.apply(
            identity =>
            identity.principalId || "11111111-1111-1111-1111-111111111111"
          ), // https://github.com/pulumi/pulumi-azure/issues/192)
        },
        templateBody: JSON.stringify(getTsiTemplate())
      },
      { parent: this }
    );

    this.dataAccessFqdn = this.template.outputs["dataAccessFqdn"];

    this._params.state.storeInVault("DataAccessFqdn", this.dataAccessFqdn, this);
  }
}
