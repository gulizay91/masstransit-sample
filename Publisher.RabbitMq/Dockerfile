FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Publisher.RabbitMq/Publisher.RabbitMq.csproj", "Publisher.RabbitMq/"]
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
RUN dotnet restore "Publisher.RabbitMq/Publisher.RabbitMq.csproj"
COPY . .
WORKDIR "/src/Publisher.RabbitMq"
RUN dotnet build "Publisher.RabbitMq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Publisher.RabbitMq.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Publisher.RabbitMq.dll"]
