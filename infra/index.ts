import * as pulumi from "@pulumi/pulumi";

import { Application } from "./components/application/application";
import { IPlanAndSlot } from "./components/application/appSvc/appSvc";
import { Business } from "./components/business/business";
import { Monitoring } from "./components/monitoring/monitoring";
import { Network } from "./components/network/network";
import { State } from "./components/state/state";

// do not create or reference container anywhere but here!

interface IMainResult {
  application: Application;
  business: Business;
  monitoring: Monitoring;
  state: State;
}

async function main(): Promise<IMainResult> {
  const monitoring = new Monitoring({ name: "monitoring" });

  const network = new Network({ name: "network" });

  const state = new State({ name: "state" }, monitoring);

  const application = new Application(
    { name: "application" },
    monitoring,
    network,
    state
  );

  const business = new Business({ name: "business" }, monitoring);

  return {
    application,
    business,
    monitoring,
    state,
  };
}
// async workaround
// https://github.com/pulumi/pulumi/issues/2910

const generateIdList = (apps: IPlanAndSlot[]): pulumi.Output<string> =>
  pulumi
    .output(apps.map((a) => pulumi.interpolate`${a.appSvc.id} `))
    .apply((array) => array.join(" "));

const result: Promise<IMainResult> = main();

export let instrumentatonKey = result.then(
  (r) => r.monitoring.applicationInsights.instrumentationKey
);
export let appHostnames = result.then((r) =>
  r.application.appSvc.mainApps.map((app) => app.appSvc.defaultSiteHostname)
);
export let identityHostnames = result.then((r) =>
  r.application.appSvc.identityApps.map((app) => app.appSvc.defaultSiteHostname)
);
export let kvUri = result.then((r) => r.state.kv.vaultUri);
export let kvName = result.then((r) => r.state.kv.name);

export let tsiFqdn = result.then((r) => r.application.tsi.dataAccessFqdn);

// export let imageName = result.then((r) =>
//   r.application.imageName,
// );

export let acrName = result.then((r) => r.application.acr.name);
export let acrId = result.then((r) => r.application.acr.id);
export let workflowTriggerId = result.then(
  (r) => r.business.workflowTrigger.id
);
export let appEventGridTopicId = result.then((r) => r.monitoring.appTopic.id);
export let webAppResourceId = result.then((r) =>
  generateIdList(r.application.appSvc.mainApps)
);
export let identityAppResourceId = result.then((r) =>
  generateIdList(r.application.appSvc.identityApps)
);

export let webAppResourceIds = result.then((r) =>
  r.application.appSvc.mainApps.map((a) => a.appSvc.id)
);
export let identityAppResourceIds = result.then((r) =>
  r.application.appSvc.identityApps.map((a) => a.appSvc.id)
);

// const asAksOutput = (c: Aks) => {
//   return {
//     aksId: c.k8sCluster.id,
//     fqdn: c.k8sInfra.fqdn,
//     fqdnName: c.k8sInfra.fqdnName,
//     group: c.k8sCluster.resourceGroupName,
//     ingressIp: c.k8sInfra.ingressIp,
//     kubeConfig: c.k8sCluster.kubeConfigRaw,
//     location: c.k8sCluster.location,
//     name: c.k8sCluster.name,
//   };
// };

// // TODO: re-enable secondary
// export let k8s = result.then((r) => {
//   return {
//     // australiaeast: asAksOutput(r.application.aks.secondary),
//     australiasoutheast: asAksOutput(r.application.aks.primary),
//   };
// });
