# BI Analytics Frontend - Testing Guide

## Quick Access URLs

### Development
- **Clinical Dashboard**: `http://localhost:4200/analytics/dashboard-clinico`
- **Financial Dashboard**: `http://localhost:4200/analytics/dashboard-financeiro`
- **Analytics Main**: `http://localhost:4200/analytics`

### Navigation Menu
Access via: **Sidebar → Relatórios → Dashboard Clínico / Dashboard Financeiro**

---

## Test Scenarios

### 1. Clinical Dashboard Tests

#### Basic Loading
1. Navigate to Clinical Dashboard
2. ✅ Verify loading spinner appears
3. ✅ Verify 4 KPI cards render
4. ✅ Verify 5 charts/visualizations appear

#### KPI Cards
- **Total Consultas**: Should show integer count
- **Taxa de Ocupação**: Should show percentage with % symbol
- **Tempo Médio**: Should show minutes with "min" suffix
- **Taxa de No-Show**: Should show percentage, red if > 15%

#### Date Range Filters
Test each filter option:
- [ ] Hoje
- [ ] Últimos 7 dias
- [ ] Últimos 30 dias (default)
- [ ] Este mês
- [ ] Mês passado
- [ ] Últimos 3 meses
- [ ] Últimos 6 meses
- [ ] Personalizado (verify custom date inputs appear)

#### Charts
1. **Consultas por Especialidade** (Donut Chart)
   - ✅ Shows specialty names
   - ✅ Shows percentages on hover
   - ✅ Legend at bottom

2. **Distribuição Semanal** (Bar Chart)
   - ✅ Shows days of week
   - ✅ Shows consultation count
   - ✅ Blue bars

3. **Diagnósticos CID-10** (List)
   - ✅ Shows CID code + description
   - ✅ Shows frequency count
   - ✅ Progress bar shows percentage
   - ✅ Top 10 diagnoses

4. **Pacientes Novos vs Retorno** (Stats)
   - ✅ Shows two numbers
   - ✅ Blue for new, green for return
   - ✅ Shows percentages

5. **Tendência Mensal** (Line Chart)
   - ✅ Shows monthly labels
   - ✅ Two lines: Realizadas (blue), Agendadas (purple)
   - ✅ Smooth curves

#### Error Handling
1. Disconnect backend
2. ✅ Verify error message displays
3. ✅ Verify "Tentar novamente" button appears
4. ✅ Click button to retry

---

### 2. Financial Dashboard Tests

#### Basic Loading
1. Navigate to Financial Dashboard
2. ✅ Verify projection banner at top
3. ✅ Verify 8 KPI cards render
4. ✅ Verify 4 charts appear

#### Revenue Projection Banner
- ✅ Shows current month name
- ✅ Shows projected amount in BRL currency
- ✅ Gradient blue/purple background

#### KPI Cards
- **Receita Total**: Total revenue amount
- **Recebida**: Received amount (green)
- **Pendente**: Pending amount (orange)
- **Atrasada**: Overdue amount (red, alert border if > 0)
- **Lucro Bruto**: Gross profit (can be negative)
- **Margem de Lucro**: Profit margin percentage
- **Ticket Médio**: Average ticket value
- **Despesas Totais**: Total expenses

#### Date Range Filters
Same as Clinical Dashboard - test all options

#### Charts
1. **Receita por Forma de Pagamento** (Pie Chart)
   - ✅ Shows payment method names
   - ✅ Translated to Portuguese
   - ✅ Shows percentages
   - ✅ Legend at bottom

2. **Receita por Convênio** (Bar Chart)
   - ✅ Shows insurance plan names
   - ✅ Shows revenue amounts
   - ✅ Currency format on hover
   - ✅ Rotated x-axis labels

3. **Fluxo de Caixa Diário** (Line Chart)
   - ✅ Two lines: Entradas (green), Saídas (red)
   - ✅ Shows daily dates (dd/MM format)
   - ✅ Currency format on hover
   - ✅ Smooth curves

4. **Despesas por Categoria** (Horizontal Bar Chart)
   - ✅ Shows category names
   - ✅ Horizontal red bars
   - ✅ Currency format on hover

---

### 3. Responsive Design Tests

#### Desktop (1920x1080)
- [ ] Charts display at full width
- [ ] KPI cards in 4-column grid
- [ ] No horizontal scrolling

#### Tablet (768x1024)
- [ ] Charts adjust to 2 columns
- [ ] KPI cards in 2 columns
- [ ] Sidebar collapsible
- [ ] Touch-friendly buttons

#### Mobile (375x667)
- [ ] Charts stack vertically
- [ ] KPI cards stack vertically
- [ ] Sidebar hidden by default
- [ ] Filter section stacks
- [ ] Custom date inputs on separate rows

---

### 4. Navigation Tests

#### Menu Access
1. [ ] Click "Relatórios" in sidebar
2. [ ] Verify it highlights
3. [ ] See submenu items appear
4. [ ] Click "Dashboard Clínico"
5. [ ] Verify navigation to clinical dashboard
6. [ ] Click back to "Relatórios"
7. [ ] Click "Dashboard Financeiro"
8. [ ] Verify navigation to financial dashboard

