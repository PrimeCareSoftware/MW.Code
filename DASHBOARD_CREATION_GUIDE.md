# Dashboard Creation Guide - Phase 3 Analytics

## 📊 Introduction

This guide explains how to create and manage custom dashboards in the MedicWarehouse System Admin module. With the new Analytics and BI features, you can build personalized dashboards with drag-and-drop widgets to monitor your SaaS metrics.

---

## 🎯 Getting Started

### Accessing Dashboards

1. Log in to System Admin with SystemAdmin role
2. Navigate to **Analytics** → **Custom Dashboards**
3. You'll see a list of your existing dashboards

### Dashboard Features

- **Drag-and-drop** widget positioning
- **Pre-built widgets** from template library
- **Custom SQL queries** for advanced users
- **Auto-refresh** capability
- **Export** to PDF, Excel, or JSON
- **Public/Private** sharing options

---

## 📝 Creating Your First Dashboard

### Step 1: Create New Dashboard

1. Click **"+ New Dashboard"** button
2. Fill in dashboard details:
   - **Name:** e.g., "Executive Overview"
   - **Description:** Brief explanation of dashboard purpose
   - **Is Default:** Check if this should be your default dashboard
   - **Is Public:** Check to share with other SystemAdmin users

3. Click **"Create"**

### Step 2: Add Widgets

1. Click **"Add Widget"** button in toolbar
2. Browse the **Widget Library** with categorized templates:
   - 💰 **Financial** - MRR, Revenue, ARR metrics
   - 👥 **Customer** - Active customers, growth, churn
   - ⚙️ **Operational** - Appointments, users, activity
   - 🏥 **Clinical** - Patients, procedures, clinics

3. Select a template (e.g., "MRR Over Time")
4. Widget is added to dashboard with default configuration

### Step 3: Position Widgets

1. **Edit Mode:** Click the lock icon to enable edit mode
2. **Drag:** Click and drag widgets to reposition
3. **Resize:** Drag corner handles to resize widgets
4. **Layout:** Widgets snap to a 12-column grid

5. **Save:** Click "Save" button to persist layout

### Step 4: Configure Widgets

1. Click the **gear icon** on any widget
2. Modify settings:
   - **Title:** Custom widget title
   - **Refresh Interval:** Auto-refresh in seconds (0 = manual)
   - **Chart Options:** Colors, axes, format
   - **Query:** Advanced users can edit SQL queries

3. Click **"Apply"** to save changes

---

## 📊 Widget Types

### 1. Metric Cards

**Purpose:** Display single KPI values with icons and color-coding

**Use Cases:**
- Total MRR
- Active Customers
- Churn Rate
- Total Appointments

**Configuration:**
```json
{
  "format": "currency",  // or "number", "percent"
  "icon": "attach_money",
  "color": "#10b981",
  "threshold": {
    "warning": 5,
    "critical": 10
  }
}
```

**Example Query:**
```sql
SELECT SUM(p."MonthlyPrice") as value
FROM "ClinicSubscriptions" cs
INNER JOIN "Plans" p ON cs."PlanId" = p."Id"
WHERE cs."Status" = 'Active'
```

---

### 2. Line Charts

**Purpose:** Show trends over time

**Use Cases:**
- MRR Over Time
- Customer Growth
- Revenue Trends

**Configuration:**
```json
{
  "xAxis": "month",
  "yAxis": "total_mrr",
  "color": "#10b981",
  "format": "currency"
}
```

**Example Query:**
```sql
SELECT 
    DATE_TRUNC('month', cs."CreatedAt") as month,
    SUM(p."MonthlyPrice") as total_mrr
FROM "ClinicSubscriptions" cs
INNER JOIN "Plans" p ON cs."PlanId" = p."Id"
WHERE cs."CreatedAt" >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY DATE_TRUNC('month', cs."CreatedAt")
ORDER BY month
```

---

### 3. Bar Charts

**Purpose:** Compare categories or time periods

**Use Cases:**
- Customer Growth by Month
- Patients by Clinic
- Revenue by Plan

**Configuration:**
```json
{
  "xAxis": "month",
  "yAxis": "new_customers",
  "color": "#3b82f6"
}
```

---

### 4. Pie Charts

**Purpose:** Show distribution or breakdown

**Use Cases:**
- Revenue by Plan Type
- Appointments by Status
- Customers by Segment

**Configuration:**
```json
{
  "labelField": "plan",
  "valueField": "revenue",
  "format": "currency"
}
```

**Example Query:**
```sql
SELECT 
    p."Name" as plan,
    SUM(p."MonthlyPrice") as revenue
FROM "ClinicSubscriptions" cs
INNER JOIN "Plans" p ON cs."PlanId" = p."Id"
WHERE cs."Status" = 'Active'
GROUP BY p."Name"
```

---

## 🔒 Writing Safe SQL Queries

### Security Rules

✅ **Allowed:**
- `SELECT` statements only
- `JOIN`, `WHERE`, `GROUP BY`, `ORDER BY`, `HAVING`
- Aggregate functions: `SUM`, `COUNT`, `AVG`, `MAX`, `MIN`
- Date functions: `DATE_TRUNC`, `NOW()`, `CURRENT_DATE`

❌ **Prohibited:**
- `INSERT`, `UPDATE`, `DELETE`
- `DROP`, `CREATE`, `ALTER`, `TRUNCATE`
- `EXEC`, `EXECUTE`, `CALL`, `PROCEDURE`
- Multiple statements (semicolons)
- SQL comments (`--` or `/* */`)

### Performance Limits

- **Timeout:** 30 seconds maximum
- **Row Limit:** 10,000 rows maximum
- **Columns:** All returned columns will be displayed

### Best Practices

