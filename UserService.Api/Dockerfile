FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService.Api/UserService.Api.csproj", "UserService.Api/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["UserService.BackgroundTasks/UserService.BackgroundTasks.csproj", "UserService.BackgroundTasks/"]
COPY ["UserService.DAL/UserService.DAL.csproj", "UserService.DAL/"]
COPY ["UserService.GraphQl/UserService.GraphQl.csproj", "UserService.GraphQl/"]
COPY ["UserService.Keycloak/UserService.Keycloak.csproj", "UserService.Keycloak/"]
COPY ["UserService.ReputationConsumer/UserService.ReputationConsumer.csproj", "UserService.ReputationConsumer/"]
RUN dotnet restore "UserService.Api/UserService.Api.csproj"
COPY . .
WORKDIR "/src/UserService.Api"
RUN dotnet build "UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
