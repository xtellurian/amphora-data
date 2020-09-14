import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

export function squarespaceDns(
    rg: azure.core.ResourceGroup,
    dnsZone: azure.dns.Zone
) {
    const opts = {
        protect: true,
    };

    const tags = {
        component: "central-dns-squarespace",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };
    const verify = new azure.dns.CNameRecord(
        "sq_v1",
        {
            name: "jr2gtsx5btwz3few95fd",
            record: "verify.squarespace.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts
    );

    const www = new azure.dns.CNameRecord(
        "sq_www",
        {
            name: "ai",
            record: "ext-cust.squarespace.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts
    );
}
