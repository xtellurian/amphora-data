ARG DOTNET_SDK_VERSION=3.1.102
ARG DOTNET_RUNTIME_VERSION=3.1.2
ARG TreatWarningsAsErrors=false
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_SDK_VERSION} AS builder
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
RUN mkdir -p /tmp/appinsights
WORKDIR /app

# install node
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash
# Install Yarn
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add - && \
    echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list && \
    apt -y update && apt install -y yarn

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet build /p:TreatWarningsAsErrors=${TreatWarningsAsErrors}
ENV ASPNETCORE_ENVIRONMENT=Development
ENV AZURE_CONFIG_DIR=/.azure/
RUN chmod +x ./test-ci.sh
CMD ./test-ci.sh
