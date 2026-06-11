# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln .
COPY src/TMS.Web/TMS.Web.csproj src/TMS.Web/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build
RUN dotnet publish src/TMS.Web/TMS.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMS.Web.dll"]