import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";

import { environmentTemplate } from "./tsi_environment";
import { eventSourceTemplate } from "./tsi_eventsource";
import { accessPolicyTemplate } from "./tsi_accesspolicy";
import { State } from "../../state/state";

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
      "tsi",
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

    const env = new azure.core.TemplateDeployment(
      this.name + "_env",
      {
        resourceGroupName: rg.name,
        deploymentMode: "Incremental",
        parameters: {
          location: config.require("location"),
          environmentName: this.env_name.result,
          storageAccountName: this._params.state.storageAccount.name,
          storageAccountResourceId: this._params.state.storageAccount.id,
        },
        templateBody: JSON.stringify(environmentTemplate())
      },
      { parent: rg }
    );

    const eventSource = new azure.core.TemplateDeployment(
      this.name + "_eventSrc",
      {
        resourceGroupName: rg.name,
        deploymentMode: "Incremental",
        parameters: {
          location: config.require("location"),
          eventHubNamespaceName: this._params.eh_namespace.name,
          eventHubName: this._params.eh.name,
          eventHubResourceId: this._params.eh.id,
          environmentName: this.env_name.result,
          keyName: "RootManageSharedAccessKey",
          sharedAccessKey: this._params.eh_namespace.defaultPrimaryKey,
          timestampPropertyName: "t"
        },
        templateBody: JSON.stringify(eventSourceTemplate())
      },
      { parent: rg, dependsOn: env}
    );

    const accessPolicy = new azure.core.TemplateDeployment(
      this.name + "_accessPolicy",
      {
        resourceGroupName: rg.name,
        deploymentMode: "Incremental",
        parameters: {
          environmentName: this.env_name.result,
          accessPolicyReaderObjectId: this._params.appSvc.identity.apply(
            identity =>
            identity.principalId || "11111111-1111-1111-1111-111111111111"
          ), // https://github.com/pulumi/pulumi-azure/issues/192)
        },
        templateBody: JSON.stringify(accessPolicyTemplate())
      },
      { parent: rg, dependsOn: eventSource }
    );

    this.dataAccessFqdn = env.outputs["dataAccessFqdn"];

    this._params.state.storeInVault("DataAccessFqdn", this.dataAccessFqdn, this);
  }
}
