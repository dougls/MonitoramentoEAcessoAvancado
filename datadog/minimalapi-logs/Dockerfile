# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . .

RUN dotnet publish -c Release -o build

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

RUN apt-get update && \
    apt-get install -y ca-certificates

COPY --from=datadog/serverless-init:1 /datadog-init /app/datadog-init
COPY --from=datadog/dd-lib-dotnet-init /datadog-init/monitoring-home/ /dd_tracer/dotnet/

ENV DD_API_KEY="add-license-key-here"
ENV DD_SITE="us5.datadoghq.com"
ENV DD_SERVICE=dotnet-logs
ENV DD_ENV=development
ENV DD_VERSION=1
ENV DD_HOSTNAME=dotnet-logs
ENV DD_LOGS_INJECTION=true
ENV DD_LOGS_ENABLED=true
ENV DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true
ENV DD_CONTAINER_EXCLUDE="name:datadog-agent"
ENV DD_TRACE_ENABLED=true
ENV DD_TRACE_PROPAGATION_STYLE=datadog

COPY --from=build /app/build .

EXPOSE 3000

CMD ["/app/datadog-init", "dotnet", "MinimalApi.dll"]