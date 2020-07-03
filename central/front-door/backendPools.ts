import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IMultiEnvironmentMultiCluster } from "../contracts";
import { IFrontendHosts, IUniqueUrl } from "../dns/front-door-dns";

const config = new pulumi.Config();

export interface IBackendPoolNames {
    app: string;
    api: string;
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

export function getBackendPools({
    backendEnvironments,
    frontendHosts,
    prodHostnames,
}: {
    // param type definition
    backendEnvironments: IBackendEnvironments;
    frontendHosts: IFrontendHosts;
    prodHostnames: pulumi.Output<any>;
}): pulumi.Input<
    Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>
> {
    const backendPools: pulumi.Input<Array<
        pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>
    >> = [];

    // APP SERVICE BACKENDS
    // add app service backends
    const prodApplicationBackends: Array<pulumi.Input<
        azure.types.input.frontdoor.FrontdoorBackendPoolBackend
    >> = [];
    for (let i = 0; i < prodBackendCount; i++) {
        prodApplicationBackends.push({
            address: prodHostnames.apply((h) => h[i]),
            hostHeader: frontendHosts.prod.app.globalHost,
            httpPort: 80,
            httpsPort: 443,
        });
    }

    // add k8s primary + secondary backend from aks
    // sydney cluster removed for cost reasons
    // prodBackends.push({
    //     address: `prod.${locations.syd}.app.${domain}`,
    //     hostHeader: `prod.${locations.syd}.app.${domain}`,
    //     httpPort: 80,
    //     httpsPort: 443,
    // });
    prodApplicationBackends.push({
        address: `prod.${locations.mel}.app.${domain}`,
        hostHeader: `prod.${locations.mel}.app.${domain}`,
        httpPort: 80,
        httpsPort: 443,
    });

    const prodApplicationBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: prodApplicationBackends,
        healthProbeName: "quickstart",
        loadBalancingName: "loadBalancingSettings1",
        name: backendEnvironments.prod.app,
    };

    backendPools.push(prodApplicationBackendPool);

    // THE API PROD POOL
    const prodAPIBackends: Array<pulumi.Input<
        azure.types.input.frontdoor.FrontdoorBackendPoolBackend
    >> = [];
    prodAPIBackends.push({
        address: `prod.${locations.mel}.api.${domain}`,
        hostHeader: `prod.${locations.mel}.api.${domain}`,
        httpPort: 80,
        httpsPort: 443,
    });

    const prodAPIBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: prodAPIBackends,
        healthProbeName: "quickstart",
        loadBalancingName: "loadBalancingSettings1",
        name: backendEnvironments.prod.api,
    };

    backendPools.push(prodAPIBackendPool);

    // Prod ID pool
    const prodIdPool = createPool(
        `${backendEnvironments.prod.identity}`,
        "prod",
        frontendHosts.prod.identity
    );

    backendPools.push(prodIdPool);

    // create develop pools
    const devAppPool = createPool(
        `${backendEnvironments.develop.app}`,
        "develop",
        frontendHosts.develop.app,
        "quickstart"
    );
    const devApiPool = createPool(
        `${backendEnvironments.develop.api}`,
        "develop",
        frontendHosts.develop.api
    );
    const devIdPool = createPool(
        `${backendEnvironments.develop.identity}`,
        "develop",
        frontendHosts.develop.identity
    );

    // create master pools
    const masterAppPool = createPool(
        `${backendEnvironments.master.app}`,
        "master",
        frontendHosts.master.app,
        "quickstart"
    );
    const masterApiPool = createPool(
        `${backendEnvironments.master.api}`,
        "master",
        frontendHosts.master.api
    );
    const masterIdPool = createPool(
        `${backendEnvironments.master.identity}`,
        "master",
        frontendHosts.master.identity
    );

    // add tobackends
    if (config.requireBoolean("deployDevelop")) {
        backendPools.push(devAppPool);
        backendPools.push(devApiPool);
        backendPools.push(devIdPool);
    }
    if (config.requireBoolean("deployMaster")) {
        backendPools.push(masterAppPool);
        backendPools.push(masterApiPool);
        backendPools.push(masterIdPool);
    }

    return backendPools;
}

const domain = "amphoradata.com";

function createPool(
    poolName: string,
    envName: string,
    url: IUniqueUrl,
    healthProbeName: string = "normal"
): azure.types.input.frontdoor.FrontdoorBackendPool {
    const backends: Array<pulumi.Input<
        azure.types.input.frontdoor.FrontdoorBackendPoolBackend
    >> = [];
    // add sydney
    // TODO: renable secondary
    // const sydHost = `${envName}.${locations.syd}.${url.appName}.${domain}`;
    // backends.push({
    //     address: sydHost,
    //     hostHeader: sydHost,
    //     httpPort: 80,
    //     httpsPort: 443,
    // });
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
        healthProbeName,
        loadBalancingName: "loadBalancingSettings1",
        name: poolName,
    };
    return backendPool;
}
