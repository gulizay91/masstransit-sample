FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Consumer.Console/Consumer.Console.csproj", "Consumer.Console/"]
RUN dotnet restore "Consumer.Console/Consumer.Console.csproj"
COPY . .
WORKDIR "/src/Consumer.Console"
RUN dotnet build "Consumer.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Console.dll"]
