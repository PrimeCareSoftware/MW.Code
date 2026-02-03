# SIEM Configuration - ELK Stack for Omni Care

This document describes the Security Information and Event Management (SIEM) setup using the ELK Stack (Elasticsearch, Logstash, Kibana).

## Overview

The SIEM provides:
- Centralized log management
- Real-time threat detection
- Security event correlation
- Automated alerting
- Compliance reporting
- Forensic analysis capabilities

## Architecture

```
┌─────────────────┐
│  Omni Care API  │
│   .NET Core     │
└────────┬────────┘
         │ Logs
         ▼
┌─────────────────┐
│    Filebeat     │ ◄─── Collects logs from files/containers
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│    Logstash     │ ◄─── Processes, enriches, filters logs
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Elasticsearch   │ ◄─── Stores and indexes logs
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│     Kibana      │ ◄─── Visualizes and alerts
└─────────────────┘
```

## Quick Start

### 1. Start ELK Stack

```bash
# Set Elasticsearch password
export ELASTIC_PASSWORD="your_secure_password_here"

# Start all services
docker-compose -f docker-compose.elk.yml up -d

# Wait for services to be healthy (2-3 minutes)
docker-compose -f docker-compose.elk.yml ps
```

### 2. Access Kibana

1. Open browser: http://localhost:5601
2. Login with:
   - Username: `elastic`
   - Password: `your_secure_password_here`

### 3. Configure Index Patterns

1. Go to Management → Stack Management → Index Patterns
2. Create index patterns:
   - `omnicare-logs-*`
   - `omnicare-security-critical-*`
   - `omnicare-failed-logins-*`

## Security Dashboards

### Dashboard 1: Security Overview

**Visualizations:**
1. **Failed Logins (24h)** - Metric
2. **Login Attempts Timeline** - Line chart
3. **Top Failed Login IPs** - Data table
4. **Geographic Login Map** - Map
5. **Account Lockouts** - Metric
6. **MFA Events** - Donut chart

**Kibana Configuration:**
```json
{
  "title": "Security Overview",
  "description": "Real-time security monitoring dashboard",
  "timeRestore": true,
  "timeFrom": "now-24h",
  "timeTo": "now",
  "refreshInterval": {
    "value": 60000,
    "pause": false
  }
}
```

### Dashboard 2: Authentication Monitoring

**Visualizations:**
1. **Successful vs Failed Logins** - Bar chart
2. **Authentication Methods** - Pie chart
3. **MFA Usage Rate** - Gauge
4. **Session Duration** - Histogram
5. **Login Failures by User** - Data table

### Dashboard 3: Threat Detection

**Visualizations:**
1. **Attack Attempts** - Metric (critical)
2. **Attack Types** - Pie chart
3. **Blocked IPs** - Data table
4. **Suspicious Activity Timeline** - Area chart
5. **WAF Blocks** - Metric

## Automated Alerts

### Alert 1: Multiple Failed Logins

**Trigger:** 5+ failed login attempts from same IP within 5 minutes

**Configuration:**
```json
{
  "name": "Multiple Failed Login Attempts",
  "schedule": {
    "interval": "5m"
  },
  "trigger": {
    "condition": {
      "script": {
        "source": "ctx.results[0].hits.total.value > 5"
      }
    }
  },
  "actions": {
    "email": {
      "to": ["security@omnicare.com.br"],
      "subject": "🚨 Multiple Failed Login Attempts Detected",
      "body": "{{ctx.results[0].hits.total.value}} failed login attempts detected from IP: {{ctx.payload.aggregations.by_ip.buckets.0.key}}"
    }
  }
}
```

### Alert 2: Account Lockout

**Trigger:** Any account lockout event

**Configuration:**
```json
{
  "name": "Account Lockout Detected",
  "schedule": {
    "interval": "1m"
  },
  "trigger": {
    "condition": {
      "script": {
        "source": "ctx.results[0].hits.total.value > 0"
      }
    }
  },
  "query": {
    "match": {
      "tags": "account_lockout"
    }
  },
  "actions": {
    "email": {
      "to": ["security@omnicare.com.br"],
      "subject": "⚠️ Account Lockout - Immediate Action Required",
      "priority": "high"
    },
    "webhook": {
      "url": "https://api.omnicare.com.br/webhooks/security-alert"
    }
  }
}
```

### Alert 3: Unusual Login Location

**Trigger:** Login from country different from user's usual location

**Configuration:**
```json
{
  "name": "Unusual Login Location",
  "schedule": {
    "interval": "5m"
  },
  "trigger": {
    "condition": {
      "script": {
        "source": "ctx.results[0].hits.total.value > 0"
      }
    }
  },
  "actions": {
    "email": {
      "to": ["security@omnicare.com.br"],
      "subject": "🌍 Unusual Login Location Detected"
    }
  }
}
```

