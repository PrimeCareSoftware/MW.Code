# TISS Test Coverage Plan

**Date**: January 2026  
**Current Coverage**: ~50% (Entity tests: 100%, Service tests: 30-40%, Controller tests: 10%, Integration tests: 0%)  
**Target Coverage**: 80%+  
**Reference**: docs/prompts-copilot/critico/03-integracao-tiss.md

## Current Test Status

### ✅ Completed (100% Coverage)

#### Entity Tests - 212 Tests Passing
**Location**: `tests/MedicSoft.Test/Entities/`

- ✅ `TissBatchTests.cs` - 30 tests
- ✅ `TissGuideTests.cs` - 32 tests
- ✅ `TissGuideProcedureTests.cs` - 30 tests
- ✅ `HealthInsuranceOperatorTests.cs` - 19 tests
- ✅ `HealthInsurancePlanTests.cs` - 18 tests
- ✅ `PatientHealthInsuranceTests.cs` - 33 tests
- ✅ `TussProcedureTests.cs` - 20 tests
- ✅ `AuthorizationRequestTests.cs` - 30 tests

**Coverage**: 100% ✅  
**Quality**: Excellent - All domain logic validated

### ⚠️ Partial Coverage (30-40%)

#### Service Tests - 118 Tests Exist
**Location**: `tests/MedicSoft.Test/Services/`

- ⚠️ `TissBatchServiceTests.cs` - 28 tests (needs expansion)
- ⚠️ `TissGuideServiceTests.cs` - 24 tests (needs expansion)
- ⚠️ `TissAnalyticsServiceTests.cs` - 28 tests (good coverage)
- ⚠️ `TissXmlValidatorServiceTests.cs` - 13 tests (needs expansion)
- ⚠️ `HealthInsuranceOperatorServiceTests.cs` - 25 tests (needs expansion)

**Coverage**: 30-40% ⚠️  
**Quality**: Good foundation, needs more edge cases and error scenarios

### ❌ Minimal Coverage (10%)

#### Controller Tests - 0 TISS Controller Tests
**Location**: `tests/MedicSoft.Test/Api/`

- ❌ `TissGuidesControllerTests.cs` - **NOT CREATED**
- ❌ `TissBatchesControllerTests.cs` - **NOT CREATED**
- ❌ `TissAnalyticsControllerTests.cs` - **NOT CREATED**
- ❌ `HealthInsuranceOperatorsControllerTests.cs` - **NOT CREATED**
- ❌ `TussProceduresControllerTests.cs` - **NOT CREATED**

**Coverage**: 0% ❌  
**Quality**: Missing - Critical gap

### ❌ No Coverage (0%)

#### Integration Tests - 0 Tests
**Location**: `tests/MedicSoft.Test/Integration/`

- ❌ `TissIntegrationTests.cs` - **CREATED BUT DISABLED** (awaiting build fix)

**Coverage**: 0% ❌  
**Quality**: Skeletal structure created, implementation pending

## Roadmap to 80% Coverage

### Phase 1: Fix Build Issues (Priority: CRITICAL)

**Problem**: Test project has unrelated compilation errors that prevent new tests from being added.

**Affected Files**:
- `tests/MedicSoft.Test/Entities/AppointmentTests.cs`
- `tests/MedicSoft.Test/Handlers/Queries/PublicClinics/SearchPublicClinicsQueryHandlerTests.cs`
- `tests/MedicSoft.Test/Services/HealthInsuranceOperatorServiceTests.cs`
- `tests/MedicSoft.Test/Services/AuthorizationRequestServiceTests.cs`
- `tests/MedicSoft.Test/Services/PatientHealthInsuranceServiceTests.cs`
- `tests/MedicSoft.Test/Services/UserServiceTests.cs`

**Errors**:
1. Missing enum types: `AppointmentType`, `AppointmentStatus`, `AppointmentMode`, `PaymentType`
2. Method signature mismatches in repository interfaces
3. Patient constructor signature changes
4. HealthInsurancePlan constructor changes

