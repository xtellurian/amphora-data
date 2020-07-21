import * as k8s from "@pulumi/kubernetes";
import * as pulumi from "@pulumi/pulumi";
import { IK8sIdentities } from "./aks";

const stack = pulumi.getStack();

export interface IK8sInfrastructureParams {
  provider: k8s.Provider;
  location: pulumi.Output<string>;
  identities: IK8sIdentities;
  appSettings: pulumi.Input<{
    [key: string]: pulumi.Input<string>;
  }>;
}

export class K8sInfrastructure extends pulumi.ComponentResource {
  public ingressController: k8s.yaml.ConfigFile;
  public fqdnName: pulumi.Output<string>;
  public fqdn: pulumi.Output<string>;
  public ingressIp: pulumi.Output<string>;
  constructor(
    private name: string,
    private params: IK8sInfrastructureParams,
    opts?: pulumi.ComponentResourceOptions
  ) {
    super("amphora:K8sInfrastructure", name, {}, opts);
    this.create();
  }

  private create() {
    const opts: pulumi.CustomResourceOptions = {
      parent: this,
      provider: this.params.provider,
    };

    // namespaces
    const amphoraNamespace = new k8s.core.v1.Namespace(
      `${this.name}-amphora`,
      {
        metadata: {
          name: "amphora",
        },
      },
      opts
    );

    const aadPodIdentityNs = new k8s.core.v1.Namespace(
      `${this.name}-aad-pod-id`,
      {
        metadata: {
          name: "aad-pod-identity",
        },
      },
      opts
    );

    const ingressNginxNamespace = new k8s.core.v1.Namespace(
      `${this.name}-nginx`,
      {
        metadata: {
          labels: {
            "app.kubernetes.io/name": "ingress-nginx",
            "app.kubernetes.io/part-of": "ingress-nginx",
          },
          name: "ingress-nginx",
        },
      },
      opts
    );

    // aad pod identity
    const aadPods = new k8s.yaml.ConfigFile(
      `${this.name}-aadPods`,
      {
        file:
          "components/application/aks/infrastructure-manifests/aad-pod-identity.yml",
        resourcePrefix: this.name,
      },
      {
        ...opts,
        dependsOn: aadPodIdentityNs,
      }
    );

    const webAppId = {
      apiVersion: "aadpodidentity.k8s.io/v1",
      kind: "AzureIdentity",
      metadata: {
        name: "web-app-identity",
        namespace: amphoraNamespace.metadata.name,
      },
      spec: {
        ClientID: this.params.identities.webApp.clientId,
        ResourceID: this.params.identities.webApp.id,
        type: 0, // 0 = MSI, 1 = SP
      },
    };

    const idSrvrId = {
      apiVersion: "aadpodidentity.k8s.io/v1",
      kind: "AzureIdentity",
      metadata: {
        name: "identity-server-identity",
        namespace: amphoraNamespace.metadata.name,
      },
      spec: {
        ClientID: this.params.identities.identityServer.clientId,
        ResourceID: this.params.identities.identityServer.id,
        type: 0, // 0 = MSI, 1 = SP
      },
    };

    const identity = new k8s.yaml.ConfigGroup(
      `${this.name}-identities`,
      {
        objs: [webAppId, idSrvrId],
        resourcePrefix: this.name,
      },
      {
        ...opts,
        dependsOn: aadPods,
      }
    );

    const webAppBinding = {
      apiVersion: "aadpodidentity.k8s.io/v1",
      kind: "AzureIdentityBinding",
      metadata: {
        name: "frontend-azure-id-binding",
        namespace: amphoraNamespace.metadata.name,
      },
      spec: {
        AzureIdentity: webAppId.metadata.name,
        Selector: "amphora-front",
      },
    };

    const idSrvrBinding = {
      apiVersion: "aadpodidentity.k8s.io/v1",
      kind: "AzureIdentityBinding",
      metadata: {
        name: "id-server-azure-id-binding",
        namespace: amphoraNamespace.metadata.name,
      },
      spec: {
        AzureIdentity: webAppId.metadata.name,
        Selector: "amphora-identity",
      },
    };

    const binding = new k8s.yaml.ConfigGroup(
      `${this.name}-idBindings`,
      {
        objs: [webAppBinding, idSrvrBinding],
        resourcePrefix: this.name,
      },
      {
        ...opts,
        dependsOn: [identity, aadPods],
      }
    );

    this.ingressController = new k8s.yaml.ConfigFile(
      `${this.name}-ingressCtlr`,
      {
        file:
          "components/application/aks/infrastructure-manifests/ingress-nginx.yml",
        resourcePrefix: this.name,
      },
      opts
    );

    // amphora frontend config map
    const webAppConfigMap = new k8s.core.v1.ConfigMap(
      `${this.name}-front-config`,
      {
        data: this.params.appSettings,
        metadata: {
          name: "amphora-frontend-config",
          namespace: amphoraNamespace.metadata.name,
        },
      },
      opts
    );

    // amphora identity config map
    const identityConfigMap = new k8s.core.v1.ConfigMap(
      `${this.name}-identity-config`,
      {
        data: this.params.appSettings,
        metadata: {
          name: "amphora-identity-config",
          namespace: amphoraNamespace.metadata.name,
        },
      },
      opts
    );

    // aksAU1-infra-ingress-nginx/ingress-nginx == with redourcePrefix
    this.ingressIp = this.ingressController
      .getResource("v1/Service", `${this.name}-ingress-nginx`, `ingress-nginx`)
      .apply((service) => service.status.loadBalancer.ingress[0].ip);

    this.fqdnName = pulumi.interpolate`${stack}-amphoradata`;
    this.fqdn = pulumi.interpolate`${this.fqdnName}.${this.params.location}.cloudapp.azure.com`;
  }
}
