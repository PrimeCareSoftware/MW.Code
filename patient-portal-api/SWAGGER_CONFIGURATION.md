# Swagger Configuration - Patient Portal API

## Overview

The Patient Portal API provides Swagger/OpenAPI documentation that is configurable and enabled by default in all environments.

## Configuration

Swagger can be enabled or disabled via the `appsettings.json` configuration:

```json
{
  "SwaggerSettings": {
    "Enabled": true
  }
}
```

## Default Behavior

- **Development**: Swagger is enabled by default
- **Production**: Swagger is enabled by default but can be disabled
- **All Environments**: Swagger is available unless explicitly disabled

## Accessing Swagger

When enabled, Swagger UI is accessible at:

- **Root URL**: `http://localhost:5101/` (Development)
- **Production**: `https://your-domain.com/`

Swagger JSON specification is available at:
- `/swagger/v1/swagger.json`

## Security Considerations

### Production Deployment

When deploying to production, consider the following security measures:

1. **Network-Level Restrictions**
   - Use firewall rules to restrict Swagger access to specific IP ranges
   - Deploy behind a VPN for internal-only access
   - Use reverse proxy (nginx, IIS) to restrict access

2. **Disable Swagger**
   - Set `SwaggerSettings:Enabled` to `false` in `appsettings.Production.json` if documentation should not be publicly accessible

3. **Authentication**
   - Swagger already includes JWT Bearer authentication configuration
   - All protected endpoints require valid JWT tokens
   - No sensitive data is exposed through Swagger schemas

### Environment Variables

You can also control Swagger via environment variable:

```bash
export SwaggerSettings__Enabled=false
dotnet run
```

Or in Docker:

```dockerfile
ENV SwaggerSettings__Enabled=false
```

## Example Configurations

### Disable Swagger in Production

**appsettings.Production.json:**
```json
{
  "SwaggerSettings": {
    "Enabled": false
  }
}
```

### Enable Only in Development

**appsettings.json:**
```json
{
  "SwaggerSettings": {
    "Enabled": false
  }
}
```

**appsettings.Development.json:**
```json
{
  "SwaggerSettings": {
    "Enabled": true
  }
}
```

## Troubleshooting

### Swagger Page is Blank

If the Swagger page appears blank:

1. Check that `SwaggerSettings:Enabled` is `true`
2. Verify the API is running and accessible
3. Check browser console for errors
4. Ensure `/swagger/v1/swagger.json` is accessible

### Cannot Access Swagger

If you cannot access Swagger:

1. Check the `SwaggerSettings:Enabled` configuration
2. Verify you're accessing the correct URL (root or `/swagger`)
3. Check firewall and network policies
4. Review application logs for errors

## Related Documentation

- [DEVELOPER_QUICKSTART.md](./DEVELOPER_QUICKSTART.md) - Quick setup guide
- [README.md](./README.md) - Main documentation
- [TROUBLESHOOTING_FAQ.md](./TROUBLESHOOTING_FAQ.md) - Common issues

## Change History

- **2026-02-04**: Added configurable Swagger support for all environments
- **Previous**: Swagger was only available in Development environment
