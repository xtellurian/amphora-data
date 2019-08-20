import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import * as random from "@pulumi/random";

import { State } from "../../state/state";
import { accessPolicyTemplate } from "./tsi_accesspolicy";
import { environmentTemplate } from "./tsi_environment";
import { eventSourceTemplate } from "./tsi_eventsource";

const config = new pulumi.Config("tsi");
const azTags = {
  component: "state",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};
const rgName = pulumi.getStack() + "-tsi";

export interface ITsiParams {
  eh_namespace: azure.eventhub.EventHubNamespace;
  eh: azure.eventhub.EventHub;
  appSvc: azure.appservice.AppService;
  state: State;
}

export class Tsi extends pulumi.ComponentResource {
  public dataAccessFqdn: pulumi.Output<any>;
  private envName: random.RandomString;
  constructor(
    private name: string,
    private params: ITsiParams,
    opts?: pulumi.ComponentResourceOptions,
  ) {
    super("amphora:Tsi", name, {}, opts);

    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: config.require("location"),
        tags: azTags,
      },
      { parent: this },
    );

    this.envName = new random.RandomString(
      "env_name",
      {
        length: 12,
        lower: true,
        number: false,
        special: false,
      },
      { parent: this },
    );

    const env = new azure.core.TemplateDeployment(
      this.name + "_env",
      {
        deploymentMode: "Incremental",
        parameters: {
          environmentName: this.envName.result,
          location: config.require("location"),
          storageAccountName: this.params.state.storageAccount.name,
          storageAccountResourceId: this.params.state.storageAccount.id,
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(environmentTemplate()),
      },
      { parent: rg },
    );

    const eventSource = new azure.core.TemplateDeployment(
      this.name + "_eventSrc",
      {
        deploymentMode: "Incremental",
        parameters: {
          environmentName: this.envName.result,
          eventHubName: this.params.eh.name,
          eventHubNamespaceName: this.params.eh_namespace.name,
          eventHubResourceId: this.params.eh.id,
          keyName: "RootManageSharedAccessKey",
          location: config.require("location"),
          sharedAccessKey: this.params.eh_namespace.defaultPrimaryKey,
          timestampPropertyName: "t",
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(eventSourceTemplate()),
      },
      { parent: rg, dependsOn: env },
    );

    const accessPolicy = new azure.core.TemplateDeployment(
      this.name + "_accessPolicy",
      {
        deploymentMode: "Incremental",
        parameters: {
          accessPolicyReaderObjectId: this.params.appSvc.identity.apply(
            (identity) =>
              identity.principalId || "11111111-1111-1111-1111-111111111111",
          ), // https://github.com/pulumi/pulumi-azure/issues/192)
          environmentName: this.envName.result,
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(accessPolicyTemplate()),
      },
      { parent: rg, dependsOn: eventSource },
    );

    this.dataAccessFqdn = env.outputs.dataAccessFqdn;

    this.params.state.storeInVault("DataAccessFqdn", this.dataAccessFqdn, this);
  }
}
