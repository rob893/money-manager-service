FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5003

# Start app on port 5003 in production mode
ENV ASPNETCORE_URLS=http://*:5003
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MoneyManagerService/MoneyManagerService.csproj", "MoneyManagerService/"]
RUN dotnet restore "MoneyManagerService/MoneyManagerService.csproj"
COPY . .
WORKDIR "/src/MoneyManagerService"
RUN dotnet build "MoneyManagerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoneyManagerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneyManagerService.dll"]

# FROM mcr.microsoft.com/dotnet/aspnet:6.0

# COPY bin/Release/net6.0/publish/ App/
# WORKDIR /App

# ENV ASPNETCORE_ENVIRONMENT=Production
# ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false

# ENTRYPOINT ["dotnet", "WoWMarketWatcher.API.dll"]
