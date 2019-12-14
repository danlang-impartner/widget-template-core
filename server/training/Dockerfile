FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS base
ENV ASPNETCORE_URLS="http://+:80"

# Copy Common Projects. Each project used by this service will need to be included as a copy command.
WORKDIR /common/dotnet
COPY common/dotnet/Impartner.Microservice.Common/. ./Impartner.Microservice.Common
COPY common/dotnet/Impartner.Microservice.Common.Mongo/. ./Impartner.Microservice.Common.Mongo

WORKDIR /src/Impartner.Microservice.Training

# Copy project file and restore all Nuget pacakges.
COPY server/training/src/Impartner.Microservice.Training/*.csproj ./
RUN dotnet restore

# Copy remaining files
COPY server/training/src/Impartner.Microservice.Training/. ./

# Create a testing image and run tests.
FROM base AS testrunner
WORKDIR /src/tests

# Copy test project files and restore all Nuget pacakges.
COPY server/training/src/Impartner.Microservice.Training.Tests/*.csproj ./
RUN dotnet restore

# Copy remaining test files
COPY server/training/src/Impartner.Microservice.Training.Tests/. ./
RUN dotnet test

# Publish build if tests pass.
FROM base AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
# Set new relic env vars
ENV CORECLR_ENABLE_PROFILING=0 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=GETS_REPLACED_BY_CI \
NEW_RELIC_APP_NAME=training

# Copy new relic deps
WORKDIR /newrelic
COPY common/newrelic/newrelic-netcore20-agent*.deb .
RUN dpkg -i ./newrelic-netcore20-agent*.deb

WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT dotnet Impartner.Microservice.Training.dll
