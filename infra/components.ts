import { ComponentResourceOptions } from "@pulumi/pulumi";

export interface IComponentParams {
  name: string;
  opts?: ComponentResourceOptions | undefined;
}

const COMPONENTS = {
  Application: Symbol.for("Application"),
  State: Symbol.for("State"),
  Monitoring: Symbol.for("Monitoring")
};

const COMPONENT_PARAMS = {
  ApplicationParams: Symbol.for("ApplicationParams"),
  StateParams: Symbol.for("StateParams"),
  MonitoringParams: Symbol.for("MonitoringParams")
};

const CONSTANTS = {
  AzStorage_KV_CS_SecretName: "StorageConnectionString"
}

export { COMPONENTS, COMPONENT_PARAMS, CONSTANTS };