#### Direct URL Access
1. [ ] Open `/analytics/dashboard-clinico` directly
2. [ ] Verify authentication required
3. [ ] After login, verify dashboard loads
4. [ ] Repeat for financial dashboard

#### Browser Back/Forward
1. [ ] Navigate between dashboards
2. [ ] Use browser back button
3. [ ] Verify previous dashboard loads
4. [ ] Use browser forward button
5. [ ] Verify navigation works correctly

---

### 5. API Integration Tests

#### Clinical Dashboard API
**Endpoint**: `GET /api/Analytics/dashboard/clinico`

**Query Params**:
- `inicio`: "2024-01-01"
- `fim`: "2024-01-31"
- `medicoId`: (optional, currently not used)

**Expected Response**: 200 OK with JSON matching `DashboardClinicoDto`

**Test**:
```bash
curl -X GET "http://localhost:5000/api/Analytics/dashboard/clinico?inicio=2024-01-01&fim=2024-01-31" \
  -H "Authorization: Bearer {token}"
```

#### Financial Dashboard API
**Endpoint**: `GET /api/Analytics/dashboard/financeiro`

**Query Params**:
- `inicio`: "2024-01-01"
- `fim`: "2024-01-31"

**Expected Response**: 200 OK with JSON matching `DashboardFinanceiroDto`

**Test**:
```bash
curl -X GET "http://localhost:5000/api/Analytics/dashboard/financeiro?inicio=2024-01-01&fim=2024-01-31" \
  -H "Authorization: Bearer {token}"
```

#### Revenue Projection API
**Endpoint**: `GET /api/Analytics/projecao/receita-mes`

**Expected Response**: 200 OK with projection data

**Test**:
```bash
curl -X GET "http://localhost:5000/api/Analytics/projecao/receita-mes" \
  -H "Authorization: Bearer {token}"
```

---

### 6. Performance Tests

#### Load Time
- [ ] Dashboard loads in < 3 seconds with real data
- [ ] Charts render in < 1 second
- [ ] Filter changes respond in < 500ms

#### Memory Usage
- [ ] Monitor browser memory (DevTools)
- [ ] Load dashboard
- [ ] Change filters 10 times
- [ ] Verify no memory leaks

#### Network
- [ ] Monitor network tab
- [ ] Verify only 1 API call per dashboard load
- [ ] Verify no unnecessary re-fetching
- [ ] Verify proper caching headers

---

### 7. Edge Cases

#### Empty Data
1. [ ] Test with date range that has no data
2. [ ] Verify "Nenhum diagnóstico registrado" message
3. [ ] Verify charts handle empty arrays gracefully
4. [ ] Verify KPI cards show 0 or N/A

#### Large Data Sets
1. [ ] Test with 6-month date range
2. [ ] Verify charts don't become unreadable
3. [ ] Verify performance remains acceptable

#### Invalid Date Range
1. [ ] Select end date before start date
2. [ ] Verify appropriate error handling

#### Network Errors
1. [ ] Disconnect network
2. [ ] Try to load dashboard
3. [ ] Verify error message
4. [ ] Reconnect network
5. [ ] Click retry
6. [ ] Verify data loads

---

### 8. Browser Compatibility

Test in:
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile Safari (iOS)
- [ ] Chrome Mobile (Android)

---

### 9. Accessibility Tests

#### Keyboard Navigation
- [ ] Tab through all interactive elements
- [ ] Verify focus indicators visible
- [ ] Press Enter on buttons
- [ ] Verify keyboard accessible

#### Screen Reader
- [ ] Test with NVDA/JAWS
- [ ] Verify KPI cards have proper labels
- [ ] Verify charts have descriptive text
- [ ] Verify error messages announced

#### Color Contrast
- [ ] Use contrast checker
- [ ] Verify all text meets WCAG AA
- [ ] Verify chart colors distinguishable

---

### 10. Export Tests (Placeholder)

1. [ ] Click "Exportar" button
2. [ ] Verify console message appears
3. [ ] Verify no errors thrown
4. [ ] (Future) Verify CSV/Excel download

---

## Bug Reporting Template

```markdown
**Dashboard**: Clinical / Financial
**Component**: KPI Card / Chart / Filter / etc.
**Browser**: Chrome 120
**Device**: Desktop / Mobile
**Steps to Reproduce**:
1. 
2. 
3. 

**Expected**: 
**Actual**: 
**Screenshot**: [attach]
**Console Errors**: [paste]
```

---

## Success Criteria

### Clinical Dashboard
- ✅ All 4 KPIs display correctly
- ✅ All 5 charts render
- ✅ Date filters work
- ✅ Responsive on all devices
- ✅ No console errors
- ✅ Loads in < 3 seconds

### Financial Dashboard
- ✅ All 8 KPIs display correctly
- ✅ All 4 charts render
- ✅ Projection banner shows
- ✅ Date filters work
- ✅ Responsive on all devices
- ✅ No console errors
- ✅ Loads in < 3 seconds

### Overall
- ✅ Navigation works
- ✅ Authentication required
- ✅ No security vulnerabilities
- ✅ Cross-browser compatible
- ✅ Accessible (WCAG AA)

---

## Contact

**Issues**: Report in GitHub Issues
**Questions**: Contact frontend team
**Documentation**: See `IMPLEMENTATION_SUMMARY_BI_ANALYTICS_FRONTEND.md`
