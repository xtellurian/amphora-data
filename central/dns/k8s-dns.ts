import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IKubernetesCluster, IMultiCluster } from "../contracts";

const tags = {
    component: "central-dns-develop",
    project: pulumi.getProject(),
    source: "pulumi",
    stack: pulumi.getStack(),
};

export interface IK8sDnsParams {
    clusters: IMultiCluster;
    rg: azure.core.ResourceGroup;
    zone: azure.dns.Zone;
    environment: string;
}

export class K8sDns extends pulumi.ComponentResource {
    constructor(
        private name: string,
        params: IK8sDnsParams,
        opts?: pulumi.ComponentResourceOptions,
    ) {
        super("amphora:K8sDns", name, {}, opts);
        this.create(
            params.rg,
            params.zone,
            params.environment,
            params.clusters,
        );
    }

    private create(
        rg: azure.core.ResourceGroup,
        zone: azure.dns.Zone,
        env: string,
        clusters: IMultiCluster,
    ) {
        // k8s dns things
        const xxx: { [key: string]: IKubernetesCluster } = clusters as any;

        Object.keys(clusters).forEach((c, index) => {
            this.createForLocation({
                cluster: xxx[c],
                env,
                index,
                rg,
                zone,
            });
        });
        // this.createForLocation({ rg, zone, env, cluster: clusters.australiasoutheast, index: 0});
        // TODO: renable secondary
        // this.createForLocation({ rg, zone, env, cluster: clusters.australiaeast, index: 1 });
    }

    private createForLocation({
        rg,
        zone,
        env,
        cluster,
        index,
    }: {
        rg: azure.core.ResourceGroup;
        zone: azure.dns.Zone;
        env: string;
        cluster: IKubernetesCluster;
        index: number;
    }) {

        // don't set Develop or Master DNS here.
        // set it in update-main-dns-sh script.
        const appARecord = new azure.dns.ARecord(
            `${env}-${this.name}-${index}-AppARec`,
            {
                name: pulumi.interpolate`${env}.${cluster.location}.app`,
                records: [cluster.ingressIp],
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            }
        );

        const apiARecord = new azure.dns.ARecord(
            `${env}-${this.name}-${index}-ApiARec`,
            {
                name: pulumi.interpolate`${env}.${cluster.location}.api`,
                records: [cluster.ingressIp],
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            }
        );

        const identityARecord = new azure.dns.ARecord(
            `${env}-${this.name}-${index}-IdARec`,
            {
                name: pulumi.interpolate`${env}.${cluster.location}.identity`,
                records: [cluster.ingressIp],
                resourceGroupName: rg.name,
                tags,
                ttl: 30,
                zoneName: zone.name,
            },
            {
                parent: this,
            }
        );
    }
}