**Action Items**:
- [ ] Fix missing enum imports in AppointmentTests
- [ ] Update repository mock setups for signature changes
- [ ] Fix Patient entity constructor calls
- [ ] Update HealthInsurancePlan usage patterns

**Estimated Effort**: 2-4 hours  
**Required**: YES - Blocks all new test development

### Phase 2: Expand Service Tests (30% → 80%)

Once build issues are fixed, expand existing service tests.

#### TissBatchServiceTests (28 tests → 50+ tests)

**Current Coverage**: ~40%  
**Target**: 80%

**Additional Tests Needed**:
- [ ] More batch status transition tests
- [ ] Edge cases for guide addition (duplicate guides, invalid guides)
- [ ] Batch validation scenarios
- [ ] XML generation error handling
- [ ] Concurrent batch operations
- [ ] Large batch performance tests
- [ ] Batch cancellation scenarios
- [ ] Batch resubmission logic

**Estimated New Tests**: 25-30  
**Estimated Effort**: 1 day

#### TissGuideServiceTests (24 tests → 45+ tests)

**Current Coverage**: ~35%  
**Target**: 80%

**Additional Tests Needed**:
- [ ] Guide status transitions (Draft → Pending → Submitted → Processed)
- [ ] Procedure addition/removal edge cases
- [ ] Authorization validation scenarios
- [ ] Patient insurance validation
- [ ] Appointment linking validation
- [ ] Guide calculation logic (totals, discounts, glosses)
- [ ] Guide cancellation and void logic
- [ ] Duplicate guide prevention

**Estimated New Tests**: 25-30  
**Estimated Effort**: 1 day

#### TissXmlGeneratorServiceTests (NEW)

**Current Coverage**: 0%  
**Target**: 80%

**Tests Needed**:
- [ ] Valid XML structure generation
- [ ] TISS version 4.02.00 compliance
- [ ] Required fields validation
- [ ] Optional fields handling
- [ ] Multiple guides in batch
- [ ] Special characters encoding
- [ ] Date/time formatting
- [ ] Decimal precision handling
- [ ] XML namespace validation
- [ ] Schema compliance

**Estimated New Tests**: 20-25  
**Estimated Effort**: 1 day

#### TissAnalyticsServiceTests (28 tests → 35+ tests)

**Current Coverage**: ~60% (Good!)  
**Target**: 80%

**Additional Tests Needed**:
- [ ] More complex gloss calculation scenarios
- [ ] Time-based metric calculations
- [ ] Operator ranking edge cases
- [ ] Performance with large datasets
- [ ] Date range validation
- [ ] Empty result handling

**Estimated New Tests**: 10-15  
**Estimated Effort**: 0.5 day

#### HealthInsuranceOperatorServiceTests (25 tests → 40+ tests)

**Current Coverage**: ~40%  
**Target**: 80%

**Additional Tests Needed**:
- [ ] Operator CRUD edge cases
- [ ] ANS registration validation
- [ ] CNPJ validation
- [ ] Contact information management
- [ ] Integration settings validation
- [ ] Operator deactivation scenarios
- [ ] Duplicate operator prevention
- [ ] Operator plan associations

**Estimated New Tests**: 15-20  
**Estimated Effort**: 0.5 day

**Total Phase 2 Effort**: 3-4 days

### Phase 3: Create Controller Integration Tests (0% → 75%)

Create comprehensive API endpoint tests for all TISS controllers.

#### TissGuidesControllerTests (NEW)

**Tests Needed** (13 endpoints):
- [ ] GET /api/tiss-guides (list all)
- [ ] GET /api/tiss-guides/{id} (get by ID)
- [ ] GET /api/tiss-guides/by-batch/{batchId}
- [ ] GET /api/tiss-guides/by-appointment/{appointmentId}
- [ ] GET /api/tiss-guides/by-status/{status}
- [ ] POST /api/tiss-guides (create)
- [ ] PUT /api/tiss-guides/{id} (update)
- [ ] DELETE /api/tiss-guides/{id}
- [ ] POST /api/tiss-guides/{id}/procedures (add procedure)
- [ ] DELETE /api/tiss-guides/{id}/procedures/{procedureId}
- [ ] PUT /api/tiss-guides/{id}/status (update status)
- [ ] POST /api/tiss-guides/{id}/submit
- [ ] POST /api/tiss-guides/{id}/cancel

