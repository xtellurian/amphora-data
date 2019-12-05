ARG DOTNET_VERSION=3.1
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_VERSION} AS builder
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet build
ENV ASPNETCORE_ENVIRONMENT=Development
ENV AZURE_CONFIG_DIR=/.azure/

CMD [ "dotnet", "test", "-l:trx;LogFileName=/output/TestOutput.xml ", "/p:CollectCoverage=true", "/p:CoverletOutputFormat=cobertura", "/p:CoverletOutput=/output/" ]

FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_VERSION} AS publisher
WORKDIR /app
COPY --from=builder /app .
RUN dotnet publish -c Release -o out api

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:${DOTNET_VERSION}-alpine
ARG gitHash
RUN echo gitHash arg is ${gitHash}
ENV ASPNETCORE_SOURCEVERSION ${gitHash}
RUN echo env var is ${ASPNETCORE_SOURCEVERSION}
WORKDIR /app
COPY --from=publisher /app/out .
ENTRYPOINT ["dotnet", "api.dll"]
