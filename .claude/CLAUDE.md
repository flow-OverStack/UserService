# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

UserService is a microservice for the flow OverStack platform handling user, role, and
reputation management. Identity (auth, registration, credentials) is delegated to
**Keycloak**; this service owns the user/role/reputation data and exposes it over three
parallel surfaces: **REST**, **GraphQL** (Hot Chocolate), and **gRPC** — all hosted in the
same process on Kestrel.

## Build / Run / Test

The repo targets **.NET 10** (`global.json` pins SDK `10.0`, all `.csproj` are `net10.0`).
The README's ".NET 9" text is stale — trust the project files.

```bash
# Build whole solution
dotnet build

# Run the API (REST + GraphQL on 7163, gRPC on 5044 by default)
cd UserService.Api
dotnet run
```

Tests are split into two xUnit categories via `[Trait("Category", ...)]`:

```bash
cd UserService.Tests
dotnet test --filter Category=Unit          # fast, no external deps
dotnet test --filter Category=Functional    # requires Docker (Testcontainers)
dotnet test --filter "FullyQualifiedName~UpdateUsername"   # a single test/class
```

**Functional tests need a running Docker daemon** — `FunctionalTestWebAppFactory` spins up
PostgreSQL + Redis containers via Testcontainers and stubs Keycloak with WireMock. Unit
tests have no external dependencies.

### EF Core migrations

Migrations live in `UserService.DAL/Migrations`; the startup project (`UserService.Api`)
holds the EF Design package and the connection string.

```bash
dotnet ef migrations add <Name> --project UserService.DAL --startup-project UserService.Api
```

Migrations are **auto-applied on startup only when `ASPNETCORE_ENVIRONMENT=Development`**
(`MigrateDatabaseAsync` in `Program.cs`). In production, generate and apply a SQL script.

## Architecture

Clean Architecture, one project per layer (see `UserService.sln` solution folders):

| Layer | Projects |
|-------|----------|
| Domain | `UserService.Domain` — entities, DTOs, enums, interfaces, settings, the Result types. No external deps. |
| Application | `UserService.Application` — services (business logic), AutoMapper mappings, FluentValidation validators, localized error resources. |
| Infrastructure | `UserService.DAL` (EF Core/Postgres), `UserService.Cache` (Redis), `UserService.Keycloak` (identity HTTP client), `UserService.Messaging` (Kafka/MassTransit), `UserService.BackgroundJobs` (Hangfire). |
| Presentation | `UserService.Api` (REST + composition root), `UserService.GraphQl` (Hot Chocolate), `UserService.GrpcServer`. |

Each non-Domain project owns a `DependencyInjection/DependencyInjection.cs` with an
`Add<Layer>()` extension; `Program.cs` wires them together, and `Startup.cs` holds the
cross-cutting `IServiceCollection`/`WebApplication` extensions (auth, Swagger, Hangfire,
OpenTelemetry, CORS, Kestrel ports, resilience).

### Key patterns — internalize these before editing

- **Result pattern, not exceptions for business outcomes.** Services return
  `BaseResult` / `BaseResult<T>` / `CollectionResult<T>` / `QueryableResult<T>`
  (`UserService.Domain/Results`). Success/failure is data; `ErrorMessage` + `ErrorCode`
  carry failures. Controllers translate via `HandleBaseResult` in `BaseController`.
  `ErrorCodes` enum + localized `ErrorMessage.resx` are the source of error identity.

- **Caching = Decorator pattern.** Read services have a plain implementation
  (`GetUserService`) and a cache wrapper (`CacheGetUserService`) registered with Scrutor's
  `services.Decorate<IGetUserService, CacheGetUserService>()` in the Application DI. The
  wrapper calls the Redis cache repo's `Get...OrFetchAndCacheAsync`, delegating to `inner`
  on miss. **To add caching to a read path, add a `Cache*` decorator — don't put cache
  logic in the base service.** Cache uses short TTLs and **negative caching**
  (`NullTimeToLiveInSeconds`) to cache misses.

- **DI registration is convention-based (Scrutor).** Application services and validators
  are auto-registered by assembly scan (`InExactNamespaceOf<AuthService>`,
  `AssignableTo(typeof(IValidator<>))`). A new service in `Application/Services` with a
  matching `Domain/Interfaces/Service` interface is picked up automatically — no manual
  registration needed (decorators are the exception, registered explicitly).

- **Data access:** Repository + Unit of Work (`IBaseRepository`, `IUnitOfWork`,
  `BaseRepository`, `UnitOfWork`). `DateInterceptor` stamps `IAuditable` entities.
  Entity config lives in `DAL/Configurations`.

- **Messaging is idempotent.** Kafka consumed via MassTransit (`BaseEventConsumer`).
  `ProcessedEventFilter` dedupes by persisted `ProcessedEvent`; `ResilientConsumeFilter`
  adds retry/circuit-breaker; failures route to a dead-letter topic. Inbound events drive
  reputation changes (`ReputationService.ApplyReputationEventAsync`).

- **Auth:** JWT Bearer validated against Keycloak (`MetadataAddress`). `MapInboundClaims`
  is **false** on purpose — original OAuth2 claim names are preserved for inter-service
  use. `ClaimsValidationMiddleware` enforces required claims. Role-gated endpoints use
  `[Authorize(Roles = ...)]` with the `Roles` enum.

- **Localization:** error messages support `en` and `ru-by` via resx; culture flows
  through `UseLocalization`.

### Cross-cutting

- **Background jobs (Hangfire on Postgres):** `ProcessedEventsResetJob`,
  `SyncUserActivitiesJob`; scheduled in `SetupHangfireJobs`. 10-attempt retry backoff up
  to 24h. Dashboard only in Development.
- **Observability:** OpenTelemetry traces/metrics/logs → Aspire dashboard, Jaeger,
  Prometheus; Serilog → Console/File/Logstash/OTel. Health checks at `/health` cover DB,
  Kafka, Redis, Elasticsearch, Hangfire, Keycloak, and the telemetry backends.
- **HTTP resilience:** `AddSafeResilienceHandler` applies standard retry/circuit-breaker
  to all `HttpClient`s but **never retries POST/PATCH on exceptions** (unknown whether the
  request reached the server); transient 5xx/408/429 are retried for all methods.

## Configuration

Settings bind from `appsettings.json` + env vars (double-underscore nesting, see
`docker-compose.yml`) + .NET User Secrets for local dev (connection string, Keycloak admin
token, Redis password — `UserSecretsId` in `UserService.Api.csproj`). `KeycloakSettings`,
`RedisSettings`, `KafkaSettings`, `PaginationRules` are the strongly-typed options.

Run the full local stack with `docker-compose.common.yml` (shared infra) +
`docker-compose.yml` (this service); both need a populated `.env` (see README).
