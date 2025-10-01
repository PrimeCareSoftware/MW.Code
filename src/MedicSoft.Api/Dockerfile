# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/MedicSoft.Api/MedicSoft.Api.csproj", "src/MedicSoft.Api/"]
COPY ["src/MedicSoft.Application/MedicSoft.Application.csproj", "src/MedicSoft.Application/"]
COPY ["src/MedicSoft.Domain/MedicSoft.Domain.csproj", "src/MedicSoft.Domain/"]
COPY ["src/MedicSoft.Repository/MedicSoft.Repository.csproj", "src/MedicSoft.Repository/"]
COPY ["src/MedicSoft.CrossCutting/MedicSoft.CrossCutting.csproj", "src/MedicSoft.CrossCutting/"]

RUN dotnet restore "src/MedicSoft.Api/MedicSoft.Api.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/MedicSoft.Api"
RUN dotnet build "MedicSoft.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MedicSoft.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user
RUN addgroup --system --gid 1001 dotnetgroup \
    && adduser --system --uid 1001 --ingroup dotnetgroup dotnetuser

COPY --from=publish /app/publish .

# Change ownership of the app directory
RUN chown -R dotnetuser:dotnetgroup /app
USER dotnetuser

EXPOSE 8080
ENTRYPOINT ["dotnet", "MedicSoft.Api.dll"]