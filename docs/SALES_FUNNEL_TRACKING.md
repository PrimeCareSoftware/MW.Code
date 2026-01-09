# Sales Funnel Tracking System

## 📋 Overview

This system implements comprehensive sales funnel tracking for the PrimeCare Software registration process. It monitors customer journey through all registration steps, identifies drop-off points, and provides actionable insights for improving conversion rates.

## 🎯 Features

### Backend
- **Unauthenticated Tracking API**: Track customer events without requiring authentication
- **Step-by-Step Monitoring**: Track all 6 registration steps
- **Data Capture**: Store relevant form data at each step (sanitized)
- **Conversion Tracking**: Mark successful registrations
- **IP & User Agent Tracking**: For analytics while respecting LGPD
- **UTM Parameter Collection**: Track marketing campaign effectiveness
- **Repository Pattern**: Clean architecture with EF Core
- **Comprehensive Statistics**: Conversion rates, drop-off analysis, session details

### Frontend (mw-site)
- **Automatic Tracking**: Seamless integration in registration flow
- **Data Sanitization**: Removes passwords and sensitive information
- **Non-Blocking**: Tracking failures don't affect user experience
- **Session Management**: Persistent tracking across page refreshes
- **Metadata Collection**: Screen size, UTM parameters, timestamps

### Admin Dashboard (mw-system-admin)
- **Visual Funnel Representation**: See completion rates at each step
- **Incomplete Sessions**: Identify potential customers who abandoned
- **Session Details**: Deep dive into individual customer journeys
- **Date Filters**: Analyze specific time periods
- **Export Functionality**: Download data for external analysis

## 🏗️ Architecture

### Database Schema

**SalesFunnelMetrics Table**:
```sql
- Id (Guid)
- SessionId (string) - Groups events from same registration attempt
- Step (int) - Registration step number (1-6)
- StepName (string) - Human-readable step name
- Action (string) - 'entered', 'completed', 'abandoned'
- CapturedData (json) - Sanitized form data
- PlanId (string) - Selected subscription plan
- IpAddress (string) - User IP for analytics
- UserAgent (string) - Browser/device information
- Referrer (string) - Source URL
- ClinicId (Guid?) - Set on conversion
- OwnerId (Guid?) - Set on conversion
- IsConverted (bool) - Registration completed
- DurationMs (long?) - Time spent on step
- Metadata (json) - UTM parameters, etc.
- CreatedAt (DateTime)
```

### API Endpoints

#### Public (Unauthenticated)
- `POST /api/salesfunnel/track` - Track a funnel event
- `POST /api/salesfunnel/convert` - Mark session as converted

#### Private (Authenticated - System Admin Only)
- `GET /api/salesfunnel/stats` - Get funnel statistics
- `GET /api/salesfunnel/incomplete` - Get incomplete sessions
- `GET /api/salesfunnel/session/{sessionId}` - Get session details
- `GET /api/salesfunnel/recent` - Get recent sessions

### Registration Steps

1. **Clinic Information**
   - Clinic name, CNPJ, phone, email
   
2. **Address**
   - Street, number, neighborhood, city, state, zip code
   
3. **Owner Information**
   - Owner name, CPF, phone, email
   
4. **Login Credentials**
   - Username, password
   
5. **Plan Selection**
   - Choose subscription plan
   
6. **Confirmation**
   - Accept terms and finalize

## 🔒 Privacy & Security (LGPD Compliance)

### Data Protection Measures

1. **Data Sanitization**
   - Passwords are never stored
   - CPF/CNPJ are masked in captured data
   - Sensitive fields are redacted

2. **Purpose Limitation**
   - Data used only for funnel optimization
   - Not shared with third parties
   - Automatically cleaned after analysis

3. **Transparency**
   - Users informed about data collection
   - Part of terms of service acceptance
   - Data retention policy documented

4. **Security**
   - Admin endpoints require authentication
   - Data encrypted in transit (HTTPS)
   - Access controlled by role

### Recommended Data Retention

- Keep metrics for 90 days for active analysis
- Archive older data or aggregate into summary statistics
- Implement automated cleanup jobs

## 📊 Metrics & KPIs

### Key Metrics Tracked

1. **Conversion Rate**
   - % of sessions that complete registration
   - Target: Industry average is 2-5% for SaaS

2. **Step Completion Rates**
   - % of users who complete each step
   - Identify major drop-off points

3. **Abandonment Rate**
   - % of users who leave at each step
   - Focus optimization efforts

4. **Time Per Step**
   - Average duration on each step
   - Identify friction points

5. **Traffic Source Analysis**
   - Which UTM sources convert best
   - ROI on marketing channels

### Industry Benchmarks

