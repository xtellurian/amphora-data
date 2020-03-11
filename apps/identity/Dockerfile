ARG DOTNET_SDK_VERSION=3.1.102
ARG DOTNET_RUNTIME_VERSION=3.1.2
ARG TreatWarningsAsErrors=false
ARG BASE=builder

FROM ${BASE} AS publisher
WORKDIR /app
COPY --from=builder /app .
RUN dotnet publish -c Release -o out identity /p:TreatWarningsAsErrors=${TreatWarningsAsErrors}

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:${DOTNET_RUNTIME_VERSION}-alpine
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ARG gitHash
RUN echo gitHash arg is ${gitHash}
ENV ASPNETCORE_SOURCEVERSION ${gitHash}
RUN echo env var is ${ASPNETCORE_SOURCEVERSION}
WORKDIR /app
COPY --from=publisher /app/out .
ENTRYPOINT ["dotnet", "identity.dll"]
