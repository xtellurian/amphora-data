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

const COMPONENT_PARAMS = {
  ApplicationParams: Symbol.for("ApplicationParams"),
  MonitoringParams: Symbol.for("MonitoringParams"),
  StateParams: Symbol.for("StateParams"),
};

const CONSTANTS = {
  AzStorage_KV_CS_SecretName: "StorageConnectionString",
  application: {
    imageName: "webapp",
  },
  authentication: {
    rian: "16ab5eb9-f9e7-4df0-b40c-58ad0e0022d8",
    spAppId: "4a60dad9-8036-4ff5-9653-c38e6e89f303",
    spObjectId: "cd43f2d2-7e12-48fc-a856-713d04ab71ed",
    subscriptionId: "651f2ed5-6e2f-41d0-b533-0cd28801ef2a",
    tenantId: "617bc365-674a-436e-8a27-5bacbe6df190",
  },
  location: {
    primary: "AustraliaSoutheast",
    secondary: "AustraliaEast",
  },
};

export { COMPONENTS, COMPONENT_PARAMS, CONSTANTS };
