FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared library
COPY microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj microservices/shared/MedicSoft.Shared.Authentication/
RUN dotnet restore microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj

# Copy Appointments API
COPY microservices/appointments/MedicSoft.Appointments.Api/MedicSoft.Appointments.Api.csproj microservices/appointments/MedicSoft.Appointments.Api/
RUN dotnet restore microservices/appointments/MedicSoft.Appointments.Api/MedicSoft.Appointments.Api.csproj

# Copy all source files
COPY microservices/shared/ microservices/shared/
COPY microservices/appointments/ microservices/appointments/

# Build
WORKDIR /src/microservices/appointments/MedicSoft.Appointments.Api
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicSoft.Appointments.Api.dll"]
