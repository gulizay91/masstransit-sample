FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Consumer.RabbitMq/Consumer.RabbitMq.csproj", "Consumer.RabbitMq/"]
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
RUN dotnet restore "Consumer.RabbitMq/Consumer.RabbitMq.csproj"
COPY . .
WORKDIR "/src/Consumer.RabbitMq"
RUN dotnet build "Consumer.RabbitMq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.RabbitMq.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.RabbitMq.dll"]
