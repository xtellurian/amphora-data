import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

export function sendGridDns(rg: azure.core.ResourceGroup, dnsZone: azure.dns.Zone) {

    const opts = {
        protect: true,
    };

    const tags = {
        component: "central-dns-sendgrid",
        project: pulumi.getProject(),
        source: "pulumi",
        stack: pulumi.getStack(),
    };
    // Send Grid (APP)
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

    // Send Grid (Marketting)
    // Adding for Isaac's marketting campaign tool
    const markettingCname1 = new azure.dns.CNameRecord("marketing1",
        {
            name: "em7374",
            record: "u15779133.wl134.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
    );
    const markettingCname2 = new azure.dns.CNameRecord("marketing2",
        {
            name: "mkt._domainkey",
            record: "mkt.domainkey.u15779133.wl134.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
    );
    const markettingCname3 = new azure.dns.CNameRecord("marketing3",
        {
            name: "mkt2._domainkey",
            record: "mkt2.domainkey.u15779133.wl134.sendgrid.net",
            resourceGroupName: rg.name,
            tags,
            ttl: 60,
            zoneName: dnsZone.name,
        },
    );
}
