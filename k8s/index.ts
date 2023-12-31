import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import { FrontEnd } from "./frontEnd";
import { AmphoraApi } from "./amphoraApi";
import { Identity } from "./identity";
import { Certificates } from "./certificates";

// stack = parent env - child env
// so stack == develop-primary OR develop-secondary

const stack = pulumi.getStack();
const stackSplit = stack.split("-");
if (stackSplit.length != 2) {
  throw new Error("Stack Name does not conform");
}

const environment = stack.split("-")[0];
const region = stack.split("-")[1];

const infra = new pulumi.StackReference(`xtellurian/amphora/${environment}`);

const k8sOutputs = infra.requireOutput("k8s");
const k8sInfo = k8sOutputs.apply((_) => _[region]);

const provider = new k8s.Provider("provider1", {
  kubeconfig: k8sInfo.apply((k) => k.kubeConfig),
});

const certs = new Certificates("certificates", {
  provider,
});

const frontEnd = new FrontEnd(
  "frontend1",
  {
    environment,
    location: <pulumi.Output<string>>k8sInfo.apply((k) => k.location),
    provider,
  },
  { dependsOn: certs }
);

const api = new AmphoraApi(
  "api",
  {
    environment,
    location: <pulumi.Output<string>>k8sInfo.apply((k) => k.location),
    provider,
  },
  { dependsOn: certs }
);

const identity = new Identity(
  "identity1",
  {
    environment,
    location: <pulumi.Output<string>>k8sInfo.apply((k) => k.location),
    provider,
  },
  { dependsOn: certs }
);

export let aks = {
  name: k8sInfo.apply((k) => k.name),
  group: k8sInfo.apply((k) => k.group),
};
