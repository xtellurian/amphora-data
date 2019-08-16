import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import { IComponentParams, COMPONENT_PARAMS } from "../../components";

import { getDashboardTemplate } from "./dashboards-arm";
const azTags = {
  source: "pulumi",
  component: "monitoring",
  stack: pulumi.getStack(),
  project: pulumi.getProject(),
}

const config = new pulumi.Config("monitoring");
const rgName = pulumi.getStack() + "-monitor";

export class MonitoringParams implements IComponentParams {
  name: string = "d-monitoring-component";
  opts?: pulumi.ComponentResourceOptions | undefined;
}

export class Monitoring extends pulumi.ComponentResource {
  private _logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace;
  private _applicationInsights: azure.appinsights.Insights;


  get logAnalyticsWorkspace(): azure.operationalinsights.AnalyticsWorkspace {
    return this._logAnalyticsWorkspace;
  }
  get appInsights(): azure.appinsights.Insights {
    return this._applicationInsights;
  }

  constructor(
    params: IComponentParams
  ) {
    super("amphora:Monitoring", params.name, {}, params.opts);

    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      { 
        tags: azTags, 
        location: config.require("location") 
      },
      { parent: this }
    );
    this._logAnalyticsWorkspace = new azure.operationalinsights.AnalyticsWorkspace(
      "logAnalytics",
      {
        resourceGroupName: rg.name,
        sku: "PerGB2018",
        tags: azTags
      },
      { parent: rg }
    );

    this._applicationInsights = new azure.appinsights.Insights(
      "appInsights",
      {
        tags: azTags,
        applicationType: "web",
        location: "AustraliaEast", // only supported region in AUS
        resourceGroupName: rg.name
      },
      { parent: rg }
    );

    // new azure.core.TemplateDeployment("dashboard", {
    //     resourceGroupName: rg.name,
    //     deploymentMode: "Incremental",
    //     parameters: {thing: "thing"},
    //     templateBody: JSON.stringify(getDashboardTemplate())
    // })
  }
}
