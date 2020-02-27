import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { CONSTANTS, IComponentParams } from "../../components";

const azTags = {
  component: "network",
  project: pulumi.getProject(),
  source: "pulumi",
  stack: pulumi.getStack(),
};

const rgName = pulumi.getStack() + "-network";

export class Network extends pulumi.ComponentResource {
  private rg: azure.core.ResourceGroup;
  private zone: azure.dns.Zone;

  constructor(
    params: IComponentParams,
  ) {
    super("amphora:Network", params.name, {}, params.opts);

    this.create();
  }

  private create() {
    const rg = new azure.core.ResourceGroup(
      rgName,
      {
        location: CONSTANTS.location.primary,
        tags: azTags,
      },
      { parent: this },
    );

    this.rg = rg;

    const zone = new azure.dns.Zone("dnsZone", {
      name: `${pulumi.getStack()}.amphoradata.internal`,
      resourceGroupName: rg.name,
    },
    {
      parent: this,
    });

    this.zone = zone;
  }
}
