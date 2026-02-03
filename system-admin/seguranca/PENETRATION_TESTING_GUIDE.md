# Penetration Testing Guide - Omni Care Software

This document provides the scope, requirements, and process for conducting professional penetration testing of the Omni Care healthcare management platform.

## Executive Summary

Penetration testing is a critical security measure to identify vulnerabilities before they can be exploited by malicious actors. This document outlines the scope, methodology, and expectations for professional security testing of Omni Care.

## Testing Scope

### In-Scope Systems

#### 1. Web Applications
- **Main Portal:** https://app.omnicare.com.br
- **Patient Portal:** https://portal.omnicare.com.br
- **Admin Dashboard:** https://admin.omnicare.com.br
- **Public Website:** https://www.omnicare.com.br

#### 2. API Endpoints
- **Main API:** https://api.omnicare.com.br/api/*
- **Patient Portal API:** https://api.omnicare.com.br/patient-portal/*
- **Authentication:** https://api.omnicare.com.br/api/auth/*
- **WebSocket:** wss://api.omnicare.com.br/ws

#### 3. Infrastructure
- SSL/TLS Configuration
- DNS Configuration
- HTTP Security Headers
- Cookie Security
- Session Management

### Out of Scope

- Physical security testing
- Social engineering attacks
- Denial of Service (DoS) attacks
- Testing during business hours (9h-18h BRT) without approval
- Production data modification
- Automated scanning without coordination

## Test Types and Methodology

### 1. OWASP Top 10 Testing

#### A01:2021 - Broken Access Control
- Test for unauthorized access to resources
- Privilege escalation attempts
- IDOR (Insecure Direct Object References)
- Missing function level access control

#### A02:2021 - Cryptographic Failures
- Weak encryption algorithms
- Improper key management
- Sensitive data exposure in transit
- Insecure password storage

#### A03:2021 - Injection
- SQL Injection
- NoSQL Injection
- Command Injection
- LDAP Injection

#### A04:2021 - Insecure Design
- Business logic flaws
- Missing rate limiting
- Insufficient input validation

#### A05:2021 - Security Misconfiguration
- Default credentials
- Unnecessary features enabled
- Improper error handling
- Missing security headers

#### A06:2021 - Vulnerable Components
- Outdated libraries and frameworks
- Known CVEs in dependencies

#### A07:2021 - Authentication Failures
- Weak password policies
- Credential stuffing
- Session fixation
- Broken JWT implementation

#### A08:2021 - Software and Data Integrity
- Insecure deserialization
- CI/CD pipeline security
- Auto-update mechanisms

#### A09:2021 - Logging and Monitoring Failures
- Insufficient logging
- Log injection
- Missing security alerts

#### A10:2021 - Server-Side Request Forgery
- SSRF vulnerabilities
- Internal network access

### 2. API Security Testing

#### Authentication and Authorization
- JWT token manipulation
- API key leakage
- OAuth2 flow vulnerabilities
- Refresh token handling

#### Data Validation
- Input validation bypass
- Mass assignment
- Parameter pollution
- Request smuggling

#### Rate Limiting
- API abuse testing
- Rate limit bypass
- Resource exhaustion

### 3. Healthcare-Specific Testing

#### LGPD Compliance
- Data access controls
- Patient data protection
- Consent management
- Data portability

#### Medical Data Security
- PHI (Protected Health Information) encryption
- Access logging
- Data anonymization
- Backup security

## Testing Timeline

### Phase 1: Reconnaissance (Week 1)
- Information gathering
- Attack surface mapping
- Technology stack identification
- Vulnerability scanning

### Phase 2: Vulnerability Assessment (Week 2)
- Manual testing of identified vulnerabilities
- Logic flaw identification
- Security misconfiguration testing

### Phase 3: Exploitation (Week 3)
- Proof of Concept (PoC) development
- Impact assessment
- Privilege escalation attempts

### Phase 4: Reporting (Week 4)
- Detailed vulnerability documentation
- Executive summary
- Remediation recommendations
- Retest coordination

## Deliverables

### 1. Executive Summary
- High-level findings
- Business impact analysis
- Risk assessment
- Strategic recommendations

### 2. Technical Report
- Detailed vulnerability descriptions
- Proof of Concept (PoC) code
- CVSS v3.1 scores
- Step-by-step reproduction steps
- Screenshots and evidence

### 3. Vulnerability Database
- Structured vulnerability list
- Severity ratings
- Affected components
- Remediation priorities

### 4. Remediation Report
- Specific fix recommendations
- Code examples
- Configuration changes
- Best practices

### 5. Retest Results
- Validation of fixes
- Residual risk assessment
- Final security posture

## CVSS Scoring System

Vulnerabilities will be scored using CVSS v3.1:

| Score Range | Severity | Priority |
|-------------|----------|----------|
| 9.0 - 10.0 | Critical | P0 - Immediate |
| 7.0 - 8.9 | High | P1 - Within 7 days |
| 4.0 - 6.9 | Medium | P2 - Within 30 days |
| 0.1 - 3.9 | Low | P3 - Within 90 days |
| 0.0 | Informational | P4 - Future consideration |

## Testing Environment

### Preferred Environment
- Staging environment with production-like data (anonymized)
- Isolated from production
- Full access to logs and monitoring

### Production Testing
If production testing is required:
- Testing window: Off-peak hours (22h-06h BRT)
- Rate-limited testing only
- Continuous coordination with DevOps team
- Immediate rollback plan

## Communication Protocol

### Points of Contact

**Primary Contact:**
- Name: [Security Team Lead]
- Email: security@omnicare.com.br
- Phone: +55 (XX) XXXXX-XXXX
- Slack: #security-team

