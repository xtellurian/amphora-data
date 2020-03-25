import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IMultiEnvironmentMultiCluster } from "../contracts";
import { IFrontendHosts, IUniqueUrl } from "../dns/front-door-dns";

const config = new pulumi.Config();

export interface IBackendPoolNames {
    app: string;
    identity: string;
}

export interface IBackendEnvironments {
    develop: IBackendPoolNames;
    master: IBackendPoolNames;
    prod: IBackendPoolNames;
}

const locations = {
    mel: "australiasoutheast",
    syd: "australiaeast",
};

const prodBackendCount = 2;

export function getBackendPools({ backendEnvironments, frontendHosts, prodHostnames }:
    {
        // param type definition
        backendEnvironments: IBackendEnvironments;
        frontendHosts: IFrontendHosts;
        prodHostnames: pulumi.Output<any>;
    })
    : pulumi.Input<Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>> {

    const backendPools: pulumi.Input<Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>> = [];
    // add the squarespace backend
    backendPools.push(
        {
            backends: [{
                address: "azalea-orca-x567.squarespace.com",
                hostHeader: "azalea-orca-x567.squarespace.com",
                httpPort: 80,
                httpsPort: 443,
            }],
            healthProbeName: "normal",
            loadBalancingName: "loadBalancingSettings1",
            name: "squarespaceBackend",
        });

    // APP SERVICE BACKENDS
    // add app service backends
    const prodBackends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
    for (let i = 0; i < prodBackendCount; i++) {
        prodBackends.push({
            address: prodHostnames.apply((h) => h[i]),
            hostHeader: frontendHosts.prod.app.globalHost,
            httpPort: 80,
            httpsPort: 443,
        });
    }

    // add k8s primary + secondary backend from aks
    prodBackends.push({
        address: `prod.${locations.syd}.app.${domain}`,
        hostHeader: `prod.${locations.syd}.app.${domain}`,
        httpPort: 80,
        httpsPort: 443,
    });
    prodBackends.push({
        address: `prod.${locations.mel}.app.${domain}`,
        hostHeader: `prod.${locations.mel}.app.${domain}`,
        httpPort: 80,
        httpsPort: 443,
    });

    const prodBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: prodBackends,
        healthProbeName: "healthz",
        loadBalancingName: "loadBalancingSettings1",
        name: backendEnvironments.prod.app,
    };

    backendPools.push(prodBackendPool);

    // Prod ID pool
    const prodIdPool = createPool(`${backendEnvironments.prod.identity}`, "prod", frontendHosts.prod.identity);

    backendPools.push(prodIdPool);

    // create develop pools
    const devAppPool = createPool(`${backendEnvironments.develop.app}`, "develop", frontendHosts.develop.app);
    const devIdPool = createPool(`${backendEnvironments.develop.identity}`, "develop", frontendHosts.develop.identity);

    // create master pools
    const masterAppPool = createPool(`${backendEnvironments.master.app}`, "master", frontendHosts.master.app);
    const masterIdPool = createPool(`${backendEnvironments.master.identity}`, "master", frontendHosts.master.identity);

    // add tobackends
    if (config.requireBoolean("deployDevelop")) {
        backendPools.push(devAppPool);
        backendPools.push(devIdPool);
    }
    if (config.requireBoolean("deployMaster")) {
        backendPools.push(masterAppPool);
        backendPools.push(masterIdPool);
    }

    return backendPools;
}

const domain = "amphoradata.com";

function createPool(poolName: string, envName: string, url: IUniqueUrl)
    : azure.types.input.frontdoor.FrontdoorBackendPool {
    const backends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
    // add sydney
    const sydHost = `${envName}.${locations.syd}.${url.appName}.${domain}`;
    backends.push({
        address: sydHost,
        hostHeader: sydHost,
        httpPort: 80,
        httpsPort: 443,
    });
    // add melbourne
    const melHost = `${envName}.${locations.mel}.${url.appName}.${domain}`;
    backends.push({
        address: melHost,
        hostHeader: melHost,
        httpPort: 80,
        httpsPort: 443,
    });

    const backendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends,
        healthProbeName: "healthz",
        loadBalancingName: "loadBalancingSettings1",
        name: poolName,
    };
    return backendPool;
}
