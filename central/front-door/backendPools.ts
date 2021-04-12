import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IAppServiceBackend } from "../contracts";
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

const prodBackendCount = 1;

export function getBackendPools({
    backendEnvironments,
    frontendHosts,
    prod,
    master,
    develop,
}: {
    // param type definition
    backendEnvironments: IBackendEnvironments;
    frontendHosts: IFrontendHosts;
    prod: IAppServiceBackend;
    master?: IAppServiceBackend;
    develop?: IAppServiceBackend;
}): pulumi.Input<
    Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>
> {
    const backendPools: pulumi.Input<Array<
        pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>
    >> = [];

    // const prodApplicationBackendPool = createPool(
    //     backendEnvironments.prod.app,
    //     frontendHosts.prod.app,
    //     "quickstart",
    //     prod
    // );

    // backendPools.push(prodApplicationBackendPool);

    // THE API PROD POOL
    // const prodAPIBackends: Array<pulumi.Input<
    //     azure.types.input.frontdoor.FrontdoorBackendPoolBackend
    // >> = [];
    // prodAPIBackends.push({
    //     address: `prod.${locations.mel}.api.${domain}`,
    //     hostHeader: `prod.${locations.mel}.api.${domain}`,
    //     httpPort: 80,
    //     httpsPort: 443,
    // });

    // const prodAPIBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
    //     backends: prodAPIBackends,
    //     healthProbeName: "quickstart",
    //     loadBalancingName: "loadBalancingSettings1",
    //     name: backendEnvironments.prod.api,
    // };

    // backendPools.push(prodAPIBackendPool);

    // Prod ID pool
    // const prodIdPool = createPool(
    //     backendEnvironments.prod.identity,
    //     frontendHosts.prod.identity,
    //     "normal",
    //     prod
    // );

    // backendPools.push(prodIdPool);

    // add tobackends
    // if (config.requireBoolean("deployDevelop")) {
    //     // create develop pools
    //     // const devApiPool = createPool(
    //     //     backendEnvironments.develop.api,
    //     //     frontendHosts.develop.api,
    //     //     "normal"
    //     // );
    //     const devAppPool = createPool(
    //         backendEnvironments.develop.app,
    //         frontendHosts.develop.app,
    //         "quickstart",
    //         develop
    //     );
    //     const devIdPool = createPool(
    //         backendEnvironments.develop.identity,
    //         frontendHosts.develop.identity,
    //         "normal",
    //         develop
    //     );
    //     backendPools.push(devAppPool);
    //     // backendPools.push(devApiPool);
    //     backendPools.push(devIdPool);
    // }
    // if (config.requireBoolean("deployMaster")) {
    //     // create master pools
    //     // const masterApiPool = createPool(
    //     //     backendEnvironments.master.api,
    //     //     frontendHosts.master.api,
    //     //     "normal"
    //     // );
    //     const masterAppPool = createPool(
    //         backendEnvironments.master.app,
    //         frontendHosts.master.app,
    //         "quickstart",
    //         master
    //     );
    //     const masterIdPool = createPool(
    //         backendEnvironments.master.identity,
    //         frontendHosts.master.identity,
    //         "normal",
    //         master
    //     );

    //     backendPools.push(masterAppPool);
    //     // backendPools.push(masterApiPool);
    //     backendPools.push(masterIdPool);
    // }

    return backendPools;
}

function createPool(
    poolName: string,
    url: IUniqueUrl,
    healthProbeName: string,
    appSvc?: IAppServiceBackend
): azure.types.input.frontdoor.FrontdoorBackendPool {
    const backends: Array<pulumi.Input<
        azure.types.input.frontdoor.FrontdoorBackendPoolBackend
    >> = [];
    if (appSvc && appSvc.identityUrl.length && appSvc.webAppUrl.length) {
        // tslint:disable-next-line: no-console
        console.log(`Creating appsvc pool for ${poolName}`);
        if (url.appName === "identity") {
            for (let i = 0; i < prodBackendCount; i++) {
                backends.push({
                    address: appSvc.identityUrl.apply((h) => h[i]),
                    hostHeader: appSvc.identityUrl.apply((h) => h[i]),
                    httpPort: 80,
                    httpsPort: 443,
                });
            }
        } else if (url.appName === "app") {
            for (let i = 0; i < prodBackendCount; i++) {
                backends.push({
                    address: appSvc.webAppUrl.apply((h) => h[i]),
                    hostHeader: appSvc.webAppUrl.apply((h) => h[i]),
                    httpPort: 80,
                    httpsPort: 443,
                });
            }
        }
    } else {
        // tslint:disable-next-line: no-console
        console.log(`Not creating appsvc pool for ${poolName}`);
    }

    const backendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends,
        healthProbeName,
        loadBalancingName: "loadBalancingSettings1",
        name: poolName,
    };
    return backendPool;
}
