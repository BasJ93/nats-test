FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nats-test-subscriber/nats-test-subscriber.csproj", "nats-test-subscriber/"]
RUN dotnet restore "nats-test-subscriber/nats-test-subscriber.csproj"
COPY . .
WORKDIR "/src/nats-test-subscriber"
RUN dotnet build "nats-test-subscriber.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nats-test-subscriber.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nats-test-subscriber.dll"]
