# System Admin User Guide - Phase 1 Features

**Version:** 1.0  
**Target Audience:** System Administrators  
**Last Updated:** January 28, 2026

---

## 📖 Table of Contents

1. [Introduction](#introduction)
2. [Global Search](#global-search)
3. [Notification Center](#notification-center)
4. [SaaS Metrics Dashboard](#saas-metrics-dashboard)
5. [Tips and Best Practices](#tips-and-best-practices)
6. [FAQ](#faq)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 Introduction

Welcome to the System Admin area! This guide covers the new features introduced in Phase 1 of the modernization project:

- **Global Search**: Find clinics, users, tickets, plans, and audit logs instantly
- **Notification Center**: Stay informed about critical events in real-time
- **SaaS Metrics Dashboard**: Monitor business performance with key metrics

---

## 🔍 Global Search

### What is Global Search?

Global Search allows you to quickly find any information in the system without navigating through multiple pages. Search across clinics, users, tickets, subscription plans, and audit logs in milliseconds.

### How to Use Global Search

#### Opening the Search Modal

**Method 1: Keyboard Shortcut (Recommended)**
- Press `Ctrl + K` (Windows/Linux) or `Cmd + K` (Mac)
- The search modal will appear immediately

**Method 2: Click the Search Icon**
- Look for the search icon 🔍 in the navigation bar
- Click to open the search modal

#### Performing a Search

1. **Type your query**: Start typing your search term (minimum 2 characters)
2. **View results**: Results appear automatically as you type, grouped by category:
   - 🏥 **Clinics**: Search by name, CNPJ, email, or tenant ID
   - 👥 **Users**: Search by username, full name, or email
   - 🎫 **Tickets**: Search by title or description
   - 📋 **Plans**: Search by name or description
   - 📝 **Audit Logs**: Search by action, entity type, or user

3. **Navigate results**: 
   - Click any result to view details
   - Press `ESC` to close the modal
   - Use mouse or touch to select items

#### Search Tips

✅ **Good Search Terms:**
- Clinic names: "Clínica São Paulo"
- CNPJ: "12.345.678/0001-90"
- Email addresses: "admin@clinica.com"
- Usernames: "dr.silva"
- Ticket titles: "problema agendamento"

❌ **What Won't Work:**
- Single characters (need at least 2)
- Special characters alone
- Empty searches

#### Search History

The system automatically saves your recent searches for quick access:
- Open search modal (Ctrl+K)
- See your recent searches below the input
- Click any recent search to repeat it
- History persists across sessions (stored locally)

### Features

- ⚡ **Fast**: Results in under 1 second
- 🎯 **Accurate**: Smart matching finds exactly what you need
- 📊 **Grouped**: Results organized by entity type
- 💡 **Highlighted**: Your search terms are highlighted in yellow
- 🕒 **History**: Recent searches saved automatically
- ⌨️ **Keyboard-friendly**: Designed for power users

---

## 🔔 Notification Center

### What are System Notifications?

The Notification Center keeps you informed about important events and issues that require attention, such as:
- Expired subscriptions
- Expiring trial periods
- Inactive clinics
- Unresponded support tickets
- System health alerts

### How to Use Notifications

#### Accessing the Notification Center

1. **Look for the bell icon** 🔔 in the top navigation bar
2. **Check the badge**: A red badge shows the count of unread notifications
3. **Click the bell**: Opens the notification panel

#### Understanding Notifications

Each notification includes:
- **Icon and Color**: Indicates notification type
  - 🚨 **Red (Critical)**: Urgent issues requiring immediate attention
  - ⚠️ **Yellow (Warning)**: Important items that need action soon
  - ℹ️ **Blue (Info)**: General information
  - ✅ **Green (Success)**: Positive confirmations

- **Title**: Brief description of the event
- **Message**: Detailed information about what happened
- **Timestamp**: How long ago the event occurred (e.g., "2h atrás")
- **Action Button**: Quick link to view related details

#### Managing Notifications

**Reading a Notification:**
1. Click on any notification in the panel
2. The notification is marked as read automatically
3. If there's an action button, you'll be taken to the relevant page

**Marking All as Read:**
1. Click "Marcar todas como lidas" at the top of the panel
2. All notifications become marked as read
3. The badge count resets to 0

**Real-Time Updates:**
- New notifications appear automatically (no page refresh needed)
- A brief toast message shows important notifications
- The badge count updates in real-time

### Notification Types

#### 🚨 Critical Notifications

**Assinatura Vencida (Expired Subscription)**
- **When**: A clinic's subscription has expired
- **Action**: Contact the clinic immediately to renew
- **Frequency**: Detected hourly

**Erro Crítico do Sistema (Critical System Error)**
- **When**: A critical system error is detected
- **Action**: Investigate and resolve immediately
- **Frequency**: As they occur

#### ⚠️ Warning Notifications

**Trial Expirando (Trial Expiring)**
- **When**: A trial subscription expires in 3 days or less
- **Action**: Contact the clinic to discuss paid plans
- **Frequency**: Checked daily at 09:00 UTC

**Clínica Inativa (Inactive Clinic)**
- **When**: A clinic hasn't had activity for 30+ days
- **Action**: Reach out to understand why they're inactive
- **Frequency**: Checked daily at 10:00 UTC

**Ticket Sem Resposta (Unresponded Ticket)**
- **When**: A high-priority ticket hasn't been answered for 24+ hours
- **Action**: Respond to the ticket promptly
- **Frequency**: Checked every 6 hours

#### ℹ️ Info Notifications

**Nova Clínica Cadastrada (New Clinic Registered)**
- **When**: A new clinic completes registration
- **Action**: Welcome them and offer onboarding assistance
- **Frequency**: As they occur

**Plano Alterado (Plan Changed)**
- **When**: A clinic upgrades or downgrades their plan
- **Action**: Note the change for records
- **Frequency**: As they occur

#### ✅ Success Notifications

**Pagamento Confirmado (Payment Confirmed)**
- **When**: A subscription payment is successfully processed
- **Action**: No action needed
- **Frequency**: As they occur

**Backup Realizado (Backup Completed)**
- **When**: System backup completes successfully
- **Action**: No action needed
- **Frequency**: Daily

---

## 📊 SaaS Metrics Dashboard

### What are SaaS Metrics?

SaaS Metrics provide insights into your business performance, helping you:
- Track revenue growth
- Monitor customer acquisition and retention
- Identify churn patterns
- Make data-driven decisions

### Key Metrics Explained

#### 💰 Financial Metrics

**MRR (Monthly Recurring Revenue)**
- Total recurring revenue this month
- Shows growth trend (% change from last month)
- **Good**: Consistent growth (>5% MoM)
- **Concerning**: Declining or stagnant MRR

**ARR (Annual Recurring Revenue)**
- Projected annual revenue (MRR × 12)
- Useful for long-term planning
- **Goal**: Steady increase year over year

**ARPU (Average Revenue Per User)**
- MRR divided by active customers
- Shows average value per customer
- **Good**: Increasing ARPU (customers upgrading)
- **Concerning**: Decreasing ARPU (customers downgrading)

#### 👥 Customer Metrics

**Active Customers**
- Total number of active subscriptions
- Shows new customers this month
- **Goal**: Consistent growth

**Churn Rate**
- Percentage of customers lost this month
- Lower is better
- **Excellent**: < 2% monthly
- **Good**: 2-5% monthly
- **Concerning**: > 5% monthly

**Trial Customers**
- Active trial subscriptions
- Potential future paid customers
- **Goal**: High trial-to-paid conversion rate

#### 📈 Growth Metrics

**MoM Growth Rate (Month-over-Month)**
- Revenue growth compared to last month
- **Excellent**: > 10%
- **Good**: 5-10%
- **Needs Improvement**: < 5%

**YoY Growth Rate (Year-over-Year)**
- Revenue growth compared to same month last year
- **Excellent**: > 40%
- **Good**: 20-40%
- **Needs Improvement**: < 20%

**Quick Ratio**
- (New MRR + Expansion MRR) / (Contraction MRR + Churned MRR)
- Measures growth efficiency
- **Excellent**: > 4
- **Good**: 2-4
- **Concerning**: < 2

#### 💎 Advanced Metrics

**LTV (Lifetime Value)**
- Average revenue generated per customer over their lifetime
- Higher is better
- **Goal**: Maximize LTV through retention

**CAC (Customer Acquisition Cost)**
- Average cost to acquire a new customer
- Lower is better
- **Goal**: Minimize CAC through efficient marketing

**LTV/CAC Ratio**
- Lifetime value divided by acquisition cost
- Shows return on marketing investment
- **Excellent**: > 3
- **Good**: 2-3
- **Unprofitable**: < 1

### Using the Dashboard

#### Viewing Metrics

**⚠️ Note**: Full dashboard visualization is pending implementation. Currently available via API endpoints.

**Coming Soon:**
1. Navigate to Dashboard or SaaS Metrics page
2. View KPI cards showing key metrics
3. Explore interactive charts:
   - Revenue Timeline (12-month view)
   - Growth Trends
   - Customer Breakdown
   - Churn Analysis

#### Interpreting Trends

**Positive Indicators:**
- ✅ MRR increasing month-over-month
- ✅ Churn rate decreasing
- ✅ New customers exceeding churned customers
- ✅ LTV/CAC ratio above 3
- ✅ Quick Ratio above 4

**Warning Signs:**
- ⚠️ MRR declining
- ⚠️ Churn rate increasing
- ⚠️ ARPU decreasing
- ⚠️ More customers churning than signing up
- ⚠️ LTV/CAC ratio below 2

---

## 💡 Tips and Best Practices

### Global Search

1. **Use keyboard shortcuts**: Ctrl+K is faster than clicking
2. **Be specific**: Use clinic names, CNPJ, or email for best results
3. **Check recent searches**: Your frequent searches are saved
4. **Partial matches work**: You don't need to type the complete name

### Notifications

1. **Check daily**: Start your day by reviewing notifications
2. **Act on critical notifications immediately**: Red badges need urgent attention
3. **Mark as read regularly**: Keep your notification center organized
4. **Use action links**: Click the action button for quick navigation

### SaaS Metrics

1. **Monitor trends, not absolutes**: A single month's data can be misleading
2. **Set goals**: Define target metrics for your business
3. **Review weekly**: Check metrics every Monday to track progress
4. **Compare periods**: Look at MoM and YoY trends for context
5. **Focus on Quick Ratio**: It's the best single indicator of growth health

---

## ❓ FAQ

**Q: Why isn't my search finding anything?**
A: Make sure your query has at least 2 characters. Try different search terms or check spelling.

**Q: How do I disable notification sounds?**
A: Currently, notifications don't have sound. Visual notifications only.

**Q: Can I customize which notifications I receive?**
A: Custom notification preferences are planned for a future release.

**Q: How often are metrics updated?**
A: Metrics are calculated in real-time when you view the dashboard. Background jobs run hourly/daily.

**Q: What does "At-Risk Customers" mean?**
A: Customers with low usage or engagement patterns. Feature in development.

**Q: Why is my CAC showing R$ 500?**
A: This is a placeholder value. Actual CAC tracking requires marketing cost data integration.

**Q: Can I export metrics data?**
A: Export functionality is planned for a future release.

**Q: How do I see historical data?**
A: Use the Revenue Timeline API endpoint with different month parameters. Dashboard visualization coming soon.

---

## 🔧 Troubleshooting

### Global Search Issues

**Problem: Search modal won't open**
- Solution: Check if Ctrl+K is captured by another app. Try clicking the search icon instead.

**Problem: Results are slow to load**
- Solution: Check your internet connection. Results should load in under 1 second.

**Problem: Getting "too short" error**
- Solution: Type at least 2 characters before searching.

### Notification Issues

**Problem: Not receiving real-time notifications**
- Solution: Ensure WebSockets are not blocked by your network/firewall.

**Problem: Badge count is wrong**
- Solution: Refresh the page. If issue persists, contact support.

**Problem: Notifications disappear**
- Solution: Check if "Mark all as read" was clicked accidentally.

### Metrics Issues

**Problem: Metrics show zero or unexpected values**
- Solution: Ensure you have subscription data in the system. Contact support if issue persists.

**Problem: MRR doesn't match expectations**
- Solution: MRR only counts active subscriptions. Check subscription statuses.

**Problem: Churn rate seems high**
- Solution: Verify subscription cancellation data. Investigate reasons for cancellations.

---

## 📞 Support

**Note:** The contact information below may be placeholders. Please verify with your organization's actual support contacts.

Need help? Contact our support team:
- **Email**: support@primecare.com.br
- **Phone**: +55 11 1234-5678
- **Hours**: Monday-Friday, 9:00-18:00 (BRT)

---

## 📚 Additional Resources

- [API Documentation](./SYSTEM_ADMIN_API_DOCUMENTATION.md)
- [Technical Implementation Guide](./SYSTEM_ADMIN_PHASE1_IMPLEMENTATION_COMPLETE.md)
- [System Admin Area Guide](./system-admin/guias/SYSTEM_ADMIN_AREA_GUIDE.md)

---

**Version:** 1.0  
**Last Updated:** January 28, 2026  
**Next Review:** February 2026
