import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { googleDns } from "./google-dns";

const ttl = 3600;
export const dnsBackup = (
    rg: azure.core.ResourceGroup,
    tags: any,
    dnsZone: azure.dns.Zone
) => {
    const txtRecord = new azure.dns.TxtRecord("outlookTxtRecord", {
        name: "@",
        records: [
            // {
            //     value: "v=spf1 include:spf.protection.outlook.com -all", // from 365
            // },
            {
                value: "a9eb31102744451ca0ad66b3cc7bde06", // from digicert
            },
            {
                value: "google-site-verification=CSxCzvDVqquSRbHF34m7pD6mJdHH1Bg4BWbTj7s8q7o", // google
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
    });

    const githubChallengeTxt = new azure.dns.TxtRecord("githubChallenge", {
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
    });

    googleDns(rg, dnsZone);
};
