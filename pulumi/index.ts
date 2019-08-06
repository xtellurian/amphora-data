import * as pulumi from "@pulumi/pulumi";
import { amphoraContainer } from "./inversify.config";
import { COMPONENTS } from "./components";

import { IApplication } from "./components/application/application";
import { IState } from "./components/state/state";
import { IMonitoring } from "./components/monitoring/monitoring";
import {
  AzureConfig,
  IAzureConfig
} from "./components/azure-config/azure-config";

import { Tsi } from "./components/tsi/tsi";
import { appendFile } from "fs";

const config = new pulumi.Config("compose");
const azureConfig = new AzureConfig();
// do not create or reference container anywhere but here!

interface MainResult {
  monitoring?: IMonitoring | undefined;
  state?: IState | undefined;
  application?: IApplication | undefined;
}

async function main(): Promise<MainResult> {
  let outputs: MainResult = {};
  await azureConfig.init();
  amphoraContainer
    .bind<IAzureConfig>("azure-config")
    .toConstantValue(azureConfig);

  if (config.getBoolean("monitoring")) {
    const monitoring = amphoraContainer.get<IMonitoring>(COMPONENTS.Monitoring);
    outputs.monitoring = monitoring;
  }

  if (config.getBoolean("state")) {
    const state = amphoraContainer.get<IState>(COMPONENTS.State);
    outputs.state = state;
  }

  if (config.getBoolean("application")) {
    const application = amphoraContainer.get<IApplication>(
      COMPONENTS.Application
    );
    outputs.application = application;
  }

  if (outputs.application) {
    const tsi = new Tsi("testtsi", {
      eh_namespace: outputs.application.eventHubCollections[0].namespace, // very fragile code
      eh: outputs.application.eventHubCollections[0].hubs[0],
      appSvc: outputs.application.appSvc
    })
  }
  return outputs;
}

// async workaround
// https://github.com/pulumi/pulumi/issues/2910
const result: Promise<MainResult> = main();

export let instrumentatonKey = result.then(r =>
  r.monitoring ? r.monitoring.appInsights.instrumentationKey : null
);

export let appUrl = result.then(r =>
  r.application ? pulumi.interpolate `https://${r.application.appSvc.defaultSiteHostname}` : null
);

export let kvUri = result.then(r =>
  r.state ? r.state.kv.vaultUri : null
);

export let appSvcSku = result.then(r =>
  r.application ? r.application.plan.sku : null
);

// export let instrumentatonKey = result.then(r => r.monitoring ?  r.monitoring.appInsights.instrumentationKey : null  )
