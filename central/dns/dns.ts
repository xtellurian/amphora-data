import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { googleDns } from "./google-dns";
import { sendGridDns } from "./sendgrid";

const config = new pulumi.Config();

const ttl = 3600;

// pass through the frontend fqdns
export function createDns(
    rg: azure.core.ResourceGroup,
    prodAppHostnames: pulumi.Output<string[]>,
    prodidentityHostnames: pulumi.Output<string[]>
) {
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

    // const autodoscoverCName = new azure.dns.CNameRecord(
    //     "autodiscoverCName",
    //     {
    //         name: "autodiscover",
    //         record: "autodiscover.outlook.com",
    //         resourceGroupName: rg.name,
    //         tags,
    //         ttl,
    //         zoneName: dnsZone.name,
    //     },
    //     opts
    // );

    const txtRecord = new azure.dns.TxtRecord(
        "outlookTxtRecord",
        {
            name: "@",
            records: [
                // {
                //     value: "v=spf1 include:spf.protection.outlook.com -all", // from 365
                // },
                {
                    value: "a9eb31102744451ca0ad66b3cc7bde06", // from digicert
                },
                {
                    value:
                        "google-site-verification=CSxCzvDVqquSRbHF34m7pD6mJdHH1Bg4BWbTj7s8q7o", // google
                },
                {
                    value: "e5ui5u2bpkb2bctg7r2naa78u7", // azure app appsvc custom domain
                },
                {
                    value: "4n0iprt7e78uq8pm1v485eqbgp", // azure identity appsvc custom domain
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    const docsCName = new azure.dns.CNameRecord(
        "docsCName",
        {
            name: "docs",
            record: "amphoradata.com",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    // PROD
    // change this to point to the app service
    const appCName = new azure.dns.CNameRecord(
        "appCName",
        {
            name: "app",
            record: prodAppHostnames[0],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );

    const identityCName = new azure.dns.CNameRecord(
        "identityCName",
        {
            name: "identity",
            record: prodidentityHostnames[0],
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts
    );
    // verify
    const appSvcVerify = new azure.dns.TxtRecord("appVerify", {
        name: "asuid.app",
        records: [
            {
                value:
                    "E810C0D4D042CC579109F76D82F5486A39355982D942DE747F9C2D4CD23A1E97",
            },
        ],
        resourceGroupName: rg.name,
        tags,
        ttl,
        zoneName: dnsZone.name,
    });

    sendGridDns(rg, dnsZone);
    googleDns(rg, dnsZone);
    // return frontDoorDns(rg, dnsZone);
}
