# Patient Portal - CI/CD Documentation

## Overview

This document describes the Continuous Integration and Continuous Deployment (CI/CD) pipeline for the Patient Portal application.

## Pipeline Architecture

The CI/CD pipeline is implemented using GitHub Actions and consists of multiple stages for testing, building, and deploying both the backend (.NET 8 API) and frontend (Angular 20) applications.

**Workflow File:** `.github/workflows/patient-portal-ci.yml`

## Triggers

The pipeline is automatically triggered by:
- **Push events** to `main` or `develop` branches
- **Pull requests** targeting `main` or `develop` branches
- **Manual dispatch** via GitHub Actions UI

```yaml
on:
  push:
    branches: [ main, develop ]
    paths:
      - 'patient-portal-api/**'
      - 'frontend/patient-portal/**'
  pull_request:
    branches: [ main, develop ]
  workflow_dispatch:
```

## Pipeline Stages

### 1. Backend Tests

**Job:** `backend-tests`  
**Runner:** Ubuntu Latest  
**Purpose:** Run all backend tests and generate coverage reports

**Steps:**
1. Checkout code
2. Setup .NET 8 SDK
3. Restore NuGet dependencies
4. Build solution in Release mode
5. Run all tests with code coverage
6. Upload test results (TRX format)
7. Upload coverage reports (Cobertura XML)

**Test Categories:**
- Unit tests (15 tests)
- Integration tests (7 tests)
- Security tests (8 tests)
- Performance tests (5 tests)

**Expected Output:**
```
Test Run Successful.
Total tests: 28+
     Passed: 28+
```

### 2. Frontend Tests

**Job:** `frontend-tests`  
**Runner:** Ubuntu Latest  
**Purpose:** Run Angular unit tests with Karma/Jasmine

**Steps:**
1. Checkout code
2. Setup Node.js 20.x with npm cache
3. Install dependencies with `npm ci`
4. Run tests in headless Chrome with coverage
5. Upload test results and coverage

**Test Framework:**
- Karma test runner
- Jasmine for assertions
- Chrome Headless browser
- Istanbul for code coverage

### 3. Security Tests

**Job:** `security-tests`  
**Runner:** Ubuntu Latest  
**Dependencies:** `backend-tests`, `frontend-tests`  
**Purpose:** Run security-focused tests and vulnerability scanning

**Components:**
1. **Security Unit Tests**
   - JWT token validation
   - Password hashing verification
   - SQL injection prevention
   - Account lockout mechanisms

2. **OWASP Dependency Check**
   - Scans NuGet packages for known vulnerabilities
   - Generates HTML report
   - Fails build on high/critical vulnerabilities

**Configuration:**
```yaml
- name: OWASP Dependency Check
  uses: dependency-check/Dependency-Check_Action@main
  with:
    project: 'Patient Portal'
    path: 'patient-portal-api'
    format: 'HTML'
```

### 4. Build Backend (Docker)

**Job:** `build-backend`  
**Runner:** Ubuntu Latest  
**Dependencies:** `backend-tests`, `security-tests`  
**Purpose:** Build Docker image for the API

**Dockerfile:** `patient-portal-api/PatientPortal.Api/Dockerfile`

**Build Strategy:**
- Multi-stage build (SDK → Runtime)
- Non-root user for security
- Health check configured
- Optimized layer caching

**Image Details:**
- Base: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Exposed Port: 8080
- Health Check: `/health` endpoint
- Size: ~200MB (optimized)

**Artifacts:**
- Docker image saved as tar.gz
- Retained for 7 days
- Used by deployment jobs

### 5. Build Frontend (Docker)

**Job:** `build-frontend`  
**Runner:** Ubuntu Latest  
**Dependencies:** `frontend-tests`  
**Purpose:** Build Docker image for the frontend

**Dockerfile:** `frontend/patient-portal/Dockerfile`

