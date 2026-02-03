# 🚀 Telemedicine Production Deployment Guide

## 📋 Overview

This guide provides comprehensive instructions for deploying the MedicSoft Telemedicine microservice to production with all security and compliance requirements met.

## ✅ Pre-Deployment Checklist

### Security Requirements

- [ ] JWT authentication configured and tested
- [ ] Azure Key Vault or AWS KMS integrated for secrets management
- [ ] Rate limiting enabled and configured
- [ ] CORS restricted to production domains only
- [ ] Security headers configured (HSTS, CSP, X-Frame-Options)
- [ ] File storage backend (Azure Blob/AWS S3) configured
- [ ] SSL/TLS certificates installed and validated
- [ ] Database connection strings secured
- [ ] API keys and secrets stored in Key Vault
- [ ] Virus scanning integrated for file uploads

### Compliance Requirements

- [ ] CFM 2.314/2022 compliance verified
- [ ] LGPD compliance validated
- [ ] Data retention policies configured (20 years for recordings)
- [ ] Consent management tested
- [ ] Identity verification workflow validated
- [ ] Audit logging enabled
- [ ] Encryption at rest enabled for all sensitive data
- [ ] Encryption in transit (HTTPS) enforced

### Infrastructure Requirements

- [ ] PostgreSQL 14+ database provisioned
- [ ] Azure Blob Storage or AWS S3 bucket created
- [ ] Daily.co account and API key obtained
- [ ] Application Insights or equivalent monitoring configured
- [ ] Load balancer configured (if multi-instance)
- [ ] Backup strategy implemented
- [ ] Disaster recovery plan documented

### Testing Requirements

- [ ] All 46+ unit tests passing
- [ ] Integration tests executed successfully
- [ ] Security penetration testing completed
- [ ] Load testing performed (100+ concurrent users)
- [ ] Failover testing completed
- [ ] Backup and restore tested

## 🔧 Configuration

### 1. Application Settings

Create `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MedicSoft.Telemedicine": "Information"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/DatabaseConnectionString/)"
  },
  "FileStorage": {
    "Type": "AzureBlob",
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/StorageConnectionString/)",
    "Container": "telemedicine-files",
    "BaseUrl": "https://your-storage.blob.core.windows.net/telemedicine-files"
  },
  "Encryption": {
    "KeyVaultUrl": "https://your-keyvault.vault.azure.net/",
    "KeyName": "telemedicine-encryption-key"
  },
  "DailyCoVideo": {
    "ApiKey": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/DailyCoApiKey/)",
    "BaseUrl": "https://api.daily.co/v1"
  },
  "JWT": {
    "Issuer": "https://medicsoft.com.br",
    "Audience": "https://api.medicsoft.com.br",
    "SecretKey": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/JWTSecret/)",
    "ExpirationMinutes": 60
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "Window": "00:01:00",
    "QueueLimit": 10
  },
  "Cors": {
    "AllowedOrigins": [
      "https://medicsoft.com.br",
      "https://app.medicsoft.com.br",
      "https://admin.medicsoft.com.br"
    ]
  },
  "ApplicationInsights": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/AppInsightsConnectionString/)"
  }
}
```

### 2. Azure Key Vault Setup

```bash
# Create Key Vault
az keyvault create \
  --name medicsoft-telemedicine-kv \
  --resource-group medicsoft-prod \
  --location brazilsouth

# Add secrets
az keyvault secret set \
  --vault-name medicsoft-telemedicine-kv \
  --name DatabaseConnectionString \
  --value "Host=prod-db.postgres.database.azure.com;Database=telemedicine;Username=admin;Password=<strong-password>"

az keyvault secret set \
  --vault-name medicsoft-telemedicine-kv \
  --name StorageConnectionString \
  --value "DefaultEndpointsProtocol=https;AccountName=medstorage;AccountKey=<key>;EndpointSuffix=core.windows.net"

az keyvault secret set \
  --vault-name medicsoft-telemedicine-kv \
  --name DailyCoApiKey \
  --value "<your-daily-co-api-key>"

az keyvault secret set \
  --vault-name medicsoft-telemedicine-kv \
  --name JWTSecret \
  --value "<strong-random-secret-256-bits>"

# Grant app access to Key Vault
az keyvault set-policy \
  --name medicsoft-telemedicine-kv \
  --object-id <app-service-managed-identity-id> \
  --secret-permissions get list
```

### 3. Database Migration

