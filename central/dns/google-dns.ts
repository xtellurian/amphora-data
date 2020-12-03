import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

export function googleDns(
    rg: azure.core.ResourceGroup,
    dnsZone: azure.dns.Zone
) {
    // google-site-verification=CSxCzvDVqquSRbHF34m7pD6mJdHH1Bg4BWbTj7s8q7o

    const ttl = 3600;
    const opts = {
        protect: false,
    };

    const tags = {
        component: "central-dns-google",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };

    const mxRecord = new azure.dns.MxRecord(
        "googleMx",
        {
            name: "@",
            records: [
                {
                    exchange: "ASPMX.L.GOOGLE.COM",
                    preference: "1",
                },
                {
                    exchange: "ALT1.ASPMX.L.GOOGLE.COM.",
                    preference: "5",
                },
                {
                    exchange: "ALT2.ASPMX.L.GOOGLE.COM",
                    preference: "5",
                },
                {
                    exchange: "ALT3.ASPMX.L.GOOGLE.COM",
                    preference: "10",
                },
                {
                    exchange: "ALT4.ASPMX.L.GOOGLE.COM",
                    preference: "10",
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );
}
