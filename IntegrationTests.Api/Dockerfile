FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IntegrationTests.Api/IntegrationTests.Api.csproj", "IntegrationTests.Api/"]
RUN dotnet restore "IntegrationTests.Api/IntegrationTests.Api.csproj"
COPY . .
WORKDIR "/src/IntegrationTests.Api"
RUN dotnet build "IntegrationTests.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntegrationTests.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationTests.Api.dll"]
