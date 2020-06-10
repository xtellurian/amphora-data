import * as pulumi from "@pulumi/pulumi";

export interface IKubernetesCluster {
    ingressIp: string | pulumi.Output<string>;
    location: string | pulumi.Output<string>;
}

export interface IMultiCluster {
    australiaeast?: IKubernetesCluster;
    australiasoutheast: IKubernetesCluster;
}

export interface IMultiEnvironmentMultiCluster {
    develop: IMultiCluster;
    master: IMultiCluster;
    prod: IMultiCluster;
}
