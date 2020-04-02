import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import { FrontEnd } from "./frontEnd";
import { Identity } from "./identity";

// stack = parent env - child env
// so stack == develop-primary OR develop-secondary

const stack = pulumi.getStack();
const stackSplit = stack.split("-");
if(stackSplit.length != 2) {
    throw new Error("Stack Name does not conform");
}

const parent = stack.split("-")[0];
const region = stack.split("-")[1];

const infra = new pulumi.StackReference(`xtellurian/amphora/${parent}`);

const k8sInfo = infra.requireOutput(`${region}`)

const provider = new k8s.Provider("provider1", {
    kubeconfig: k8sInfo.apply(k => k.kubeConfig),
});

const frontEnd = new FrontEnd("frontend1", {
    fqdn: <pulumi.Output<string>>k8sInfo.apply(k => k.fqdn),
    location: <pulumi.Output<string>>k8sInfo.apply(k => k.location),
    provider,
});

const identity = new Identity("identity1", {
    fqdn: <pulumi.Output<string>>k8sInfo.apply(k => k.fqdn),
    location: <pulumi.Output<string>>k8sInfo.apply(k => k.location),
    provider,
});
