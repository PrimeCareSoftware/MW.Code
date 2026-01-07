# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PatientPortal.sln .
COPY PatientPortal.Api/PatientPortal.Api.csproj PatientPortal.Api/
COPY PatientPortal.Application/PatientPortal.Application.csproj PatientPortal.Application/
COPY PatientPortal.Domain/PatientPortal.Domain.csproj PatientPortal.Domain/
COPY PatientPortal.Infrastructure/PatientPortal.Infrastructure.csproj PatientPortal.Infrastructure/
COPY PatientPortal.Tests/PatientPortal.Tests.csproj PatientPortal.Tests/

# Restore dependencies
RUN dotnet restore

# Copy remaining source code
COPY . .

# Build the application
WORKDIR /src/PatientPortal.Api
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "PatientPortal.Api.dll"]
