# System Admin User Guide - Complete Features

**Version:** 2.0  
**Target Audience:** System Administrators  
**Last Updated:** January 28, 2026

---

## 📖 Table of Contents

### Phase 1 Features
1. [Introduction](#introduction)
2. [Global Search](#global-search)
3. [Notification Center](#notification-center)
4. [SaaS Metrics Dashboard](#saas-metrics-dashboard)

### Phase 2 Features (NEW)
5. [Advanced Clinic Management](#advanced-clinic-management)
6. [Health Score System](#health-score-system)
7. [Tag Management](#tag-management)
8. [Cross-Tenant User Management](#cross-tenant-user-management)

### General
9. [Tips and Best Practices](#tips-and-best-practices)
10. [FAQ](#faq)
11. [Troubleshooting](#troubleshooting)

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
- **Email**: support@omnicare.com.br
- **Phone**: +55 11 1234-5678
- **Hours**: Monday-Friday, 9:00-18:00 (BRT)

---

## 📚 Additional Resources

- [Phase 1 API Documentation](./SYSTEM_ADMIN_API_DOCUMENTATION.md)
- [Phase 2 API Documentation](./SYSTEM_ADMIN_FASE2_API_DOCUMENTATION.md)
- [Phase 1 Implementation Guide](./SYSTEM_ADMIN_PHASE1_IMPLEMENTATION_COMPLETE.md)
- [Phase 2 Implementation Guide](./SYSTEM_ADMIN_FASE2_IMPLEMENTACAO.md)
- [System Admin Area Guide](./system-admin/guias/SYSTEM_ADMIN_AREA_GUIDE.md)

---

## 🎯 Phase 2: Advanced Clinic Management (NEW)

### Overview

Phase 2 transforms the basic clinic listing into a powerful CRM system with:
- **Multiple View Modes**: List, Cards, Map, and Kanban views
- **Health Score**: Automated clinic health assessment (0-100 points)
- **Smart Tagging**: Categorize and segment clinics
- **Cross-Tenant User Management**: Manage users across all clinics
- **Advanced Analytics**: Deep insights into clinic performance

---

## 📊 Advanced Clinic Management

### Multiple View Modes

Switch between different views to analyze your clinics:

#### 1. List View (Default)
- Traditional table layout with sortable columns
- Advanced filtering options
- Bulk actions support
- Best for: Detailed data analysis

#### 2. Cards View
- Visual card-based layout
- Quick overview of key metrics
- Color-coded health status
- Best for: Quick scanning of multiple clinics

#### 3. Map View
- Geographic visualization
- Filter by region
- Cluster markers for density
- Best for: Geographic analysis and planning

#### 4. Kanban View
- Drag-and-drop by status
- Columns: Trial → Active → At-Risk → Churned
- Visual pipeline management
- Best for: Workflow management and tracking

### Advanced Filtering

Filter clinics by multiple criteria simultaneously:

**Available Filters:**
- 🔍 **Search**: Name, email, document, subdomain
- ✅ **Status**: Active / Inactive
- 🏷️ **Tags**: Filter by assigned tags
- 💚 **Health Status**: Healthy / Needs Attention / At Risk
- 📋 **Subscription**: Trial / Active / Expired / Suspended
- 📅 **Date Range**: Created after/before specific dates

**How to Use:**
1. Click "Filters" button in the toolbar
2. Select your criteria
3. Click "Apply Filters"
4. Results update automatically
5. Clear filters anytime with "Reset"

### Segment Quick Access

Pre-defined segments for quick access:

- 🆕 **New**: Clinics created in last 30 days
- 🧪 **Trial**: Currently on trial period
- 🔴 **At Risk**: Health score < 50 points
- 🟡 **Needs Attention**: Health score 50-79 points
- 🟢 **Healthy**: Health score ≥ 80 points
- ⏸️ **Inactive**: Deactivated clinics
- 💎 **VIP**: High-value customers (tagged)

**How to Use:**
1. Look for segment chips at the top of the page
2. Click any segment chip
3. View is automatically filtered
4. Number badge shows count for each segment

---

## 💚 Health Score System

### What is Health Score?

Health Score is an automated 0-100 point assessment of clinic engagement and satisfaction. It helps identify:
- Clinics that need attention
- Churn risk indicators
- Success stories
- Upsell opportunities

### How Health Score is Calculated

The score is based on 4 components:

#### 1. Usage Score (0-30 points)
Based on days since last activity:
- ✅ Active today (≤1 day): **30 points**
- ✅ Active this week (≤7 days): **25 points**
- ⚠️ Active this month (≤14 days): **20 points**
- ⚠️ Rarely active (≤30 days): **10 points**
- 🔴 Inactive (>30 days): **0 points**

#### 2. User Engagement Score (0-25 points)
Percentage of users active in last 30 days:
- Formula: `25 × (active_users / total_users)`
- 100% active = 25 points
- 50% active = 12.5 points
- 0% active = 0 points

#### 3. Support Score (0-20 points)
Based on open support tickets:
- 0 tickets: **20 points** ✅
- 1 ticket: **15 points** ✅
- 2 tickets: **10 points** ⚠️
- 3 tickets: **5 points** 🔴
- 4+ tickets: **0 points** 🔴

#### 4. Payment Score (0-25 points)
Payment status:
- All payments current: **25 points** ✅
- Overdue or failed payments: **0 points** 🔴

### Health Status Classification

| Status | Score Range | Color | Action Required |
|--------|-------------|-------|-----------------|
| 🟢 **Healthy** | 80-100 | Green | Maintain relationship |
| 🟡 **Needs Attention** | 50-79 | Yellow | Check in, offer support |
| 🔴 **At Risk** | 0-49 | Red | Urgent intervention |

### Viewing Health Score

**In Clinic List:**
- Health score badge shown next to clinic name
- Color indicates status
- Click badge for detailed breakdown

**In Clinic Profile:**
- Dedicated "Health Score" tab
- Detailed component breakdown
- Historical trends (coming soon)
- Recommendations for improvement

### Best Practices

1. **Daily Review**: Check "At Risk" segment daily
2. **Weekly Outreach**: Contact "Needs Attention" clinics weekly
3. **Proactive Support**: Reach out before score drops
4. **Success Stories**: Learn from "Healthy" clinics
5. **Automation**: Set up alerts for score changes (coming soon)

---

## 🏷️ Tag Management

### What are Tags?

Tags are labels you can assign to clinics for organization, segmentation, and automation. Think of them as flexible categories that help you:
- Organize clinics by any criteria
- Create targeted marketing campaigns
- Track special situations
- Automate workflows

### Tag Categories

Tags are organized into 5 categories:

1. **Type** (🏥): Business type
   - Examples: Dental, Medical, Veterinary, Psychology
   
2. **Region** (📍): Geographic location
   - Examples: Southeast, South, North, Northeast
   
3. **Value** (💎): Revenue segmentation
   - Examples: High Value, Standard, Entry
   
4. **Status** (🎯): Lifecycle stage
   - Examples: New, At Risk, Churned, Recovering
   
5. **Custom** (✨): Your own categories
   - Examples: Partner, Referral, Beta Tester

### Creating Tags

1. Navigate to Settings → Tags Management
2. Click "Create New Tag"
3. Fill in the form:
   - **Name**: Short, descriptive name (e.g., "High Value")
   - **Category**: Select from dropdown
   - **Color**: Choose a color for visual identification
   - **Description**: Optional details about when to use
   - **Automatic**: Check if this tag should be applied automatically
4. Click "Save"

### Assigning Tags

**Manual Assignment:**
1. Open clinic profile
2. Click "Add Tag" button
3. Select tags from dropdown
4. Tags are applied immediately

**Bulk Assignment:**
1. Select multiple clinics (checkboxes)
2. Click "Bulk Actions" → "Add Tag"
3. Select tag to apply
4. Confirm action

**Automatic Assignment:**
Some tags are applied automatically based on rules:
- "At Risk" → No activity in 30 days
- "High Value" → MRR ≥ R$ 1,000
- "New" → Created in last 30 days

### Filtering by Tags

**In Clinic List:**
1. Open "Filters" panel
2. Section "Tags"
3. Select one or more tags
4. Click "Apply"
5. Only clinics with selected tags are shown

**Quick Tag Filter:**
- Click any tag badge directly
- Instant filter applied
- Click again to remove filter

### Tag Best Practices

✅ **Do:**
- Use consistent naming conventions
- Choose distinct colors for easy identification
- Document tag usage in descriptions
- Review and clean up unused tags periodically
- Use automatic tags for objective criteria

❌ **Don't:**
- Create too many similar tags
- Use vague or unclear names
- Change tag meanings over time
- Forget to remove outdated tags

---

## 👥 Cross-Tenant User Management

### Overview

Manage all users across all clinics from a single interface. Perfect for:
- Password resets
- Account activation/deactivation
- User auditing
- Cross-clinic analysis

### Searching Users

**Basic Search:**
1. Navigate to "All Users" section
2. Enter search term (name, email, phone)
3. Results show users from all clinics
4. Click user to view details

**Advanced Filtering:**
- **Role**: Filter by Doctor, Secretary, Admin, etc.
- **Status**: Active or inactive users
- **Clinic**: Users from specific clinic
- **Combination**: Use multiple filters together

### User Details

Click any user to see:
- Full name and contact info
- Role and permissions
- Account status (active/inactive)
- Creation date
- Associated clinic information
- Recent activity (if available)

### User Operations

#### Reset Password
1. Open user profile
2. Click "Reset Password"
3. Enter new temporary password
4. Click "Confirm"
5. User receives notification (optional)
6. User must change password on next login

**Security Note:** New password must be at least 6 characters.

#### Toggle Activation
1. Open user profile
2. Click "Activate" or "Deactivate" button
3. Confirm action
4. Status changes immediately
5. User is logged out if deactivated

**Use Cases:**
- Deactivate: Employee left, security concern, inactive account
- Activate: Re-enable access, restore account

### Bulk Operations

Select multiple users to:
- Send email announcements
- Export user list
- Generate reports

---

## 📈 Clinic Profile (Enhanced)

### Profile Tabs

The new clinic profile has 5 tabs:

#### 1. Overview Tab
- Basic information (name, contact, address)
- Current subscription details
- Health score summary
- Quick actions (edit, deactivate, impersonate)

#### 2. Timeline Tab
- Chronological history of all events
- Subscription changes
- Ticket creation/resolution
- User additions
- Significant activities
- Filterable by event type

#### 3. Metrics Tab
- Usage statistics
- Login frequency (7 days, 30 days)
- Appointments created
- Patients registered
- Documents generated
- Customizable date range

#### 4. Health Score Tab
- Detailed score breakdown
- Each component explained
- Historical trends (coming soon)
- Recommendations for improvement
- Export report option

#### 5. Tags Tab
- All assigned tags
- Add/remove tags
- Tag history
- Automatic vs manual tags

---

## 💡 Tips and Best Practices (Updated)

### For Clinic Management

1. **Daily Routine:**
   - Review "At Risk" segment first thing
   - Check health scores for alerts
   - Respond to new support tickets

2. **Weekly Tasks:**
   - Analyze "Needs Attention" clinics
   - Review new clinic onboarding
   - Update tags as needed
   - Check usage metrics

3. **Monthly Review:**
   - Overall health score trends
   - Segment distribution changes
   - Tag effectiveness
   - User engagement patterns

### For User Management

1. **Security:**
   - Regularly review inactive users
   - Deactivate accounts promptly when employees leave
   - Use strong temporary passwords for resets
   - Monitor cross-tenant access patterns

2. **Organization:**
   - Keep user roles up to date
   - Document special permissions
   - Regular user audits

---

**Version:** 2.0  
**Last Updated:** January 28, 2026  
**Next Review:** February 2026
