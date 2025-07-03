# Flow OverStack - UserService
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)

## Project Overview

UserService is a microservice responsible for all user-related operations within the flow OverStack platform. It provides user authentication with Keycloak for identity management and roles management.

## Technologies and Patterns Used

* **.NET 9 & C#** — Core framework and language
* **ASP.NET Core** — HTTP API.
* **Entity Framework Core with PostgreSQL** — Data access (Repository & Unit of Work patterns) to PostgreSQL database
* **Clean Architecture** — Layered separation (Domain, Application, Infrastructure)
* **Decorator Pattern** — allows behavior to be added to individual objects dynamically without affecting others. In this project, it is used to implement caching.
* **Keycloak** — OAuth2/OpenID Connect identity provider
* **Hangfire** — Hosted services for background jobs
* **Redis** — Caching layer
* **Kafka** — Message queue that listens to main events
* **Hot Chocolate** — GraphQL endpoint with built-in support for pagination, filtering, and sorting
* **gRPC** — High-performance RPC interface
* **Observability** — Traces, logs, and metrics collected via OpenTelemetry and Logstash, exported to Aspire dashboard, Jaeger, ElasticSearch, and Prometheus
* **Monitoring & Visualization** — Dashboards in Grafana, Kibana and Aspire
* **Health Checks** — Status endpoints to monitor service availability and dependencies
* **xUnit & Coverlet** — Automated unit and integration testing with code coverage
* **SonarQube & Qodana** — Code quality and coverage analysis

## Architecture and Design
This service follows the principles of Clean Architecture. The solution is split into multiple projects that correspond to each architectural layer.

![Clean Architecture](https://www.milanjovanovic.tech/blogs/mnw_017/clean_architecture.png?imwidth=1920)

| Layer | Project |
| ----- | ------- |
| **Presentation** | UserService.Grpc, UserService.GraphQl, UserService.Api |
| **Application** | UserService.Application |
| **Domain** | UserService.Domain |
| **Infrastructure** | UserService.BackgroundJobs, UserService.Cache, UserService.DAL, UserService.Keycloak, UserService.Messaging |

Full system design on Miro: [Application Structure Board](https://miro.com/app/board/uXjVLx6YYx4=/?share_link_id=993967197754)

## Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://docs.docker.com/desktop)
* Running service dependencies

### Installation

1. Clone the repo
2. Configure `appsettings.json` and `.NET User Secrets` in `UserService.Api` with your database, Redis, and Keycloak settings.
   Example: 
   ```json
    {
    "ConnectionStrings": {
    "PostgresSQL": "Server=localhost;Port=5433; Database=user-service-db; User Id=<YOUR-USER-ID>; Password=<YOUR-PASSWORD>"
    },
    "KeycloakSettings": {
      "AdminToken": "<YOUR-TOKEN>"
    },
    "RedisSettings": {
      "Password": "<YOUR-PASSWORD>"
    }
   }
   ```
4. Start supporting services:

   ```bash
   docker-compose -p flowoverstack -f docker-compose.common.yml up -d
   docker-compose -p userservice -f docker-compose.yml up -d
   ```
5. Run the API:

   ```bash
   cd UserService.Api
   dotnet run
   ```
## API Documentation

The following endpoints are available by default:

| `UseHttpsForRestApi` | REST API & Swagger | GraphQL Endpoint | gRPC Endpoint |
| ------------------ | ------------------ | ---------------- | ------------- |
| `true` | https://localhost:7163/swagger |	https://localhost:7163/graphql | https://localhost:7163 http://localhost:5044 |
| `false`	| http://localhost:7163/swagger |	http://localhost:7163/graphql |	http://localhost:5044 |

## Testing

Run unit and functional tests:

```bash
cd UserService.Tests
dotnet test --filter Category=Functional
dotnet test --filter Category=Unit
```

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_UserService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_UserService)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch 
5. Open a Pull Request

Please follow the existing code conventions and include tests for new functionality.
You are also welcome to open issues for bug reports, feature requests, or to discuss improvements. 

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/flow-OverStack/UserService/blob/master/LICENSE) file for details.

## Contact

For questions or suggestions open an issue.