```bash
# Apply migrations to production database
export ConnectionStrings__DefaultConnection="<production-connection-string>"

cd src/MedicSoft.Telemedicine.Infrastructure
dotnet ef database update --context TelemedicineDbContext --verbose

# Verify migrations
dotnet ef migrations list --context TelemedicineDbContext
```

### 4. Azure Blob Storage Setup

```bash
# Create storage account
az storage account create \
  --name medstorageprod \
  --resource-group medicsoft-prod \
  --location brazilsouth \
  --sku Standard_ZRS \
  --encryption-services blob \
  --min-tls-version TLS1_2

# Create containers
az storage container create \
  --name telemedicine-files \
  --account-name medstorageprod \
  --public-access off

az storage container create \
  --name identity-documents \
  --account-name medstorageprod \
  --public-access off

az storage container create \
  --name recordings \
  --account-name medstorageprod \
  --public-access off

# Enable soft delete
az storage blob service-properties delete-policy update \
  --account-name medstorageprod \
  --enable true \
  --days-retained 30
```

## 🐳 Docker Deployment

### 1. Build Docker Image

```bash
# Build production image
docker build -t medicsoft-telemedicine:latest \
  -f src/MedicSoft.Telemedicine.Api/Dockerfile .

# Tag for registry
docker tag medicsoft-telemedicine:latest \
  medicsoftcr.azurecr.io/telemedicine:v1.0.0

# Push to Azure Container Registry
docker push medicsoftcr.azurecr.io/telemedicine:v1.0.0
```

### 2. Docker Compose for Production

```yaml
version: '3.8'

services:
  telemedicine-api:
    image: medicsoftcr.azurecr.io/telemedicine:v1.0.0
    container_name: telemedicine-api
    restart: always
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    volumes:
      - /var/log/telemedicine:/app/logs
    networks:
      - medicsoft-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    depends_on:
      - postgres

  postgres:
    image: postgres:16-alpine
    container_name: telemedicine-db
    restart: always
    environment:
      - POSTGRES_DB=telemedicine
      - POSTGRES_USER=medicsoft
      - POSTGRES_PASSWORD_FILE=/run/secrets/db_password
    secrets:
      - db_password
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - /var/backups/postgres:/backups
    networks:
      - medicsoft-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U medicsoft"]
      interval: 10s
      timeout: 5s
      retries: 5

secrets:
  db_password:
    external: true

volumes:
  postgres_data:

networks:
  medicsoft-network:
    driver: bridge
```

## ☸️ Kubernetes Deployment

### 1. Deployment Configuration

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: telemedicine-api
  namespace: medicsoft-prod
spec:
  replicas: 3
  selector:
    matchLabels:
      app: telemedicine-api
  template:
    metadata:
      labels:
        app: telemedicine-api
    spec:
      containers:
      - name: api
        image: medicsoftcr.azurecr.io/telemedicine:v1.0.0
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        volumeMounts:
        - name: config
          mountPath: /app/appsettings.Production.json
          subPath: appsettings.Production.json
      volumes:
      - name: config
        configMap:
          name: telemedicine-config
---
apiVersion: v1
kind: Service
metadata:
  name: telemedicine-service
  namespace: medicsoft-prod
spec:
  selector:
    app: telemedicine-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: LoadBalancer
```

## 🔒 Security Hardening

### 1. Enable Security Headers

Add middleware in `Program.cs`:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(self), camera=(self)");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' https://api.daily.co;");
    
    // HSTS header
    if (context.Request.IsHttps)
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    }
    
    await next();
});
```

### 2. Configure Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var tenantId = context.Request.Headers["X-Tenant-Id"].ToString();
        
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: tenantId,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", cancellationToken: token);
    };
});

app.UseRateLimiter();
```

### 3. Production CORS Policy

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins(
                "https://medicsoft.com.br",
                "https://app.medicsoft.com.br",
                "https://admin.medicsoft.com.br")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

app.UseCors("ProductionPolicy");
```

## 📊 Monitoring and Observability

### 1. Application Insights Configuration

```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;
    options.EnableQuickPulseMetricStream = true;
});

// Custom telemetry
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
```

### 2. Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "database",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "sql", "postgresql" })
    .AddAzureBlobStorage(
        builder.Configuration["FileStorage:ConnectionString"],
        name: "blob-storage",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "storage", "azure" })
    .AddUrlGroup(
        new Uri("https://api.daily.co/v1/"),
        name: "dailyco-api",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "external", "video" });

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### 3. Structured Logging

