import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "search",
};

const config = new pulumi.Config("azuresearch");

export interface IAzureSearchParams {
    rg: azure.core.ResourceGroup;
}

export class AzureSearch extends pulumi.ComponentResource {
    public service: azure.search.Service;
    constructor(
        name: string,
        params: IAzureSearchParams,
        opts?: pulumi.ComponentResourceOptions) {

        super("amphora:AzureSearch", name, {}, opts);

        this.create(params.rg);
    }

    private create(rg: azure.core.ResourceGroup) {
        const service = new azure.search.Service("service",
            {
                location: rg.location,
                resourceGroupName: rg.name,
                sku: config.get("sku") || "free",
                tags,
            },
            {
                parent: this,
            });
        this.service = service;
    }
}
