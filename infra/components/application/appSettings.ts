import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS } from "../../components";
import { Monitoring } from "../monitoring/monitoring";

interface IAppSvcParams {
    rg: azure.core.ResourceGroup;
    kv: azure.keyvault.KeyVault;
    acr: azure.containerservice.Registry;
    monitoring: Monitoring;
}

const appsConfig = new pulumi.Config("apps");

const externalServices = appsConfig.requireObject<any>("externalServices"); // todo remove

const stack = pulumi.getStack();

const host = appsConfig.get("mainHost") ? appsConfig.require("mainHost") : "";

export function getK8sAppSettings(kv: azure.keyvault.KeyVault, location: pulumi.Output<string>) {
    return {
        APPINSIGHTS_INSTRUMENTATIONKEY: this.params.monitoring.applicationInsights.instrumentationKey, // important
        DisableHsts: "",
        Environment__Location: location,
        Environment__Stack: stack,
        ExternalServices__IdentityBaseUrl: externalServices.identityBaseUrl,
        ExternalServices__WebAppBaseUrl: externalServices.webAppBaseUrl,
        Host__MainHost: host, // important
        Logging__ApplicationInsights__LogLevel__Default: "Warning",
        STACK: stack,
        WEBSITES_PORT: "80",
        kvStorageCSSecretName: CONSTANTS.AzStorage_KV_CS_SecretName, // important
        kvUri: kv.vaultUri, // important
    };
}