### Alert 4: Critical Security Event

**Trigger:** Any event tagged as "critical"

**Configuration:**
```json
{
  "name": "Critical Security Event",
  "schedule": {
    "interval": "1m"
  },
  "trigger": {
    "condition": {
      "script": {
        "source": "ctx.results[0].hits.total.value > 0"
      }
    }
  },
  "query": {
    "match": {
      "tags": "critical"
    }
  },
  "actions": {
    "email": {
      "to": ["security@omnicare.com.br", "admin@omnicare.com.br"],
      "subject": "🚨 CRITICAL SECURITY EVENT",
      "priority": "urgent"
    },
    "slack": {
      "message": "Critical security event detected! Check Kibana immediately."
    }
  }
}
```

## Log Retention Policy

### Storage Management

**Index Lifecycle Management (ILM):**

```json
{
  "policy": {
    "phases": {
      "hot": {
        "actions": {
          "rollover": {
            "max_size": "50GB",
            "max_age": "7d"
          }
        }
      },
      "warm": {
        "min_age": "7d",
        "actions": {
          "readonly": {},
          "forcemerge": {
            "max_num_segments": 1
          }
        }
      },
      "cold": {
        "min_age": "30d",
        "actions": {
          "freeze": {}
        }
      },
      "delete": {
        "min_age": "90d",
        "actions": {
          "delete": {}
        }
      }
    }
  }
}
```

**Retention Rules:**
- Hot data: 0-7 days (fast SSD)
- Warm data: 7-30 days (compressed, read-only)
- Cold data: 30-90 days (archived)
- Deleted: After 90 days

## Integration with .NET Application

### Serilog Configuration

Add to `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "Http",
        "Args": {
          "requestUri": "http://localhost:5000",
          "textFormatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

### Log Security Events

```csharp
// In your AuthService or SecurityService
_logger.LogWarning("Failed login attempt from {IpAddress} for user {Username}",
    ipAddress, username);

_logger.LogCritical("Account locked for user {UserId} after {FailedAttempts} failed attempts",
    userId, failedAttempts);

_logger.LogInformation("Successful login with MFA for user {UserId} from {IpAddress}",
    userId, ipAddress);
```

## Monitoring and Maintenance

### Daily Tasks

- Check Kibana dashboards
- Review critical alerts
- Verify log ingestion rate

### Weekly Tasks

- Review failed login patterns
- Analyze attack attempts
- Update alert thresholds
- Check disk space usage

### Monthly Tasks

- Generate compliance reports
- Review and update dashboards
- Optimize index lifecycle
- Performance tuning

## Cost Estimation

**Infrastructure Costs (Self-Hosted):**

| Resource | Specification | Monthly Cost |
|----------|---------------|--------------|
| Server | 4 CPU, 16GB RAM, 500GB SSD | R$ 300-500 |
| Backup Storage | 1TB | R$ 50 |
| **Total** | | **R$ 350-550/month** |

**Alternative: Elastic Cloud**
- Basic: $95/month (~R$ 475)
- Standard: $200/month (~R$ 1,000)
- Enterprise: Custom pricing

**Recommendation:** Self-hosted for cost optimization (R$ 400/month avg)

## Troubleshooting

### Elasticsearch won't start

```bash
# Check logs
docker logs omnicare-elasticsearch

# Increase vm.max_map_count
sudo sysctl -w vm.max_map_count=262144
```

### Logstash not receiving logs

```bash
# Test connectivity
telnet localhost 5044

# Check Logstash logs
docker logs omnicare-logstash
```

### Kibana connection error

```bash
# Verify Elasticsearch is running
curl http://localhost:9200/_cluster/health

# Check Kibana logs
docker logs omnicare-kibana
```

## Security Best Practices

1. **Change default passwords** immediately
2. **Enable TLS/SSL** for all connections
3. **Use role-based access control** (RBAC)
4. **Enable audit logging** for Elasticsearch
5. **Regular backups** of Elasticsearch data
6. **Monitor disk space** to prevent data loss
7. **Restrict network access** to ELK services

## References

- [Elastic Stack Documentation](https://www.elastic.co/guide/index.html)
- [Logstash Reference](https://www.elastic.co/guide/en/logstash/current/index.html)
- [Kibana Guide](https://www.elastic.co/guide/en/kibana/current/index.html)
- [Elasticsearch Security](https://www.elastic.co/guide/en/elasticsearch/reference/current/security-settings.html)

---

**Última Atualização:** 27 de Janeiro de 2026  
**Responsável:** Equipe de Segurança Omni Care
