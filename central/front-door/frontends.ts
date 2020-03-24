import * as azure from "@pulumi/azure";
import { Input } from "@pulumi/pulumi";
import { IFrontendHosts } from "../dns/front-door-dns";

export function getFrontendEndpoints(kv: azure.keyvault.KeyVault, frontendHosts: IFrontendHosts)
    : Input<Array<Input<azure.types.input.frontdoor.FrontdoorFrontendEndpoint>>> {
    const frontends: Input<Array<Input<azure.types.input.frontdoor.FrontdoorFrontendEndpoint>>> = [
        {
            customHttpsProvisioningEnabled: false,
            hostName: "amphora.azurefd.net",
            name: "defaultFrontend",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                azureKeyVaultCertificateSecretName: "static-site",
                azureKeyVaultCertificateSecretVersion: "2cb1416e06e640229d2e28bacb1eb9cd",
                azureKeyVaultCertificateVaultId: kv.id,
                certificateSource: "AzureKeyVault",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "amphoradata.com",
            name: "rootDomain",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "www.amphoradata.com",
            name: "wwwDomain",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "beta.amphoradata.com",
            name: "betaDomain",
            sessionAffinityEnabled: true,
        },
        {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: frontendHosts.prod.app.globalHost,
            name: frontendHosts.prod.app.frontendName,
            sessionAffinityEnabled: true,
        },
        {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: frontendHosts.prod.identity.globalHost,
            name: frontendHosts.prod.identity.frontendName,
            sessionAffinityEnabled: true,
        },
    ];

    return frontends;
}
