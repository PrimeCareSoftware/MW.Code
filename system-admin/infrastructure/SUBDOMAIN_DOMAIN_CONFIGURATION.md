# Subdomain Domain Configuration Guide

## Overview

The subdomain feature now supports **any domain configuration**, making it easy to change the company domain or use different domains for development, testing, and production environments.

## Problem Solved

**Before:** The subdomain feature was hardcoded to work only with `.medicwarehouse.com` domain.

**After:** The system now supports any URL format through simple environment configuration:
- `http://clinica01.localhost:4200` (development)
- `http://clinica01.com.br` (custom domain)
- `http://clinica01.medicwarehouse.com.br` (original domain)
- `http://clinica01.yourdomain.com` (any other domain)

## How It Works

### Backend (C# - Already Domain-Agnostic)

The `TenantResolutionMiddleware` automatically extracts the subdomain from **any** URL format:

```csharp
// Supports all these formats:
// subdomain.localhost -> "subdomain"
// subdomain.domain.com -> "subdomain"
// subdomain.domain.com.br -> "subdomain"
```

**Key Features:**
- Skips plain `localhost` and IP addresses
- Supports 2+ part domains (e.g., `subdomain.localhost`, `subdomain.com.br`)
- Excludes `www` subdomain automatically
- Case-insensitive

### Frontend (Angular)

The `TenantResolverService` uses the same logic:

```typescript
// Examples of supported formats:
// clinica01.localhost -> "clinica01"
// clinica01.com.br -> "clinica01"
// clinica01.medicwarehouse.com.br -> "clinica01"
```

## Configuration

### 1. Development Environment

Edit `frontend/medicwarehouse-app/src/environments/environment.ts`:

```typescript
export const environment = {
  // ... other config ...
  tenant: {
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger'],
    domainSuffix: 'localhost:4200'  // ← Change this for local development
  }
};
```

Edit `frontend/mw-system-admin/src/environments/environment.ts`:

```typescript
export const environment = {
  // ... other config ...
  tenant: {
    domainSuffix: 'localhost:4200'  // ← Change this for local development
  }
};
```

### 2. Production Environment

Edit `frontend/medicwarehouse-app/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  // ... other config ...
  tenant: {
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger'],
    domainSuffix: 'medicwarehouse.com'  // ← Change this to your production domain
  }
};
```

Edit `frontend/mw-system-admin/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  // ... other config ...
  tenant: {
    domainSuffix: 'medicwarehouse.com'  // ← Change this to your production domain
  }
};
```

## Example Configurations

### Example 1: Using localhost with port

```typescript
tenant: {
  domainSuffix: 'localhost:4200'
}
```

Result: Subdomains display as `clinica01.localhost:4200`

### Example 2: Using custom Brazilian domain

```typescript
tenant: {
  domainSuffix: 'minhaempresa.com.br'
}
```

Result: Subdomains display as `clinica01.minhaempresa.com.br`

### Example 3: Using simple .com domain

```typescript
tenant: {
  domainSuffix: 'mycompany.com'
}
```

Result: Subdomains display as `clinica01.mycompany.com`

## UI Impact

The domain suffix configuration affects the following UI elements:

### System Admin - Subdomain List

**Before:**
```
Subdomain: clinica01.medicwarehouse.com
```

**After (configurable):**
```
Subdomain: clinica01.localhost:4200        (dev)
Subdomain: clinica01.yourdomain.com        (prod)
```

### System Admin - Create Subdomain Modal

**Before:**
```
[minhaclinica] .medicwarehouse.com
```

**After (configurable):**
```
[minhaclinica] .localhost:4200        (dev)
[minhaclinica] .yourdomain.com        (prod)
```

## DNS Configuration

### Development (localhost)

No DNS configuration needed. Just access:
- `http://clinica01.localhost:4200`

**Note:** Some browsers may require `/etc/hosts` configuration for subdomain.localhost to work. Modern browsers typically support it natively.

### Production

Configure DNS with wildcard subdomain:

```
Type: A or CNAME
Host: *
Value: Your server IP or domain
TTL: 3600
```

Examples:
- `*.yourdomain.com` → Your server IP
- `*.medicwarehouse.com` → Your server IP
- `*.minhaempresa.com.br` → Your server IP

## Testing

### Test with Different URLs

1. **Localhost with subdomain:**
   ```
   http://clinica01.localhost:4200/login
   ```

2. **Custom domain:**
   ```
   http://clinica01.yourdomain.com/login
   ```

3. **Brazilian domain:**
   ```
   http://clinica01.medicwarehouse.com.br/login
   ```

### Verify Subdomain Extraction

Check browser console logs to verify tenant resolution:

```
Extracted subdomain from host: clinica01
Resolved subdomain clinica01 to tenantId: abc-123-def
```

## Migration Steps

To change from `medicwarehouse.com` to a new domain:

1. **Update environment files** (as shown above)
2. **Configure DNS** with wildcard subdomain
3. **Update SSL certificates** to support wildcard subdomain
4. **Test thoroughly** in staging environment
5. **Deploy** to production
6. **Inform users** of new domain (if needed)

## Backend Configuration

No backend configuration changes needed! The middleware is already fully domain-agnostic and will work with any domain.

## Important Notes

- The subdomain must be at least 3 characters long
- Only lowercase letters, numbers, and hyphens are allowed
- Cannot start or end with a hyphen
- The subdomain `www` is automatically excluded
- IP addresses and plain `localhost` don't support subdomain extraction

## Troubleshooting

### Issue: Subdomain not being detected

**Solution:** Check that:
1. URL format is correct: `subdomain.domain.tld`
2. Not using plain `localhost` without subdomain
3. Browser cache is cleared
4. Console shows no errors

### Issue: SSL certificate errors with subdomain

**Solution:** Ensure SSL certificate supports wildcard domains:
- `*.yourdomain.com` or `*.medicwarehouse.com.br`

### Issue: DNS not resolving subdomain

**Solution:** 
1. Verify DNS wildcard record is configured
2. Wait for DNS propagation (up to 24-48 hours)
3. Test with `nslookup` or `dig` command

## Support

For more information about subdomain functionality, see:
- `src/MedicSoft.Api/Middleware/TenantResolutionMiddleware.cs`
- `frontend/medicwarehouse-app/src/app/services/tenant-resolver.service.ts`
- `frontend/mw-system-admin/src/app/pages/subdomains/subdomains-list.ts`
