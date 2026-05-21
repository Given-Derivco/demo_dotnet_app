# Stage 1 – build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY WebApp.csproj ./
RUN dotnet restore WebApp.csproj
COPY . .
RUN dotnet publish WebApp.csproj -c Release -o /app/publish --no-restore

# Stage 2 – download OpenTelemetry .NET instrumentation
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS otel-download
WORKDIR /otel
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && curl -sSfL https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v1.7.0/otel-dotnet-auto-install.sh \
       -o otel-dotnet-auto-install.sh \
    && OTEL_DOTNET_AUTO_HOME=/otel/otel-dotnet-auto sh otel-dotnet-auto-install.sh \
    && rm otel-dotnet-auto-install.sh \
    && apt-get remove -y curl && apt-get autoremove -y && rm -rf /var/lib/apt/lists/*

# Stage 3 – runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Copy app
WORKDIR /app
COPY --from=build /app/publish .

# Copy OpenTelemetry instrumentation
COPY --from=otel-download /otel/otel-dotnet-auto /otel-dotnet-auto

# OpenTelemetry .NET auto-instrumentation environment variables
ENV OTEL_DOTNET_AUTO_HOME=/otel-dotnet-auto
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={918728DD-259F-4A6A-AC2B-B85E1B658318}
ENV CORECLR_PROFILER_PATH=/otel-dotnet-auto/linux-x64/OpenTelemetry.AutoInstrumentation.Native.so
ENV DOTNET_STARTUP_HOOKS=/otel-dotnet-auto/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll
ENV DOTNET_ADDITIONAL_DEPS=/otel-dotnet-auto/AdditionalDeps
ENV DOTNET_SHARED_STORE=/otel-dotnet-auto/store
ENV OTEL_SERVICE_NAME=dotnet-app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# OTEL_EXPORTER_OTLP_ENDPOINT is intentionally NOT set here;
# it is injected at runtime by the deployer.

EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/healthz || exit 1

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

ENTRYPOINT ["dotnet", "WebApp.dll"]