**Technical Contact:**
- Name: [DevOps Lead]
- Email: devops@omnicare.com.br
- Slack: #devops-alerts

### Escalation Process

**Critical Findings:**
1. Immediate notification via phone/Slack
2. Email report within 2 hours
3. Emergency meeting within 4 hours

**High Findings:**
1. Slack notification within 1 hour
2. Email report within 24 hours
3. Meeting scheduled within 48 hours

**Medium/Low Findings:**
1. Added to daily status report
2. Discussed in weekly security meeting

### Daily Status Updates

- Daily summary email at 18h BRT
- Brief description of testing activities
- Any findings discovered
- Next day's testing plan

## Recommended Pentesting Firms (Brazil)

### 1. Morphus Security
- **Website:** www.morphussecurity.com
- **Specialization:** Full-stack security testing
- **Pricing:** R$ 25,000 - R$ 40,000
- **Duration:** 4-6 weeks
- **Pros:** Comprehensive reports, excellent healthcare experience
- **Cons:** Higher cost

### 2. Clavis Security
- **Website:** www.clavissecurity.com
- **Specialization:** Web/API security
- **Pricing:** R$ 15,000 - R$ 30,000
- **Duration:** 3-4 weeks
- **Pros:** Competitive pricing, good technical depth
- **Cons:** Limited retest support

### 3. Tempest Security
- **Website:** www.tempest.com.br
- **Specialization:** Enterprise security
- **Pricing:** R$ 30,000 - R$ 50,000
- **Duration:** 4-8 weeks
- **Pros:** Enterprise-grade, compliance focus
- **Cons:** Premium pricing

### 4. Conviso
- **Website:** www.convisoappsec.com
- **Specialization:** Application Security + DevSecOps
- **Pricing:** R$ 25,000 - R$ 45,000
- **Duration:** 4-6 weeks
- **Pros:** AppSec + DevSecOps integration
- **Cons:** Longer engagement timeline

## Cost Estimation

### Basic Package (R$ 15,000 - R$ 20,000)
- 80-100 hours of testing
- OWASP Top 10 coverage
- Web application testing
- API security basics
- Technical report
- No retest included

### Complete Package (R$ 30,000 - R$ 35,000) ⭐ Recommended
- 160-200 hours of testing
- Comprehensive OWASP coverage
- Web + API + Infrastructure
- Healthcare-specific testing
- Detailed reports (executive + technical)
- One retest included
- Video call presentations

### Enterprise Package (R$ 50,000+)
- 300+ hours of testing
- Comprehensive security assessment
- Multiple retests
- Quarterly testing program
- DevSecOps integration
- Security training
- Ongoing consultation

## Post-Testing Activities

### 1. Remediation Phase
- Prioritize findings by severity
- Assign to development teams
- Track progress in issue tracker
- Regular status meetings

### 2. Retest
- Schedule after remediation complete
- Verify all critical/high findings fixed
- Document any remaining issues
- Final sign-off

### 3. Continuous Improvement
- Update security policies
- Implement lessons learned
- Enhance monitoring and alerts
- Security awareness training

### 4. Compliance Documentation
- Store pentest reports securely
- Use for LGPD compliance
- Reference in security audits
- Update risk register

## Success Criteria

The penetration test will be considered successful when:

- [ ] Zero critical vulnerabilities remaining
- [ ] All high vulnerabilities remediated or accepted as risk
- [ ] Medium vulnerabilities with remediation plan
- [ ] Comprehensive documentation provided
- [ ] Retest validates fixes
- [ ] Team trained on findings

## Legal and Compliance

### Testing Agreement

A formal testing agreement must include:
- Scope definition
- Testing timeline
- Allowed testing methods
- Data handling requirements
- NDA (Non-Disclosure Agreement)
- Liability limitations
- Results ownership

### Data Protection

- All testing must comply with LGPD
- No real patient data in testing environment
- Secure handling of discovered vulnerabilities
- Encrypted communication of findings

### Insurance

Pentesting firm should have:
- Professional liability insurance
- Cyber liability coverage
- Minimum R$ 1M coverage

## Appendix A: Test Checklist

### Authentication
- [ ] Password complexity requirements
- [ ] Brute force protection
- [ ] Account lockout mechanism
- [ ] MFA implementation
- [ ] Session timeout
- [ ] Secure password reset
- [ ] JWT security

### Authorization
- [ ] Role-based access control
- [ ] Privilege escalation prevention
- [ ] Horizontal access control
- [ ] Vertical access control
- [ ] API authorization

### Data Protection
- [ ] Data encryption at rest
- [ ] Data encryption in transit
- [ ] Sensitive data exposure
- [ ] PII handling
- [ ] Medical data protection

### Infrastructure
- [ ] SSL/TLS configuration
- [ ] HTTP security headers
- [ ] Cookie security flags
- [ ] CORS configuration
- [ ] DNS security

### Application Security
- [ ] Input validation
- [ ] Output encoding
- [ ] SQL injection prevention
- [ ] XSS prevention
- [ ] CSRF protection
- [ ] File upload security

## References

- [OWASP Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)
- [NIST SP 800-115 - Technical Guide to Information Security Testing](https://csrc.nist.gov/publications/detail/sp/800-115/final)
- [PTES - Penetration Testing Execution Standard](http://www.pentest-standard.org/)
- [LGPD - Lei Geral de Proteção de Dados](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)

---

**Document Version:** 1.0  
**Last Updated:** 27 de Janeiro de 2026  
**Owner:** Omni Care Security Team  
**Review Cycle:** Annually or after significant changes
