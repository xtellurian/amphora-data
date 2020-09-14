import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IMultiEnvironmentMultiCluster } from "../contracts";
import { frontDoorDns, IFrontendHosts } from "./front-door-dns";
import { K8sDns } from "./k8s-dns";
import { sendGridDns } from "./sendgrid";
import { squarespaceDns } from "./squarespace";

const config = new pulumi.Config();

const ttl = 3600;

// pass through the frontend fqdns
export function createDns(rg: azure.core.ResourceGroup, clusters: IMultiEnvironmentMultiCluster): IFrontendHosts {

    const tags = {
        component: "central-dns",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };

    const opts = {
        protect: false,
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
    if (config.requireBoolean("deployDevelop")) {
        const devDns = new K8sDns("k8sDevelop", {
            clusters: clusters.develop,
            environment: "develop",
            rg,
            zone: dnsZone,
        });
    }
    if (config.requireBoolean("deployMaster")) {
        const masterDns = new K8sDns("k8sMaster", {
            clusters: clusters.master,
            environment: "master",
            rg,
            zone: dnsZone,
        });
    }

    const prodDns = new K8sDns("k8sProd", {
        clusters: clusters.prod,
        environment: "prod",
        rg,
        zone: dnsZone,
    });

    const wwwCname = new azure.dns.CNameRecord("wwwCname",
        {
            name: "www",
            record: "amphoradata.github.io",
            resourceGroupName: rg.name,
            tags,
            ttl,
            zoneName: dnsZone.name,
        },
        opts,
    );

    // these come from O365

    const sipDirCName = new azure.dns.CNameRecord("sipDir",
        {
            name: "sip",
            record: "sipdir.online.lync.com",
            resourceGroupName: rg.name,
            tags,
            ttl,
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
            ttl,
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
            ttl,
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
            ttl,
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
            ttl,
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
            ttl,
            zoneName: dnsZone.name,
        },
        opts,
    );

    sendGridDns(rg, dnsZone);
    squarespaceDns(rg, dnsZone);

    return frontDoorDns(rg, dnsZone);
}
