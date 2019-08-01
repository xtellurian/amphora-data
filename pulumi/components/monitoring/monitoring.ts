import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import { injectable, inject } from "inversify";
import { IComponentParams, COMPONENT_PARAMS } from "../../components";

import { getDashboardTemplate } from "./dashboards-arm";

const config = new pulumi.Config("monitoring");

export interface IMonitoring {
  logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace;
  appInsights: azure.appinsights.Insights;
}

@injectable()
export class MonitoringParams implements IComponentParams {
  name: string = "d-monitoring-component";
  opts?: pulumi.ComponentResourceOptions | undefined;
}

@injectable()
export class Monitoring extends pulumi.ComponentResource
  implements IMonitoring {
  private _logAnalyticsWorkspace: azure.operationalinsights.AnalyticsWorkspace;
  private _applicationInsights: azure.appinsights.Insights;


  get logAnalyticsWorkspace(): azure.operationalinsights.AnalyticsWorkspace {
    return this._logAnalyticsWorkspace;
  }
  get appInsights(): azure.appinsights.Insights {
    return this._applicationInsights;
  }

  constructor(
    @inject(COMPONENT_PARAMS.MonitoringParams) params: IComponentParams
  ) {
    super("amphora:Monitoring", params.name, {}, params.opts);

    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      config.require("rg"),
      { tags: { source: "pulumi" }, location: config.require("location") },
      { parent: this }
    );
    this._logAnalyticsWorkspace = new azure.operationalinsights.AnalyticsWorkspace(
      "logAnalytics",
      {
        resourceGroupName: rg.name,
        sku: "PerGB2018"
      },
      { parent: this }
    );

    this._applicationInsights = new azure.appinsights.Insights(
      "appInsights",
      {
        applicationType: "web",
        location: "AustraliaEast", // only supported region in AUS
        resourceGroupName: rg.name
      },
      { parent: this }
    );

    // new azure.core.TemplateDeployment("dashboard", {
    //     resourceGroupName: rg.name,
    //     deploymentMode: "Incremental",
    //     parameters: {thing: "thing"},
    //     templateBody: JSON.stringify(getDashboardTemplate())
    // })
  }
}
