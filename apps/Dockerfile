FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore ./api

RUN dotnet publish -c Release -o out api

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine
ARG gitHash
RUN echo gitHash arg is ${gitHash}
ENV ASPNETCORE_SOURCEVERSION ${gitHash}
RUN echo env var is ${ASPNETCORE_SOURCEVERSION}
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "api.dll"]
