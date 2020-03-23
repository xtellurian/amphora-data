FROM mcr.microsoft.com/azure-cli
RUN az aks install-cli
RUN echo >> ~/.bashrc
RUN echo 'alias "k=kubectl"' >> ~/.bashrc
ENTRYPOINT az aks get-credentials -n "$name" -g "$group" && /bin/bash