import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import { FrontEnd } from "./frontEnd";

const infra = new pulumi.StackReference(`xtellurian/amphora/${pulumi.getStack()}`);

const k8sPrimary = infra.requireOutput("k8sPrimary");
const k8sSecondary = infra.requireOutput("k8sSecondary");

const primaryProvider = new k8s.Provider("provider1", {
    kubeconfig: k8sPrimary.apply(k => k.kubeConfig),
});
const secondaryProvider = new k8s.Provider("provider2", {
    kubeconfig: k8sSecondary.apply(k => k.kubeConfig),
});

const frontEnd1 = new FrontEnd("frontend1", {
    fqdn: <pulumi.Output<string>>k8sPrimary.apply(k => k.fqdn),
    provider: primaryProvider
});

const frontEnd2 = new FrontEnd("frontend2", {
    fqdn: <pulumi.Output<string>>k8sSecondary.apply(k => k.fqdn),
    provider: secondaryProvider
});
