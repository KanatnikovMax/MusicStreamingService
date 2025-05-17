FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MusicStreamingService/MusicStreamingService.sln", "."]
COPY ["MusicStreamingService/MusicStreamingService.Service/MusicStreamingService.Service.csproj", "MusicStreamingService.Service/"]
COPY ["MusicStreamingService/MusicStreamingService.BusinessLogic/MusicStreamingService.BusinessLogic.csproj", "MusicStreamingService.BusinessLogic/"]
COPY ["MusicStreamingService/MusicStreamingService.DataAccess.Cassandra/MusicStreamingService.DataAccess.Cassandra.csproj", "MusicStreamingService.DataAccess.Cassandra/"]
COPY ["MusicStreamingService/MusicStreamingService.DataAccess.Postgres/MusicStreamingService.DataAccess.Postgres.csproj", "MusicStreamingService.DataAccess.Postgres/"]

RUN dotnet restore "MusicStreamingService.sln"

COPY . .

WORKDIR "/src/MusicStreamingService/MusicStreamingService.Service"
RUN dotnet build "MusicStreamingService.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MusicStreamingService.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicStreamingService.Service.dll"]