1. **Use Table Aliases:**
```sql
SELECT c."Id" as clinic_id, c."TradeName" as clinic_name
FROM "Clinics" c
```

2. **Filter Data:**
```sql
WHERE cs."CreatedAt" >= CURRENT_DATE - INTERVAL '12 months'
```

3. **Aggregate Large Datasets:**
```sql
GROUP BY DATE_TRUNC('month', cs."CreatedAt")
```

4. **Limit Results:**
```sql
ORDER BY patient_count DESC
LIMIT 10
```

### Testing Queries

Before creating a widget, test your query:
1. Use a database client (pgAdmin, DBeaver)
2. Run query with EXPLAIN ANALYZE to check performance
3. Verify results are correct
4. Copy query to widget configuration

---

## 🎨 Dashboard Design Tips

### Layout Best Practices

1. **Top Row:** Most important KPIs (metric cards)
2. **Middle:** Trend charts (line/bar)
3. **Bottom:** Detailed breakdowns (pie/table)

### Grid Sizing

- **Metric Cards:** 3 columns × 2 rows (fits 4 per row)
- **Small Charts:** 6 columns × 3 rows (fits 2 per row)
- **Large Charts:** 12 columns × 4 rows (full width)
- **Tables:** 12 columns × 6 rows (full width)

### Example Layout

```
┌─────────┬─────────┬─────────┬─────────┐
│   MRR   │  Active │  Churn  │  ARPU   │  ← Metrics (3×2 each)
│  $45K   │   120   │  2.5%   │  $375   │
├─────────┴─────────┴─────────┴─────────┤
│      MRR Over Time (Line Chart)        │  ← Trend (12×4)
│                                        │
│                                        │
├───────────────────┬────────────────────┤
│ Customer Growth   │  Revenue Breakdown │  ← Details (6×3 each)
│   (Bar Chart)     │   (Pie Chart)      │
│                   │                    │
└───────────────────┴────────────────────┘
```

### Color Scheme

**Financial Metrics:**
- MRR/Revenue: Green (#10b981)
- Costs: Red (#ef4444)

**Customer Metrics:**
- Growth: Blue (#3b82f6)
- Churn: Red (#ef4444)

**Operational Metrics:**
- Activity: Purple (#8b5cf6)
- Alerts: Orange (#f59e0b)

**Clinical Metrics:**
- Patients: Orange (#f97316)
- Health: Green (#10b981)

---

## 📤 Exporting Dashboards

### Export Formats

1. **JSON** - Raw data for integration
2. **PDF** - Professional reports with branding
3. **Excel** - Spreadsheet for analysis

### Export Process

1. Open dashboard
2. Click **"Export"** button in toolbar
3. Select format
4. Download begins automatically

### PDF Features (Coming Soon)

- Company logo and branding
- Dashboard name and date
- All widgets rendered as images
- Metric values in tables
- Footer with generation timestamp

---

## 🔄 Auto-Refresh

Configure widgets to automatically refresh:

1. Edit widget configuration
2. Set **"Refresh Interval"** in seconds:
   - `0` - Manual refresh only
   - `60` - Every 1 minute
   - `300` - Every 5 minutes
   - `3600` - Every 1 hour

3. Widget displays refresh countdown timer
4. Data updates automatically without page reload

**Note:** Use caution with frequent refreshes on complex queries

---

## 🌐 Sharing Dashboards

### Public Dashboards

1. Edit dashboard settings
2. Check **"Is Public"**
3. All SystemAdmin users can view
4. Only creator can edit

### Private Dashboards

- Default for new dashboards
- Only creator can view and edit
- Not visible to other users

---

## 🛠️ Troubleshooting

### Widget Shows No Data

**Possible Causes:**
- Query returns empty result set
- Insufficient permissions
- Query timeout (> 30 seconds)

**Solutions:**
- Check query in database client
- Verify table names and columns
- Add date filters to reduce data
- Contact administrator for permissions

### Widget Shows Error

**Error Messages:**
- "Query contains unsafe operations" - Query blocked by security filter
- "Query timeout exceeded" - Query too slow (> 30 seconds)
- "Too many rows returned" - Query returned > 10,000 rows

**Solutions:**
- Review SQL security rules above
- Optimize query with indexes
- Add WHERE clause to filter data
- Use GROUP BY to aggregate

### Dashboard Not Saving

**Possible Causes:**
- Network connection issue
- Session expired
- Insufficient storage

**Solutions:**
- Refresh page and try again
- Log out and log back in
- Contact administrator

---

## 💡 Examples

### Executive Dashboard

**Purpose:** High-level SaaS metrics for leadership

**Widgets:**
1. Total MRR (Metric)
2. Active Customers (Metric)
3. Churn Rate (Metric)
4. ARPU (Metric)
5. MRR Over Time (Line Chart)
6. Revenue by Plan (Pie Chart)

---

### Operations Dashboard

**Purpose:** Monitor daily operational metrics

**Widgets:**
1. Total Appointments Today (Metric)
2. Active Users (Metric)
3. Open Tickets (Metric)
4. Appointments by Status (Pie Chart)
5. Appointment Trend (Line Chart)

---

### Customer Health Dashboard

**Purpose:** Track customer success metrics

**Widgets:**
1. Active Customers (Metric)
2. New Customers This Month (Metric)
3. Churn Rate (Metric)
4. Customer Growth (Bar Chart)
5. Customers at Risk (Table)

---

## 📞 Support

For additional help:
- **Documentation:** Check full API documentation
- **Support:** Contact system-admin@medicwarehouse.com
- **Training:** Request dashboard creation workshop

---

**Last Updated:** January 28, 2026  
**Version:** 1.0  
**Phase:** 3 - Analytics and BI