**Build Strategy:**
- Multi-stage build (Node → nginx)
- Production-optimized Angular build
- Custom nginx configuration
- Non-root user for security

**nginx Configuration:**
- Security headers (CSP, X-Frame-Options, etc.)
- Gzip compression enabled
- Cache control for static assets
- Angular routing support (fallback to index.html)

**Image Details:**
- Base: `nginx:1.25-alpine`
- Exposed Port: 8080
- Health Check: `wget` on root
- Size: ~50MB (highly optimized)

### 6. Performance Tests

**Job:** `performance-tests`  
**Runner:** Ubuntu Latest  
**Dependencies:** `build-backend`  
**Trigger:** Only on push to `main` branch  
**Purpose:** Run load testing and performance benchmarks

**Tool:** k6 (Grafana)

**Tests:**
- Response time benchmarks
- Load testing (20 concurrent users)
- Endpoint availability checks
- Performance thresholds validation

**Thresholds:**
```javascript
thresholds: {
  http_req_duration: ['p(95)<500ms'],
  http_req_failed: ['rate<0.01'],
}
```

**Test Scenarios:**
1. Ramp up to 20 users in 30s
2. Sustain load for 1 minute
3. Ramp down to 0 in 20s

### 7. Deploy to Staging

**Job:** `deploy-staging`  
**Runner:** Ubuntu Latest  
**Dependencies:** `build-backend`, `build-frontend`, `performance-tests`  
**Trigger:** Push to `develop` branch  
**Environment:** staging  
**URL:** https://patient-portal-staging.medicwarehouse.com

**Process:**
1. Download Docker images
2. Load images from artifacts
3. Push to container registry
4. Deploy to staging environment
5. Run database migrations
6. Execute health checks
7. Create deployment summary

**Environment Variables:**
```bash
ASPNETCORE_ENVIRONMENT=Staging
ConnectionStrings__DefaultConnection=<staging-db>
JwtSettings__SecretKey=<staging-secret>
```

### 8. Deploy to Production

**Job:** `deploy-production`  
**Runner:** Ubuntu Latest  
**Dependencies:** `build-backend`, `build-frontend`, `performance-tests`  
**Trigger:** Push to `main` branch  
**Environment:** production (requires approval)  
**URL:** https://patient-portal.medicwarehouse.com

**Process:**
1. Download Docker images
2. Load images from artifacts
3. Push to container registry
4. Deploy to production environment
5. Run database migrations (with backup)
6. Execute comprehensive health checks
7. Enable monitoring and alerts
8. Create deployment summary

**Environment Variables:**
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<production-db>
JwtSettings__SecretKey=<production-secret>
Cors__AllowedOrigins__0=https://patient-portal.medicwarehouse.com
```

**Safety Measures:**
- Requires manual approval in GitHub
- Automatic rollback on health check failure
- Blue-green deployment strategy
- Database backup before migration

## Monitoring and Alerts

### Health Checks

**Backend Health Check:**
```
GET /health
Expected: 200 OK
Content: "Healthy"
```

**Frontend Health Check:**
```
GET /health
Expected: 200 OK
Content: "healthy"
```

### Deployment Summary

Each deployment creates a summary in GitHub Actions:

```markdown
## Production Deployment Summary
✅ Backend deployed successfully
✅ Frontend deployed successfully
📍 URL: https://patient-portal.medicwarehouse.com
⏱️ Deployment time: 5m 32s
📊 Tests passed: 48/48
🔒 Security checks: Passed
⚡ Performance: p95 < 500ms
```

## Quality Gates

The pipeline enforces the following quality gates:

1. **Test Coverage:** Minimum 70% code coverage
2. **Test Pass Rate:** 100% tests must pass
3. **Security:** No high/critical vulnerabilities
4. **Performance:** p95 response time < 500ms
5. **Build:** All builds must succeed

**Failure Actions:**
- Tests failing → Block merge
- Security issues → Block deployment
- Performance regression → Warning (non-blocking)

## Environment Variables

### Required Secrets

Configure in GitHub repository settings:

```yaml
# Required for all environments
POSTGRES_PASSWORD: <database-password>
JWT_SECRET_KEY: <jwt-secret-key>
SONAR_TOKEN: <sonarcloud-token>

