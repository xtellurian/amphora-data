import * as pulumi from "@pulumi/pulumi";

export interface IAppServiceBackend {
    webAppUrl: pulumi.Output<string[]>;
    identityUrl: pulumi.Output<string[]>;
}
