import * as pulumi from "@pulumi/pulumi";

import { Application } from "./components/application/application";
import { Monitoring } from "./components/monitoring/monitoring";
import { Network } from "./components/network/network";
import { State } from "./components/state/state";

// do not create or reference container anywhere but here!

interface IMainResult {
  application: Application;
  monitoring: Monitoring;
  state: State;
}

async function main(): Promise<IMainResult> {

  const monitoring = new Monitoring({ name: "monitoring" });

  const network = new Network({ name: "network" });

  const state = new State({ name: "state" }, monitoring);

  const application = new Application({ name: "application" }, monitoring, network, state);

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
export let kvName = result.then((r) =>
  r.state.kv.name,
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

export let AzureMapsClientId = result.then((r) =>
  r.application.AzureMaps.clientId,
);
export let AzureMapsAccountName = result.then((r) =>
  r.application.AzureMaps.accountName,
);
export let AzureMapsResourceGroup = result.then((r) =>
  r.application.AzureMaps.resourceGroup,
);

export let webAppResourceId = result.then((r) =>
  r.application.appSvc.id,
);
