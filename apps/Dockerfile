FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore ./api

RUN dotnet publish -c Release -o out api

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine
ARG gitHash
ENV ASPNETCORE_SOURCEVERSION ${gitHash}
WORKDIR /app
COPY --from=build-env /app/api/out .
ENTRYPOINT ["dotnet", "api.dll"]
