import * as pulumi from "@pulumi/pulumi";

import { Application } from "./components/application/application";
import { AzureConfig } from "./components/azure-config/azure-config";
import { Monitoring } from "./components/monitoring/monitoring";
import { State } from "./components/state/state";

const azureConfig = new AzureConfig();
// do not create or reference container anywhere but here!

interface IMainResult {
  application: Application;
  monitoring: Monitoring;
  state: State;
}

async function main(): Promise<IMainResult> {

  await azureConfig.init();

  const monitoring = new Monitoring({ name: "monitoring" });

  const state = new State({ name: "state" }, monitoring, azureConfig);

  const application = new Application({ name: "application" }, monitoring, state, azureConfig);

  return {
    application,
    monitoring,
    state,
  };
}

// async workaround
// https://github.com/pulumi/pulumi/issues/2910
const result: Promise<IMainResult> = main();

export let instrumentatonKey = result.then((r) =>
  r.monitoring.applicationInsights.instrumentationKey,
);

export let appUrl = result.then((r) =>
  pulumi.interpolate`https://${r.application.appSvc.defaultSiteHostname}`,
);

export let kvUri = result.then((r) =>
  r.state.kv.vaultUri,
);

export let appSvcSku = result.then((r) =>
  r.application.plan.sku,
);

export let tsiFqdn = result.then((r) =>
  r.application.tsi.dataAccessFqdn,
);

export let imageName = result.then((r) =>
  r.application.imageName,
);

export let acrName = result.then((r) =>
  r.application.acr.name,
);
