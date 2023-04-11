FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nats-test-publisher/nats-test-publisher.csproj", "nats-test-publisher/"]
RUN dotnet restore "nats-test-publisher/nats-test-publisher.csproj"
COPY . .
WORKDIR "/src/nats-test-publisher"
RUN dotnet build "nats-test-publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nats-test-publisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nats-test-publisher.dll"]
