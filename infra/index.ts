import * as pulumi from "@pulumi/pulumi";

import { Application, ApplicationParams } from "./components/application/application";
import { State, StateParams } from "./components/state/state";
import { Monitoring, MonitoringParams } from "./components/monitoring/monitoring";
import { AzureConfig } from "./components/azure-config/azure-config";

import { Tsi } from "./components/application/tsi/tsi";

const azureConfig = new AzureConfig();
// do not create or reference container anywhere but here!

interface MainResult {
  monitoring: Monitoring;
  state: State;
  application: Application
}

async function main(): Promise<MainResult> {

  await azureConfig.init();

  const monitoring = new Monitoring(new MonitoringParams());

  const state = new State(new StateParams(), monitoring, azureConfig);

  const application = new Application(new ApplicationParams(), monitoring, state, azureConfig);


  return {
    monitoring,
    state,
    application
  };
}

// async workaround
// https://github.com/pulumi/pulumi/issues/2910
const result: Promise<MainResult> = main();

export let instrumentatonKey = result.then(r =>
  r.monitoring ? r.monitoring.appInsights.instrumentationKey : null
);

export let appUrl = result.then(r =>
  r.application ? pulumi.interpolate`https://${r.application.appSvc.defaultSiteHostname}` : null
);

export let kvUri = result.then(r =>
  r.state ? r.state.kv.vaultUri : null
);

export let appSvcSku = result.then(r =>
  r.application ? r.application.plan.sku : null
);

export let tsiFqdn = result.then(r =>
  r.application.tsi.dataAccessFqdn
);

// export let instrumentatonKey = result.then(r => r.monitoring ?  r.monitoring.appInsights.instrumentationKey : null  )
