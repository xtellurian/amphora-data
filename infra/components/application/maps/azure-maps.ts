import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

// tslint:disable-next-line: no-empty-interface
export interface IAzureMapsParams {
    rg: azure.core.ResourceGroup;
}

const tags = {
    component: "application",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
    subcomponent: "maps",
};

export class AzureMaps extends pulumi.ComponentResource {
    public maps: azure.maps.Account;
    /**
     *
     */
    constructor(
        name: string,
        params: IAzureMapsParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:AzureMaps", name, {}, opts);

        this.create(params.rg);
    }

    private create(rg: azure.core.ResourceGroup) {

        const maps = new azure.maps.Account("maps", {
            resourceGroupName: rg.name,
            skuName: "s0",
            tags,
        },
        {
            parent: this,
        });

        this.maps = maps;
    }
}
