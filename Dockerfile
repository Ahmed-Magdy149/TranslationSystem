# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY TranslationManagementSystem.slnx .

# Copy project files
COPY src/TMS.Core/TMS.Core.csproj src/TMS.Core/
COPY src/TMS.Application/TMS.Application.csproj src/TMS.Application/
COPY src/TMS.Infrastructure/TMS.Infrastructure.csproj src/TMS.Infrastructure/
COPY src/TMS.Web/TMS.Web.csproj src/TMS.Web/

# Restore dependencies
RUN dotnet restore src/TMS.Web/TMS.Web.csproj

# Copy everything else
COPY src/ src/

# Build and publish
WORKDIR /src/src/TMS.Web
RUN dotnet publish TMS.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "TMS.Web.dll"]
