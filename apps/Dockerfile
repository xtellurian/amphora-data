ARG DOTNET_SDK_VERSION=3.1.102
ARG DOTNET_RUNTIME_VERSION=3.1.2
ARG TreatWarningsAsErrors=false
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_SDK_VERSION} AS builder
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
RUN mkdir -p /tmp/appinsights
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet build /p:TreatWarningsAsErrors=${TreatWarningsAsErrors}
ENV ASPNETCORE_ENVIRONMENT=Development
ENV AZURE_CONFIG_DIR=/.azure/

CMD [ "dotnet", "test", "-l:trx;LogFileName=/output/TestOutput.xml ", "/p:CollectCoverage=true", "/p:CoverletOutputFormat=cobertura", "/p:CoverletOutput=/output/" ]
