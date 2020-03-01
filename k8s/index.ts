import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import { FrontEnd } from "./frontEnd";

const infra = new pulumi.StackReference(`xtellurian/amphora/${pulumi.getStack()}`);

const kubeConfig = infra.getOutput("kubeConfig");
const fqdn = <pulumi.Output<string>> infra.requireOutput("k8sFqdn");

var provider = new k8s.Provider("k8sProvider", {
    kubeconfig: kubeConfig,
});

var frontEnd = new FrontEnd("frontEnd", {
    fqdn,
    provider: provider
})
