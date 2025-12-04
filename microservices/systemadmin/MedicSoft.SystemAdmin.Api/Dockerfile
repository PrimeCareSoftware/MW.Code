FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared library
COPY microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj microservices/shared/MedicSoft.Shared.Authentication/
RUN dotnet restore microservices/shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj

# Copy SystemAdmin API
COPY microservices/systemadmin/MedicSoft.SystemAdmin.Api/MedicSoft.SystemAdmin.Api.csproj microservices/systemadmin/MedicSoft.SystemAdmin.Api/
RUN dotnet restore microservices/systemadmin/MedicSoft.SystemAdmin.Api/MedicSoft.SystemAdmin.Api.csproj

# Copy all source files
COPY microservices/shared/ microservices/shared/
COPY microservices/systemadmin/ microservices/systemadmin/

# Build
WORKDIR /src/microservices/systemadmin/MedicSoft.SystemAdmin.Api
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicSoft.SystemAdmin.Api.dll"]
