FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gw-Demo.Api/Gw-Demo.Api.csproj", "Gw-Demo.Api/"]
RUN dotnet restore "Gw-Demo.Api/Gw-Demo.Api.csproj"
COPY . .
WORKDIR "/src/Gw-Demo.Api"
RUN dotnet build "Gw-Demo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gw-Demo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gw-Demo.Api.dll"]
