import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
// tslint:disable-next-line: no-duplicate-imports
import { Input } from "@pulumi/pulumi";
import { IFrontendHosts, IUniqueUrl } from "../dns/front-door-dns";

const config = new pulumi.Config();

export function getFrontendEndpoints(
    kv: azure.keyvault.KeyVault,
    frontendHosts: IFrontendHosts
): Input<Array<Input<azure.types.input.frontdoor.FrontdoorFrontendEndpoint>>> {
    const frontends: Input<Array<
        Input<azure.types.input.frontdoor.FrontdoorFrontendEndpoint>
    >> = [
        {
            customHttpsProvisioningEnabled: false,
            hostName: "amphora.azurefd.net",
            name: "defaultFrontend",
            sessionAffinityEnabled: false,
        },
        {
            customHttpsConfiguration: {
                azureKeyVaultCertificateSecretName: "static-site",
                azureKeyVaultCertificateSecretVersion:
                    "2cb1416e06e640229d2e28bacb1eb9cd",
                azureKeyVaultCertificateVaultId: kv.id,
                certificateSource: "AzureKeyVault",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "amphoradata.com",
            name: "rootDomain",
            sessionAffinityEnabled: false,
        },
        {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "docs.amphoradata.com",
            name: "docsDomain",
            sessionAffinityEnabled: false,
        },
    ];

    frontends.push(getFrontend(frontendHosts.prod.app));
    frontends.push(getFrontend(frontendHosts.prod.api));
    frontends.push(getFrontend(frontendHosts.prod.identity));

    if (config.requireBoolean("deployDevelop")) {
        frontends.push(getFrontend(frontendHosts.develop.app));
        frontends.push(getFrontend(frontendHosts.develop.api));
        frontends.push(getFrontend(frontendHosts.develop.identity));
    }

    if (config.requireBoolean("deployMaster")) {
        frontends.push(getFrontend(frontendHosts.master.app));
        frontends.push(getFrontend(frontendHosts.master.api));
        frontends.push(getFrontend(frontendHosts.master.identity));
    }

    return frontends;
}

function getFrontend(
    frontend: IUniqueUrl
): azure.types.input.frontdoor.FrontdoorFrontendEndpoint {
    return {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: frontend.globalHost,
        name: frontend.frontendName,
        sessionAffinityEnabled: false,
    };
}
