import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { frontDoorDns, IFrontendHosts } from "./front-door-dns";
import { googleDns } from "./google-dns";
import { sendGridDns } from "./sendgrid";

const config = new pulumi.Config();

const ttl = 3600;

// pass through the frontend fqdns
export function createDns(rg: azure.core.ResourceGroup): IFrontendHosts {
    const tags = {
        component: "central-dns",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };

    const opts = {
        protect: false,
    };

    const dnsZone = new azure.dns.Zone(
        "centralDnsZone",
        {
            name: "amphoradata.com",
            resourceGroupName: rg.name,
            tags,
        },
        opts
    );

    const wwwCname = new azure.dns.CNameRecord(
        "wwwCname",
        {
            name: "www",
            record: "amphoradata.github.io",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    // these come from O365

    const sipDirCName = new azure.dns.CNameRecord(
        "sipDir",
        {
            name: "sip",
            record: "sipdir.online.lync.com",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );
    const linqDiscoverCName = new azure.dns.CNameRecord(
        "linqDiscover",
        {
            name: "lyncdiscover",
            record: "webdir.online.lync.com",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    const githubChallengeTxt = new azure.dns.TxtRecord(
        "githubChallenge",
        {
            name: "_github-challenge-amphoradata",
            records: [
                {
                    value: "44d6785da4",
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    // directly from o365
    // const mxRecord = new azure.dns.MxRecord(
    //     "outlookMx",
    //     {
    //         name: "@",
    //         records: [
    //             {
    //                 exchange: "amphoradata-com.mail.protection.outlook.com",
    //                 preference: "0",
    //             },
    //         ],
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: dnsZone.name,
    //     },
    //     opts
    // );

    const autodoscoverCName = new azure.dns.CNameRecord(
        "autodiscoverCName",
        {
            name: "autodiscover",
            record: "autodiscover.outlook.com",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    const txtRecord = new azure.dns.TxtRecord(
        "outlookTxtRecord",
        {
            name: "@",
            records: [
                {
                    value: "v=spf1 include:spf.protection.outlook.com -all", // from 365
                },
                {
                    value: "a9eb31102744451ca0ad66b3cc7bde06", // from digicert
                },
                {
                    value:
                        "google-site-verification=CSxCzvDVqquSRbHF34m7pD6mJdHH1Bg4BWbTj7s8q7o", // google
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    sendGridDns(rg, dnsZone);
    googleDns(rg, dnsZone);
    return frontDoorDns(rg, dnsZone);
}
