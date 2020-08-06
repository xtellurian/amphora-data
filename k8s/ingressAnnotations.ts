export const common = {
  "kubernetes.io/ingress.class": "nginx",
  "cert-manager.io/cluster-issuer": "letsencrypt",
  "nginx.ingress.kubernetes.io/limit-rps": "5", // rate limits per second per IP address
  "nginx.ingress.kubernetes.io/limit-rpm": "300", // rate limits per minute per IP address
  "nginx.ingress.kubernetes.io/rewrite-target": " /$1",
  "nginx.ingress.kubernetes.io/proxy-body-size": "0", // disable body size checking
};
