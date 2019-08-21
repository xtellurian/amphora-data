import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IComponentParams } from "../../components";

import { getDashboardTemplate } from "./dashboards-arm";
const azTags = {
  component: "monitoring",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};

const config = new pulumi.Config("monitoring");
const rgName = pulumi.getStack() + "-monitor";

export class Monitoring extends pulumi.ComponentResource {
  public logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace;
  public applicationInsights: azure.appinsights.Insights;

  constructor(
    params: IComponentParams,
  ) {
    super("amphora:Monitoring", params.name, {}, params.opts);

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

    this.logAnalyticsWorkspace = new azure.operationalinsights.AnalyticsWorkspace(
      "logAnalytics",
      {
        resourceGroupName: rg.name,
        sku: "PerGB2018",
        tags: azTags,
      },
      {
        dependsOn: rg,
        parent: rg,
      },
    );

    this.applicationInsights = new azure.appinsights.Insights(
      "appInsights",
      {
        applicationType: "web",
        location: "AustraliaEast", // only supported region in AUS
        resourceGroupName: rg.name,
        tags: azTags,
      },
      {
        dependsOn: rg,
        parent: rg,
      },
    );

    // new azure.core.TemplateDeployment("dashboard", {
    //     resourceGroupName: rg.name,
    //     deploymentMode: "Incremental",
    //     parameters: {thing: "thing"},
    //     templateBody: JSON.stringify(getDashboardTemplate())
    // })
  }
}
