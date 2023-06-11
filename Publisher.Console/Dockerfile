FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MassTransit.Publisher/MassTransit.Publisher.csproj", "MassTransit.Publisher/"]
RUN dotnet restore "MassTransit.Publisher/MassTransit.Publisher.csproj"
COPY . .
WORKDIR "/src/MassTransit.Publisher"
RUN dotnet build "MassTransit.Publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransit.Publisher.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransit.Publisher.dll"]
