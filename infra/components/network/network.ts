import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IComponentParams, CONSTANTS } from "../../components";

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

  public AddCNameRecord(name: string, record: pulumi.Input<string>): azure.dns.CNameRecord {
    const newRecord = new azure.dns.CNameRecord(name, {
      name,
      record,
      resourceGroupName: this.rg.name,
      ttl: 300,
      zoneName: this.zone.name,
    },
    {
      parent: this,
    });
    return newRecord;
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
