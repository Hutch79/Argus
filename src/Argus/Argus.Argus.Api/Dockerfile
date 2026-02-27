FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Argus.Argus.Api/Argus.Argus.Api.csproj", "Argus.Argus.Api/"]
RUN dotnet restore "Argus.Argus.Api/Argus.Argus.Api.csproj"
COPY . .
WORKDIR "/src/Argus.Argus.Api"
RUN dotnet build "./Argus.Argus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Argus.Argus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Argus.Argus.Api.dll"]
