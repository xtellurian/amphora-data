import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import * as random from "@pulumi/random";

import { azureMapsTemplate } from "./azure-maps-template";

// tslint:disable-next-line: no-empty-interface
export interface IAzureMapsParams {
    rg: azure.core.ResourceGroup;
}

export class AzureMaps extends pulumi.ComponentResource {
    public mapsName: random.RandomString;
    public clientId: pulumi.Output<any>;
    public resourceId: pulumi.Output<any>;
    public resourceGroup: pulumi.Output<any>;
    public accountName: pulumi.Output<string>;
    /**
     *
     */
    constructor(
        private name: string,
        private params: IAzureMapsParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:AzureMaps", name, {}, opts);

        this.create(params.rg);
    }

    private create(rg: azure.core.ResourceGroup) {

        this.mapsName = new random.RandomString(
            "maps_name",
            {
              length: 12,
              lower: true,
              number: false,
              special: false,
            },
            { parent: this },
          );

        const maps = new azure.core.TemplateDeployment(
            this.name + "_resource",
            {
                deploymentMode: "Incremental",
                parameters: {
                    account_name: this.mapsName.result,
                },
                resourceGroupName: rg.name,
                templateBody: JSON.stringify(azureMapsTemplate()),
            },
            { parent: rg },
        );

        this.clientId = maps.outputs.clientId;
        this.resourceId = maps.outputs.resourceId;
        this.accountName = this.mapsName.result;
        this.resourceGroup = rg.name;
    }
}
