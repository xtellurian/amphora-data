import { ComponentResourceOptions } from "@pulumi/pulumi";

export interface IComponentParams {
  name: string;
  opts?: ComponentResourceOptions | undefined;
}

const COMPONENTS = {
  Application: Symbol.for("Application"),
  Monitoring: Symbol.for("Monitoring"),
  State: Symbol.for("State"),
};

const CONSTANTS = {
  AzStorage_KV_CS_SecretName: "StorageConnectionString",
  application: {
    imageName: "webapp",
  },
  authentication: {
    tenantId: "617bc365-674a-436e-8a27-5bacbe6df190",
  },
  location: {
    primary: "AustraliaSoutheast",
    secondary: "AustraliaEast",
  },
};

export { COMPONENTS, CONSTANTS };
