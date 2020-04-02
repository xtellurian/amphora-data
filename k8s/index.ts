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

const k8sInfo = infra.requireOutput(`k8s${region}`)

// const k8sPrimary = infra.requireOutput("k8sPrimary");
// const k8sSecondary = infra.requireOutput("k8sSecondary");

const provider = new k8s.Provider("provider1", {
    kubeconfig: k8sInfo.apply(k => k.kubeConfig),
});
// const secondaryProvider = new k8s.Provider("provider2", {
//     kubeconfig: k8sSecondary.apply(k => k.kubeConfig),
// });

// Primary
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

// Secondary
// const frontEnd2 = new FrontEnd("frontend2", {
//     fqdn: <pulumi.Output<string>>k8sSecondary.apply(k => k.fqdn),
//     location: <pulumi.Output<string>>k8sSecondary.apply(k => k.location),
//     provider: secondaryProvider
// });

// const identity2 = new Identity("identity2", {
//     fqdn: <pulumi.Output<string>>k8sSecondary.apply(k => k.fqdn),
//     location: <pulumi.Output<string>>k8sSecondary.apply(k => k.location),
//     provider: secondaryProvider
// });
