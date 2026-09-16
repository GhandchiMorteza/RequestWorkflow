# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS source

WORKDIR /src

# Copy project files first to improve Docker layer caching
COPY ["src/RequestWorkflow.Api/RequestWorkflow.Api.csproj", "src/RequestWorkflow.Api/"]
COPY ["src/RequestWorkflow.Application/RequestWorkflow.Application.csproj", "src/RequestWorkflow.Application/"]
COPY ["src/RequestWorkflow.Infrastructure/RequestWorkflow.Infrastructure.csproj", "src/RequestWorkflow.Infrastructure/"]
COPY ["src/RequestWorkflow.Domain/RequestWorkflow.Domain.csproj", "src/RequestWorkflow.Domain/"]

RUN dotnet restore "src/RequestWorkflow.Api/RequestWorkflow.Api.csproj"

COPY . .


# -----------------------
# Publish API
# -----------------------

FROM source AS publish

RUN dotnet publish \
    "src/RequestWorkflow.Api/RequestWorkflow.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# -----------------------
# Build EF migration bundle
# -----------------------

FROM source AS migration-build

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet tool install \
    --global dotnet-ef \
    --version 10.0.12

RUN dotnet ef migrations bundle \
    --project "src/RequestWorkflow.Infrastructure/RequestWorkflow.Infrastructure.csproj" \
    --startup-project "src/RequestWorkflow.Api/RequestWorkflow.Api.csproj" \
    --configuration Release \
    --output /app/efbundle


# -----------------------
# Migration image
# -----------------------

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrations

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=migration-build /app/efbundle /app/efbundle

RUN chmod +x /app/efbundle

ENTRYPOINT ["/app/efbundle"]


# -----------------------
# API runtime
# -----------------------

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RequestWorkflow.Api.dll"]