Based on best practices from tools like:
- **Mixpanel**: Event tracking and funnel analysis
- **Amplitude**: User behavior analytics
- **Google Analytics**: Enhanced ecommerce tracking
- **Hotjar**: Session recording and heatmaps

## 🚀 Usage Guide

### For Developers

#### Backend Integration

The tracking is automatically integrated in the `RegistrationController`:

```csharp
// Add sessionId to registration request
var registrationData = {
    ...model,
    sessionId: salesFunnelTracking.getSessionId()
};

// Conversion is automatically tracked on success
await _salesFunnelService.MarkConversionAsync(new MarkConversionDto {
    SessionId = request.SessionId,
    ClinicId = result.ClinicId.Value,
    OwnerId = result.OwnerId.Value
});
```

#### Frontend Integration

Tracking is handled by `SalesFunnelTrackingService`:

```typescript
// Track entering a step
salesFunnelTracking.trackStepEntered(step, capturedData, planId);

// Track completing a step
salesFunnelTracking.trackStepCompleted(step, capturedData, planId);

// Track abandonment
salesFunnelTracking.trackStepAbandoned(step, capturedData, planId);

// Clear on successful registration
salesFunnelTracking.clearSession();
```

### For System Administrators

1. **Access Dashboard**: Navigate to `/sales-metrics` in mw-system-admin
2. **Filter Data**: Select date range to analyze
3. **Analyze Funnel**: Review completion rates per step
4. **Review Incomplete Sessions**: Identify potential for follow-up
5. **Export Data**: Download for external analysis

### For Business Analysts

#### Key Questions to Answer

1. **Where do users drop off?**
   - Look at abandonment rates per step
   - Focus optimization on highest drop-off points

2. **Which marketing channels work best?**
   - Filter by UTM parameters
   - Compare conversion rates across sources

3. **What time periods perform best?**
   - Analyze by date range
   - Identify seasonal patterns

4. **Are there technical issues?**
   - Review incomplete sessions
   - Check for unusual patterns

## 🔧 Configuration

### Environment Variables

Frontend (`environment.ts`):
```typescript
export const environment = {
  apiUrl: 'https://api.medicwarehouse.com' // Backend API URL
};
```

Backend (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..." // PostgreSQL connection
  }
}
```

### Database Migration

Run the migration to create the table:

```bash
cd src/MedicSoft.Repository
dotnet ef database update --startup-project ../MedicSoft.Api
```

## 📈 Optimization Recommendations

### Based on Best Practices

1. **Reduce Form Fields**
   - Only ask for essential information early
   - Progressive disclosure for optional fields

2. **Clear Progress Indicators**
   - Show users where they are in the process
   - Set expectations for time required

3. **Inline Validation**
   - Real-time feedback on form errors
   - Reduce frustration and re-entry

4. **Social Proof**
   - Display testimonials near drop-off points
   - Show number of successful registrations

5. **Exit Intent Detection**
   - Trigger offers when user attempts to leave
   - Capture email for follow-up

6. **Mobile Optimization**
   - Test on all devices
   - Ensure form fields work well on mobile

7. **A/B Testing**
   - Test different copy, layouts, CTAs
   - Use data to inform decisions

## 🔄 Maintenance

### Regular Tasks

1. **Weekly**: Review conversion rates and major drop-offs
2. **Monthly**: Analyze marketing channel performance
3. **Quarterly**: Clean up old data (90+ days)
4. **As Needed**: Investigate unusual patterns or technical issues

### Monitoring

Set up alerts for:
- Conversion rate drops below threshold
- Sudden increase in abandonment at specific step
- Technical errors in tracking system

## 📚 References

### Inspired by Industry Leaders

1. **Mixpanel Funnel Analysis**
   - Event-based tracking
   - Cohort analysis
   - Retention metrics

2. **Amplitude Behavioral Analytics**
   - User journey mapping
   - Drop-off identification
   - Conversion optimization

3. **Google Analytics Enhanced Ecommerce**
   - Checkout funnel tracking
   - Product performance
   - Marketing attribution

4. **Hotjar Session Recording**
   - Visual behavior analysis
   - Form analytics
   - Feedback collection

### Further Reading

- [The Ultimate Guide to Conversion Rate Optimization](https://www.wordstream.com/conversion-rate-optimization)
- [SaaS Metrics 2.0](https://www.forentrepreneurs.com/saas-metrics-2/)
- [Funnel Analysis Best Practices](https://mixpanel.com/blog/funnel-analysis-best-practices/)

## 🤝 Support

For issues or questions:
- Technical: Review logs in `/api/salesfunnel` endpoints
- Business: Contact analytics team for interpretation
- Data Privacy: Ensure compliance with LGPD requirements

## 📄 License

Part of PrimeCare Software system - Internal use only
