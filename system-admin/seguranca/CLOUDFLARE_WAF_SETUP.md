# Cloudflare WAF Configuration for PrimeCare

This document describes the Web Application Firewall (WAF) configuration for PrimeCare using Cloudflare.

## Overview

The WAF provides protection against common web attacks including:
- SQL Injection
- Cross-Site Scripting (XSS)
- CSRF attacks
- DDoS protection
- Rate limiting
- Bad bot detection

## Cloudflare Plan Recommendation

**Recommended:** Business Plan ($200/month)
- Complete WAF with OWASP CRS
- Advanced DDoS protection
- Rate limiting
- Custom rules
- Bot management
- Certificate management

## WAF Rules Configuration

### 1. SQL Injection Protection

```yaml
rules:
  - name: "Block SQL Injection Attempts"
    expression: |
      (http.request.uri.query contains "UNION" or
       http.request.uri.query contains "SELECT" or
       http.request.uri.query contains "DROP TABLE" or
       http.request.uri.query contains "INSERT INTO" or
       http.request.body contains "' OR '1'='1" or
       http.request.body contains "1=1--" or
       http.request.body contains "admin'--")
    action: block
    priority: 1
```

### 2. XSS Protection

```yaml
  - name: "Block Cross-Site Scripting (XSS)"
    expression: |
      (http.request.uri.query contains "<script" or
       http.request.uri.query contains "javascript:" or
       http.request.uri.query contains "onerror=" or
       http.request.body contains "<script" or
       http.request.body contains "onclick=" or
       http.request.body contains "onload=")
    action: block
    priority: 2
```

### 3. Path Traversal Protection

```yaml
  - name: "Block Path Traversal"
    expression: |
      (http.request.uri.path contains "../" or
       http.request.uri.path contains "..\\")
    action: block
    priority: 3
```

### 4. Rate Limiting for Login

```yaml
  - name: "Rate Limit Login Attempts"
    expression: "http.request.uri.path eq '/api/auth/login'"
    action: rate_limit
    ratelimit:
      requests_per_period: 10
      period: 60  # seconds
      mitigation_timeout: 300  # 5 minutes
    priority: 4
```

### 5. Rate Limiting for API

```yaml
  - name: "Rate Limit API Requests"
    expression: "http.request.uri.path contains '/api/'"
    action: rate_limit
    ratelimit:
      requests_per_period: 100
      period: 60  # seconds
      mitigation_timeout: 60
    priority: 5
```

### 6. Bad Bot Detection

```yaml
  - name: "Block Known Attack Tools"
    expression: |
      (http.user_agent contains "sqlmap" or
       http.user_agent contains "nikto" or
       http.user_agent contains "nmap" or
       http.user_agent contains "masscan" or
       http.user_agent contains "python-requests" or
       http.user_agent contains "curl" or
       http.user_agent contains "wget")
    action: challenge  # Show CAPTCHA instead of blocking
    priority: 6
```

### 7. Country-Based Restrictions (Optional)

```yaml
  - name: "Geographic Restrictions"
    expression: |
      not (ip.geoip.country in {"BR" "US" "PT"})
    action: challenge
    priority: 7
```

### 8. HTTPS Enforcement

```yaml
  - name: "Enforce HTTPS"
    expression: "ssl eq false"
    action: redirect
    redirect_url: "https://${http.request.host}${http.request.uri.path}"
    priority: 8
```

## Configuration Steps

### 1. Add Domain to Cloudflare

1. Sign up at cloudflare.com
2. Add your domain (e.g., primecare.com.br)
3. Update nameservers at your domain registrar
4. Wait for DNS propagation (usually 2-24 hours)

### 2. Enable SSL/TLS

1. Go to SSL/TLS tab
2. Select "Full (Strict)" mode
3. Enable "Always Use HTTPS"
4. Enable "Automatic HTTPS Rewrites"

### 3. Configure WAF Rules

1. Go to Security → WAF
2. Enable OWASP Core Ruleset
3. Add custom rules as documented above
4. Set appropriate sensitivity level

### 4. Configure Rate Limiting

1. Go to Security → Rate Limiting
2. Create rules for:
   - Login endpoint: 10 requests/minute
   - API endpoints: 100 requests/minute
   - Password reset: 5 requests/hour

### 5. Configure Page Rules

1. Create caching rules for static assets
2. Set security level to "High" for admin paths
3. Enable "Browser Integrity Check"

### 6. Enable Bot Management

1. Go to Security → Bots
2. Enable bot fight mode
3. Configure bot detection sensitivity
4. Allow verified bots (Google, Bing)

## Monitoring and Alerts

### Cloudflare Dashboard

Monitor the following metrics:
- Blocked requests per hour
- Top blocked countries
- Top blocked attack types
- Rate limit violations

### Email Alerts

Configure alerts for:
- High volume of blocked requests (>100/min)
- DDoS attacks detected
- Certificate expiration warnings
- DNS changes

## Cost Breakdown

| Plan | Price/Month | Features |
|------|-------------|----------|
| Free | $0 | Basic DDoS, SSL |
| Pro | $20 | WAF basics, 20 page rules |
| Business | $200 | Full WAF, OWASP, Custom rules ⭐ |
| Enterprise | Custom | Advanced features, 24/7 support |

**Recommended for PrimeCare:** Business Plan ($200/month)

## Testing

### Test SQL Injection Protection

```bash
# Should be blocked
curl -X POST "https://api.primecare.com.br/api/patients?id=1' OR '1'='1"
```

### Test XSS Protection

```bash
# Should be blocked
curl -X POST "https://api.primecare.com.br/api/search?q=<script>alert('xss')</script>"
```

### Test Rate Limiting

```bash
# Run 15 times quickly - should start blocking after 10
for i in {1..15}; do
  curl -X POST "https://api.primecare.com.br/api/auth/login" \
    -d '{"username":"test","password":"test"}'
done
```

## Maintenance

### Monthly Tasks

- Review blocked requests log
- Update WAF rules based on new threats
- Check false positive rate
- Review rate limit thresholds

### Quarterly Tasks

- Security audit of WAF configuration
- Update OWASP ruleset
- Review and update custom rules
- Performance impact analysis

## References

- [Cloudflare WAF Documentation](https://developers.cloudflare.com/waf/)
- [OWASP Core Rule Set](https://owasp.org/www-project-modsecurity-core-rule-set/)
- [Cloudflare Rate Limiting](https://developers.cloudflare.com/waf/rate-limiting-rules/)

---

**Última Atualização:** 27 de Janeiro de 2026  
**Responsável:** Equipe de Segurança PrimeCare
