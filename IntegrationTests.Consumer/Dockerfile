FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IntegrationTests.Consumer/IntegrationTests.Consumer.csproj", "IntegrationTests.Consumer/"]
RUN dotnet restore "IntegrationTests.Consumer/IntegrationTests.Consumer.csproj"
COPY . .
WORKDIR "/src/IntegrationTests.Consumer"
RUN dotnet build "IntegrationTests.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntegrationTests.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationTests.Consumer.dll"]
