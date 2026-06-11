# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj file first (for layer caching)
COPY src/TMS.Web/TMS.Web.csproj src/TMS.Web/

# Restore dependencies
RUN dotnet restore src/TMS.Web/TMS.Web.csproj

# Copy everything else
COPY . .

# Build
RUN dotnet build src/TMS.Web/TMS.Web.csproj -c Release -o /app/build

# Publish
RUN dotnet publish src/TMS.Web/TMS.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMS.Web.dll"]