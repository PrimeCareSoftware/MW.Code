FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy telemedicine projects
COPY telemedicine/src/MedicSoft.Telemedicine.Domain/MedicSoft.Telemedicine.Domain.csproj telemedicine/src/MedicSoft.Telemedicine.Domain/
COPY telemedicine/src/MedicSoft.Telemedicine.Application/MedicSoft.Telemedicine.Application.csproj telemedicine/src/MedicSoft.Telemedicine.Application/
COPY telemedicine/src/MedicSoft.Telemedicine.Infrastructure/MedicSoft.Telemedicine.Infrastructure.csproj telemedicine/src/MedicSoft.Telemedicine.Infrastructure/
COPY telemedicine/src/MedicSoft.Telemedicine.Api/MedicSoft.Telemedicine.Api.csproj telemedicine/src/MedicSoft.Telemedicine.Api/

# Restore dependencies
RUN dotnet restore telemedicine/src/MedicSoft.Telemedicine.Api/MedicSoft.Telemedicine.Api.csproj

# Copy all source files
COPY telemedicine/ telemedicine/

# Build
WORKDIR /src/telemedicine/src/MedicSoft.Telemedicine.Api
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicSoft.Telemedicine.Api.dll"]
