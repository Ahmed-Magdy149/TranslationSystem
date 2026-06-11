# Build stage - Use .NET 10.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy all project files
COPY . .

# Restore dependencies
RUN dotnet restore src/TMS.Web/TMS.Web.csproj

# Build
RUN dotnet build src/TMS.Web/TMS.Web.csproj -c Release -o /app/build

# Publish
RUN dotnet publish src/TMS.Web/TMS.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMS.Web.dll"]