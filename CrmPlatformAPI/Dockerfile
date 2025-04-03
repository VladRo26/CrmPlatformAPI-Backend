# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the csproj and restore dependencies
COPY CrmPlatformAPI.csproj .
RUN dotnet restore CrmPlatformAPI.csproj

# Copy the rest of the files
COPY . .

# Build and publish the app
RUN dotnet publish CrmPlatformAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CrmPlatformAPI.dll"]
