# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY CrmPlatformAPI/CrmPlatformAPI.csproj CrmPlatformAPI/
RUN dotnet restore CrmPlatformAPI/CrmPlatformAPI.csproj

# Copy the source code and build the project
COPY CrmPlatformAPI/. ./CrmPlatformAPI/
WORKDIR /src/CrmPlatformAPI
RUN dotnet publish CrmPlatformAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CrmPlatformAPI.dll"]
