import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { frontDoorDns } from "./dns/front-door-dns";
import { K8sDns } from "./dns/k8s-dns";

export function createDns(rg: azure.core.ResourceGroup) {

    const tags = {
        component: "central-dns",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };

    const opts = {
        protect: true,
    };

    const dnsZone = new azure.dns.Zone("centralDnsZone",
        {
            name: "amphoradata.com",
            resourceGroupName: rg.name,
            tags,
        },
        opts,
    );

    // k8s dns
    const devDns = new K8sDns("k8dDevelop", {
        environment: "develop",
        rg,
        zone: dnsZone,
    });
    const masterDns = new K8sDns("k8dMaster", {
        environment: "master",
        rg,
        zone: dnsZone,
    });
    const prodDns = new K8sDns("k8dProd", {
        environment: "prod",
        rg,
        zone: dnsZone,
    });

    const docsCName = new azure.dns.CNameRecord("docsCName",
        {
            name: "docs",
            record: "amphoradata.github.io",
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: dnsZone.name,
        },
        opts,
    );

    frontDoorDns(rg, dnsZone);

    // these come from O365

    const sipDirCName = new azure.dns.CNameRecord("sipDir",
        {
            name: "sip",
            record: "sipdir.online.lync.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: dnsZone.name,
        },
        opts,
    );
    const linqDiscoverCName = new azure.dns.CNameRecord("linqDiscover",
        {
            name: "lyncdiscover",
            record: "webdir.online.lync.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: dnsZone.name,
        },
        opts,
    );

    const githubChallengeTxt = new azure.dns.TxtRecord("githubChallenge",
        {
            name: "_github-challenge-amphoradata",
            records: [
                {
                    value: "44d6785da4",
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl: 30,
            zoneName: dnsZone.name,
        },
        opts,
    );

    // directly from o365
    const mxRecord = new azure.dns.MxRecord("outlookMx",
        {
            name: "@",
            records: [
                {
                    exchange: "amphoradata-com.mail.protection.outlook.com",
                    preference: "0",
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl: 3600,
            zoneName: dnsZone.name,
        },
        opts,
    );

    const autodoscoverCName = new azure.dns.CNameRecord("autodiscoverCName",
        {
            name: "autodiscover",
            record: "autodiscover.outlook.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 3600,
            zoneName: dnsZone.name,
        },
        opts,
    );

    const txtRecord = new azure.dns.TxtRecord("outlookTxtRecord",
        {
            name: "@",
            records: [
                {
                    value: "v=spf1 include:spf.protection.outlook.com -all", // from 365
                },
                {
                    value: "a9eb31102744451ca0ad66b3cc7bde06", // from digicert
                },
            ],
            resourceGroupName: rg.name,
            tags,
            ttl: 3600,
            zoneName: dnsZone.name,
        },
        opts,
    );

    // squarespace
    const verifyCName = new azure.dns.CNameRecord("verifySquarespace",
        {
            name: "4cexeacc9y5xc84bxj78",
            record: "verify.squarespace.com",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts,
    );

    // Send Grid

    const sendGridCName1 = new azure.dns.CNameRecord("sendGridCName1",
        {
            name: "em2252",
            record: "u12987374.wl125.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts,
    );
    const sendGridCName2 = new azure.dns.CNameRecord("sendGridCName2",
        {
            name: "s1._domainkey",
            record: "s1.domainkey.u12987374.wl125.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts,
    );
    const sendGridCName3 = new azure.dns.CNameRecord("sendGridCName3",
        {
            name: "s2._domainkey",
            record: "s2.domainkey.u12987374.wl125.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
        opts,
    );
}
