import * as azure from "@pulumi/azure";

export interface IAzureConfig {
    clientConfig: azure.core.GetClientConfigResult;
}

export class AzureConfig {
    clientConfig: azure.core.GetClientConfigResult;

    async init() {
        let c = await azure.core.getClientConfig();
        this.clientConfig = c;
    }
}