# Optional
GITHUB_TOKEN: <automatically-provided>
```

### Environment-Specific Configuration

**Development:**
```yaml
ASPNETCORE_ENVIRONMENT: Development
JWT_EXPIRY_MINUTES: 60
```

**Staging:**
```yaml
ASPNETCORE_ENVIRONMENT: Staging
JWT_EXPIRY_MINUTES: 30
```

**Production:**
```yaml
ASPNETCORE_ENVIRONMENT: Production
JWT_EXPIRY_MINUTES: 15
```

## Local Development

### Run CI Tests Locally

**Backend:**
```bash
cd patient-portal-api
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

**Frontend:**
```bash
cd frontend/patient-portal
npm ci
npm test -- --watch=false --code-coverage
```

**E2E Tests:**
```bash
cd frontend/patient-portal
npm run e2e
```

### Build Docker Images Locally

**Backend:**
```bash
cd patient-portal-api
docker build -f PatientPortal.Api/Dockerfile -t patient-portal-api:local .
```

**Frontend:**
```bash
cd frontend/patient-portal
docker build -t patient-portal-frontend:local .
```

### Run with Docker Compose

```bash
cd patient-portal-api
docker-compose up --build
```

Access:
- Frontend: http://localhost:4202
- API: http://localhost:5001
- Swagger: http://localhost:5001/swagger

## Troubleshooting

### Common Issues

**1. Tests Failing in CI but Passing Locally**
- Check Node.js/npm versions match
- Verify .NET SDK version is 8.0.x
- Clear caches: `npm ci` instead of `npm install`

**2. Docker Build Failures**
- Verify Dockerfile paths are correct
- Check for missing dependencies
- Review build logs for specific errors

**3. Deployment Failures**
- Verify environment variables are set
- Check database connectivity
- Review health check logs
- Verify Docker images were uploaded

**4. Performance Test Failures**
- Review k6 output for specific metrics
- Check if thresholds are too strict
- Verify staging environment resources

### Debug Commands

```bash
# View workflow runs
gh run list --workflow patient-portal-ci.yml

# View specific run details
gh run view <run-id>

# Download artifacts
gh run download <run-id>

# Re-run failed jobs
gh run rerun <run-id> --failed
```

## Best Practices

1. **Always run tests locally before pushing**
2. **Use feature branches for development**
3. **Create PRs for code review**
4. **Wait for CI to pass before merging**
5. **Monitor deployment summaries**
6. **Review security scan results**
7. **Keep dependencies updated**
8. **Document breaking changes**

## Metrics and Reporting

### CI/CD Metrics

The pipeline tracks:
- Build success rate
- Test pass rate
- Code coverage trends
- Deployment frequency
- Mean time to recovery (MTTR)
- Change failure rate

### Reports Generated

1. **Test Results:** TRX and JUnit XML
2. **Code Coverage:** Cobertura XML, HTML
3. **Security Scan:** OWASP HTML report
4. **Performance:** k6 JSON results
5. **Deployment:** GitHub Deployment summary

## Future Enhancements

- [ ] SonarCloud integration for code quality
- [ ] Automated rollback on errors
- [ ] Slack/Teams notifications
- [ ] Performance trend monitoring
- [ ] Automated security patching
- [ ] Blue-green deployment
- [ ] Canary releases
- [ ] A/B testing support

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [OWASP Dependency Check](https://owasp.org/www-project-dependency-check/)
- [k6 Load Testing](https://k6.io/docs/)
- [Playwright E2E Testing](https://playwright.dev/)

---

**Last Updated:** Janeiro 2026  
**Version:** 1.0.0  
**Maintained by:** Omni Care Software Team