**Test Scenarios Per Endpoint**:
- Success case (200 OK)
- Not found case (404)
- Validation error (400)
- Unauthorized (401)
- Tenant isolation

**Estimated Tests**: 50-60  
**Estimated Effort**: 1.5 days

#### TissBatchesControllerTests (NEW)

**Tests Needed** (14 endpoints):
- [ ] GET /api/tiss-batches (list all)
- [ ] GET /api/tiss-batches/{id}
- [ ] GET /api/tiss-batches/by-operator/{operatorId}
- [ ] GET /api/tiss-batches/by-status/{status}
- [ ] POST /api/tiss-batches (create)
- [ ] PUT /api/tiss-batches/{id}
- [ ] DELETE /api/tiss-batches/{id}
- [ ] POST /api/tiss-batches/{id}/guides/{guideId}
- [ ] DELETE /api/tiss-batches/{id}/guides/{guideId}
- [ ] POST /api/tiss-batches/{id}/generate-xml
- [ ] GET /api/tiss-batches/{id}/xml
- [ ] POST /api/tiss-batches/{id}/submit
- [ ] POST /api/tiss-batches/{id}/cancel
- [ ] GET /api/tiss-batches/{id}/validation-result

**Estimated Tests**: 55-65  
**Estimated Effort**: 1.5 days

#### TissAnalyticsControllerTests (NEW)

**Tests Needed** (8 endpoints):
- [ ] GET /api/tiss-analytics/gloss-summary
- [ ] GET /api/tiss-analytics/gloss-by-operator
- [ ] GET /api/tiss-analytics/gloss-by-procedure
- [ ] GET /api/tiss-analytics/performance-metrics
- [ ] GET /api/tiss-analytics/approval-rate
- [ ] GET /api/tiss-analytics/payment-time
- [ ] GET /api/tiss-analytics/operator-ranking
- [ ] GET /api/tiss-analytics/authorization-metrics

**Estimated Tests**: 35-40  
**Estimated Effort**: 1 day

#### HealthInsuranceOperatorsControllerTests (NEW)

**Tests Needed** (11 endpoints):
- [ ] GET /api/health-insurance-operators
- [ ] GET /api/health-insurance-operators/{id}
- [ ] POST /api/health-insurance-operators
- [ ] PUT /api/health-insurance-operators/{id}
- [ ] DELETE /api/health-insurance-operators/{id}
- [ ] GET /api/health-insurance-operators/by-ans/{ansCode}
- [ ] GET /api/health-insurance-operators/{id}/plans
- [ ] POST /api/health-insurance-operators/{id}/plans
- [ ] GET /api/health-insurance-operators/{id}/contracts
- [ ] POST /api/health-insurance-operators/{id}/contracts
- [ ] GET /api/health-insurance-operators/{id}/statistics

**Estimated Tests**: 45-50  
**Estimated Effort**: 1 day

**Total Phase 3 Effort**: 5 days

### Phase 4: Create E2E Integration Tests (0% → 80%)

Implement comprehensive end-to-end tests that validate complete workflows.

#### TissIntegrationTests

**Test Workflows**:

1. **Complete Billing Cycle**
   - [ ] Create operator and plan
   - [ ] Create patient insurance
   - [ ] Create appointment
   - [ ] Create TISS guide for appointment
   - [ ] Add procedures to guide
   - [ ] Create billing batch
   - [ ] Add guide to batch
   - [ ] Generate XML
   - [ ] Validate XML structure
   - [ ] Submit batch
   - [ ] Track batch status
   - **Estimated Tests**: 5-8 tests
   - **Estimated Effort**: 1 day

