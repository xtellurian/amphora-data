import * as azure from "@pulumi/azure";

export interface IAzureConfig {
    clientConfig: azure.core.GetClientConfigResult;
}

export class AzureConfig {
    public clientConfig: azure.core.GetClientConfigResult;

    public async init() {
        const c = await azure.core.getClientConfig();
        this.clientConfig = c;
        // tslint:disable-next-line: no-console
        console.log("servicePrincipalApplicationId: " + c.servicePrincipalApplicationId);
        // tslint:disable-next-line: no-console
        console.log("servicePrincipalObjectId: " + c.servicePrincipalObjectId);
        // tslint:disable-next-line: no-console
        console.log("clientId: " + c.clientId);
    }
}
