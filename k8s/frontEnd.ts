import * as pulumi from "@pulumi/pulumi";
import * as k8s from "@pulumi/kubernetes";
import * as kx from "@pulumi/kubernetesx";
import { common } from "./ingressAnnotations";

export interface IFrontendArgs {
  environment: string;
  location: pulumi.Output<string>;
  provider: k8s.Provider;
}

const frontendConfig = new pulumi.Config("frontend");
const image = frontendConfig.require("image");
const replicas = frontendConfig.getNumber("replicas") ?? 1;

const name = "amphora-front";

export class FrontEnd extends pulumi.ComponentResource {
  constructor(
    private name: string,
    private params: IFrontendArgs,
    opts?: pulumi.ComponentResourceOptions
  ) {
    super("amphora-k8s:FrontEnd", name, {}, opts);
    this.create();
  }

  private create() {
    const opts = {
      parent: this,
      provider: this.params.provider,
    };

    const pb = new kx.PodBuilder({
      nodeSelector: {
        "beta.kubernetes.io/os": "linux",
      },
      containers: [
        {
          // name is not required. If not provided, it is inferred from the image.
          name,
          image,
          ports: { http: 80 }, // Simplified ports syntax.
          resources: {
            requests: {
              cpu: "100m",
              memory: "128Mi",
            },
            limits: {
              cpu: "250m",
              memory: "256Mi",
            },
          },
          envFrom: [
            {
              configMapRef: {
                name: "amphora-frontend-config",
              },
            },
          ],
          env: [{ name: "FeatureManagement__Spa", value: "false" }], // old app, no SPA
          livenessProbe: {
            httpGet: {
              path: "/healthz",
              port: 80,
            },
            initialDelaySeconds: 3,
            periodSeconds: 10,
          },
          readinessProbe: {
            httpGet: {
              path: "/healthz",
              port: 80,
            },
            initialDelaySeconds: 3,
            periodSeconds: 3,
          },
        },
      ],
    });

    const deployment = new kx.Deployment(
      `${this.name}-deploy`,
      {
        apiVersion: "apps/v1",
        metadata: {
          name,
          namespace: "amphora",
        },
        spec: {
          replicas,
          selector: {
            matchLabels: {
              app: name,
            },
          },
          template: {
            metadata: {
              labels: {
                app: name,
                aadpodidbinding: "amphora-front",
              },
            },
            spec: pb.podSpec,
          },
        },
      },
      opts
    );

    const service = new kx.Service(
      `${this.name}-svc`,
      {
        kind: "Service",
        metadata: {
          name,
          namespace: "amphora",
        },
        spec: {
          ports: [
            {
              port: 80,
            },
          ],
          selector: {
            app: name,
          },
        },
      },
      opts
    );

    // http config for each host
    const http = {
      paths: [
        {
          backend: {
            serviceName: name,
            servicePort: 80,
          },
          path: "/(.*)",
        },
      ],
    };
    // add the rule for each host
    const rules: pulumi.Input<
      k8s.types.input.networking.v1beta1.IngressRule
    >[] = [];

    const hosts: pulumi.Output<string>[] = [
      pulumi.interpolate`${this.params.environment}.${this.params.location}.app.amphoradata.com`,
      // pulumi.interpolate `${env}.app.amphoradata.com` // the front foor
    ];

    if (hosts) {
      hosts.forEach((h) => {
        rules.push({
          host: h,
          http,
        });
      });
    }

    const ingress = new k8s.networking.v1beta1.Ingress(
      `${this.name}-ingress`,
      {
        kind: "Ingress",
        metadata: {
          name: `${name}-ingress`,
          namespace: "amphora",
          annotations: {
            ...common,
          },
        },
        spec: {
          tls: [
            {
              hosts: [...hosts],
              secretName: "front-tls-secret",
            },
          ],
          rules,
        },
      },
      { ...opts, deleteBeforeReplace: true }
    );
  }
}
