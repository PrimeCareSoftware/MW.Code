FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared library
COPY microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj microservices/shared/MedicSoft.Shared.Authentication/
RUN dotnet restore microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj

# Copy MedicalRecords API
COPY microservices/medicalrecords/MedicSoft.MedicalRecords.Api/MedicSoft.MedicalRecords.Api.csproj microservices/medicalrecords/MedicSoft.MedicalRecords.Api/
RUN dotnet restore microservices/medicalrecords/MedicSoft.MedicalRecords.Api/MedicSoft.MedicalRecords.Api.csproj

# Copy all source files
COPY microservices/shared/ microservices/shared/
COPY microservices/medicalrecords/ microservices/medicalrecords/

# Build
WORKDIR /src/microservices/medicalrecords/MedicSoft.MedicalRecords.Api
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicSoft.MedicalRecords.Api.dll"]