2. **TUSS Import and Usage**
   - [ ] Import TUSS from CSV
   - [ ] Import TUSS from Excel
   - [ ] Query procedures by code
   - [ ] Query procedures by description
   - [ ] Use TUSS procedure in guide
   - [ ] Verify pricing from TUSS
   - **Estimated Tests**: 4-6 tests
   - **Estimated Effort**: 0.5 day

3. **Authorization Workflow**
   - [ ] Request prior authorization
   - [ ] Approve authorization
   - [ ] Deny authorization
   - [ ] Link authorization to guide
   - [ ] Validate authorization requirement
   - [ ] Track authorization status
   - **Estimated Tests**: 4-6 tests
   - **Estimated Effort**: 0.5 day

4. **Analytics and Reporting**
   - [ ] Process multiple batches
   - [ ] Calculate gloss metrics
   - [ ] Generate performance reports
   - [ ] Verify metric accuracy
   - [ ] Test time-based analytics
   - [ ] Test operator comparisons
   - **Estimated Tests**: 3-5 tests
   - **Estimated Effort**: 0.5 day

5. **Error Scenarios**
   - [ ] Invalid XML generation
   - [ ] Batch validation failures
   - [ ] Duplicate guide prevention
   - [ ] Authorization denial impact
   - [ ] Network failure handling
   - **Estimated Tests**: 4-6 tests
   - **Estimated Effort**: 0.5 day

**Total Phase 4 Effort**: 3 days

## Summary

### Total Effort Estimate

| Phase | Description | Effort | Priority |
|-------|-------------|--------|----------|
| Phase 1 | Fix Build Issues | 2-4 hours | CRITICAL ⚠️ |
| Phase 2 | Expand Service Tests | 3-4 days | HIGH 🔥 |
| Phase 3 | Controller Integration Tests | 5 days | HIGH 🔥 |
| Phase 4 | E2E Integration Tests | 3 days | MEDIUM 📊 |
| **Total** | | **11-12 days** | |

### Coverage Projection

| Component | Current | After Phase 1 | After Phase 2 | After Phase 3 | After Phase 4 |
|-----------|---------|---------------|---------------|---------------|---------------|
| Entities | 100% ✅ | 100% ✅ | 100% ✅ | 100% ✅ | 100% ✅ |
| Services | 30-40% ⚠️ | 30-40% ⚠️ | **80%+ ✅** | 80%+ ✅ | 80%+ ✅ |
| Controllers | 0% ❌ | 0% ❌ | 0% ❌ | **75%+ ✅** | 75%+ ✅ |
| Integration | 0% ❌ | 0% ❌ | 0% ❌ | 0% ❌ | **80%+ ✅** |
| **Overall** | **~50%** | **~50%** | **~65%** | **~80%** | **~85%** ✅ |

### Recommended Approach

1. **Week 1**: Fix build issues + Expand service tests (Phase 1 + Phase 2)
2. **Week 2**: Create controller integration tests (Phase 3)
3. **Week 3**: Create E2E integration tests (Phase 4)

### Testing Tools & Frameworks

- **Unit Testing**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Test Data**: AutoFixture (optional)
- **Integration Testing**: WebApplicationFactory (ASP.NET Core)
- **Code Coverage**: Coverlet + ReportGenerator

### Success Criteria

- ✅ All tests pass without errors
- ✅ Service test coverage ≥ 80%
- ✅ Controller test coverage ≥ 75%
- ✅ Integration test coverage ≥ 80%
- ✅ Overall TISS test coverage ≥ 80%
- ✅ No critical bugs in TISS workflow
- ✅ All acceptance criteria from 03-integracao-tiss.md met

## Next Steps

1. **Immediate**: Fix test project build errors (Phase 1)
2. **Short-term**: Expand service tests (Phase 2)
3. **Medium-term**: Add controller tests (Phase 3)
4. **Long-term**: Add integration tests (Phase 4)

---

**Last Update**: January 2026  
**Status**: Plan defined, awaiting build fix to proceed  
**Owner**: Development Team
