import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import * as random from "@pulumi/random";

import { CONSTANTS } from "../../../components";
import { AzureId } from "../../../models/azure-id";
import { State } from "../../state/state";
// import { IAksCollection } from "../application";
import { AppSvc } from "../appSvc/appSvc";
import { accessPolicyTemplate } from "./tsi_accesspolicy";
import { environmentTemplate } from "./tsi_environment";
import { eventSourceTemplate } from "./tsi_eventsource";

const auth = new pulumi.Config("auth");

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
  appSvc: AppSvc;
  state: State;
}

export class Tsi extends pulumi.ComponentResource {
  public dataAccessFqdn: pulumi.Output<any>;
  private envName: random.RandomString;
  constructor(
    private name: string,
    private params: ITsiParams,
    opts?: pulumi.ComponentResourceOptions
  ) {
    super("amphora:Tsi", name, {}, opts);

    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: CONSTANTS.location.primary,
        tags: azTags,
      },
      { parent: this }
    );

    this.envName = new random.RandomString(
      "env_name",
      {
        length: 12,
        lower: true,
        number: false,
        special: false,
      },
      { parent: this }
    );

    const env = new azure.core.TemplateDeployment(
      this.name + "_env",
      {
        deploymentMode: "Incremental",
        parameters: {
          environmentName: this.envName.result,
          location: CONSTANTS.location.primary,
          storageAccountName: this.params.state.storageAccount.name,
          storageAccountResourceId: this.params.state.storageAccount.id,
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(environmentTemplate()),
      },
      { parent: rg }
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
          location: CONSTANTS.location.primary,
          sharedAccessKey: this.params.eh_namespace.defaultPrimaryKey,
          timestampPropertyName: "t",
        },
        resourceGroupName: rg.name,
        templateBody: JSON.stringify(eventSourceTemplate()),
      },
      { parent: rg, dependsOn: env }
    );

    const accessPolicies: azure.core.TemplateDeployment[] = [];

    this.params.appSvc.mainApps.forEach((app) => {
      accessPolicies.push(
        new azure.core.TemplateDeployment(
          app.name + "_ap",
          {
            deploymentMode: "Incremental",
            parameters: {
              accessPolicyReaderObjectId: app.appSvc.identity.apply(
                (identity) =>
                  identity.principalId || "11111111-1111-1111-1111-111111111111"
              ), // https://github.com/pulumi/pulumi-azure/issues/192)
              environmentName: this.envName.result,
              name: app.name,
            },
            resourceGroupName: rg.name,
            templateBody: JSON.stringify(accessPolicyTemplate()),
          },
          { parent: rg, dependsOn: eventSource }
        )
      );
      if (app.appSvcStaging) {
        // if the staging env exists, add it to the access policy
        accessPolicies.push(
          new azure.core.TemplateDeployment(
            app.name + "_apSlot",
            {
              deploymentMode: "Incremental",
              parameters: {
                accessPolicyReaderObjectId: app.appSvcStaging.identity.apply(
                  (identity) =>
                    identity.principalId ||
                    "11111111-1111-1111-1111-111111111111"
                ), // https://github.com/pulumi/pulumi-azure/issues/192)
                environmentName: this.envName.result,
                name: app.name + "staging",
              },
              resourceGroupName: rg.name,
              templateBody: JSON.stringify(accessPolicyTemplate()),
            },
            { parent: rg, dependsOn: eventSource }
          )
        );
      }
      if (pulumi.getStack() !== "prod") {
        // add the build agent
      }
    });

    const identities = auth.requireObject<AzureId[]>("ids");
    identities.forEach((i) => {
      accessPolicies.push(
        new azure.core.TemplateDeployment(
          `${i.name}-access`,
          {
            deploymentMode: "Incremental",
            parameters: {
              accessPolicyReaderObjectId: i.objectId,
              environmentName: this.envName.result,
              name: `${i.name}-access`,
            },
            resourceGroupName: rg.name,
            templateBody: JSON.stringify(accessPolicyTemplate()),
          },
          {
            dependsOn: eventSource,
            parent: rg,
          }
        )
      );
    });

    this.dataAccessFqdn = env.outputs.dataAccessFqdn;

    this.params.state.storeInVault(
      "TsiDataAccessFqdn",
      "Tsi--DataAccessFqdn",
      this.dataAccessFqdn,
      this
    );
  }
}
