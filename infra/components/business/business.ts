import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS, IComponentParams } from "../../components";
import { Monitoring } from "../monitoring/monitoring";
import schemaAsJsonString from "./eventGridWebhookSchema";

const tags = {
    component: "business",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

const rgName = pulumi.getStack() + "-business";

export class Business extends pulumi.ComponentResource {
    public appMonitoringWorkflow: azure.logicapps.Workflow;
    public workflowTrigger: azure.logicapps.TriggerHttpRequest;

    constructor(
        private params: IComponentParams,
        private monitoring: Monitoring,
    ) {
        super("amphora:Business", params.name, {}, params.opts);

        this.create();
    }

    private create() {
        const rg = new azure.core.ResourceGroup(
            rgName,
            {
                location: CONSTANTS.location.primary,
                tags,
            },
            { parent: this },
        );

        this.appMonitoringWorkflow = new azure.logicapps.Workflow("appMonitor", {
            location: rg.location,
            resourceGroupName: rg.name,
        });

        this.workflowTrigger = new azure.logicapps.TriggerHttpRequest("workflowTrigger", {
            logicAppId: this.appMonitoringWorkflow.id,
            method: "POST",
            schema: schemaAsJsonString(),
        });
    }
}