```csharp
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .WriteTo.ApplicationInsights(
            builder.Configuration["ApplicationInsights:ConnectionString"],
            TelemetryConverter.Traces)
        .WriteTo.File(
            path: "/var/log/telemedicine/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30);
});
```

## 🔄 Backup and Disaster Recovery

### 1. Database Backup Strategy

```bash
# Automated daily backup script
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR=/var/backups/postgres
RETENTION_DAYS=30

# Create backup
pg_dump -h prod-db.postgres.database.azure.com \
  -U medicsoft \
  -d telemedicine \
  -F c \
  -f $BACKUP_DIR/telemedicine_$DATE.backup

# Upload to Azure Blob Storage
az storage blob upload \
  --account-name medstorageprod \
  --container-name backups \
  --name telemedicine_$DATE.backup \
  --file $BACKUP_DIR/telemedicine_$DATE.backup

# Remove old backups
find $BACKUP_DIR -name "telemedicine_*.backup" -mtime +$RETENTION_DAYS -delete
```

### 2. File Storage Backup

```bash
# Azure Blob Storage has built-in geo-redundancy
# Enable versioning for additional protection
az storage blob service-properties update \
  --account-name medstorageprod \
  --enable-versioning true
```

## 📈 Performance Optimization

### 1. Database Indexes

```sql
-- Critical indexes for performance
CREATE INDEX idx_telemedicine_consent_patient_tenant 
ON telemedicine_consents(patient_id, tenant_id) 
WHERE is_revoked = false;

CREATE INDEX idx_telemedicine_session_appointment 
ON telemedicine_sessions(appointment_id, tenant_id);

CREATE INDEX idx_identity_verification_user_tenant 
ON identity_verifications(user_id, tenant_id, verification_status);

CREATE INDEX idx_recordings_session 
ON telemedicine_recordings(session_id, tenant_id);
```

### 2. Connection Pooling

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db;Database=telemedicine;Username=app;Password=***;Pooling=true;MinPoolSize=10;MaxPoolSize=100;ConnectionIdleLifetime=60;ConnectionPruningInterval=10;"
  }
}
```

## 🚨 Incident Response

### 1. Monitoring Alerts

Configure alerts in Azure Monitor:

```yaml
alerts:
  - name: "High Error Rate"
    condition: "exceptions > 10 per 5 minutes"
    severity: "Critical"
    action: "Page on-call engineer"
  
  - name: "High Response Time"
    condition: "avg response time > 2 seconds for 5 minutes"
    severity: "Warning"
    action: "Send email to team"
  
  - name: "Low Availability"
    condition: "availability < 99% for 10 minutes"
    severity: "Critical"
    action: "Page on-call engineer + Create incident"
```

### 2. Rollback Procedure

```bash
# Rollback to previous version
kubectl rollout undo deployment/telemedicine-api -n medicsoft-prod

# Or rollback to specific revision
kubectl rollout undo deployment/telemedicine-api --to-revision=2 -n medicsoft-prod

# Verify rollback
kubectl rollout status deployment/telemedicine-api -n medicsoft-prod
```

## ✅ Post-Deployment Validation

### 1. Smoke Tests

```bash
# Health check
curl https://api.medicsoft.com.br/health

# Authentication test
curl -X POST https://api.medicsoft.com.br/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"***"}'

# Create session test
curl -X POST https://api.medicsoft.com.br/api/telemedicine/sessions \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: test-tenant" \
  -H "Content-Type: application/json" \
  -d '{"appointmentId":"test","patientId":"test"}'
```

### 2. Performance Baseline

```bash
# Run load test
artillery run load-test.yml

# Expected results:
# - p95 response time: < 200ms
# - p99 response time: < 500ms
# - Error rate: < 0.1%
# - Throughput: > 1000 req/s
```

## 📞 Support and Maintenance

### Escalation Contacts

- **L1 Support:** support@medicsoft.com.br
- **L2 Engineering:** engineering@medicsoft.com.br
- **On-Call:** +55 11 9xxxx-xxxx
- **Security Issues:** security@medicsoft.com.br

### Maintenance Windows

- **Regular Maintenance:** Sunday 02:00-04:00 BRT
- **Emergency Patches:** As needed with 2-hour notice
- **Major Updates:** Quarterly, scheduled 1 month in advance

---

**Last Updated:** January 29, 2026  
**Version:** 1.0.0  
**Maintainer:** Omni Care Software DevOps Team
