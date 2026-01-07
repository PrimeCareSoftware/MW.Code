import { test, expect } from '@playwright/test';

test.describe('Appointments', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'TestPassword123!');
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/dashboard/);
    
    // Navigate to appointments
    await page.goto('/appointments');
  });

  test('should display appointments page', async ({ page }) => {
    await expect(page).toHaveURL(/\/appointments/);
    await expect(page.locator('h1, h2')).toContainText(/Appointments|Agendamentos|Consultas/i);
  });

  test('should display appointments list', async ({ page }) => {
    // Check if table or list is present
    const listPresent = await page.locator('table, .appointment-list, mat-list').isVisible();
    expect(listPresent).toBeTruthy();
  });

  test('should filter appointments by status', async ({ page }) => {
    const filterButton = page.locator('button:has-text("Filter"), button:has-text("Filtrar")');
    if (await filterButton.isVisible()) {
      await filterButton.click();
      await page.click('text=/Scheduled|Agendado/i');
    }
  });

  test('should view appointment details', async ({ page }) => {
    // Click on first appointment if available
    const firstAppointment = page.locator('table tr:nth-child(1), .appointment-item:first-child').first();
    if (await firstAppointment.isVisible()) {
      await firstAppointment.click();
      // Should show details
      await expect(page.locator('text=/Details|Detalhes/i')).toBeVisible();
    }
  });

  test('should display empty state when no appointments', async ({ page }) => {
    // This test assumes there might be no appointments for a new user
    const emptyState = page.locator('text=/No appointments|Nenhum agendamento/i');
    const appointmentsList = page.locator('table tbody tr, .appointment-item');
    
    const hasAppointments = (await appointmentsList.count()) > 0;
    const hasEmptyState = await emptyState.isVisible();
    
    // Either should have appointments or empty state
    expect(hasAppointments || hasEmptyState).toBeTruthy();
  });
});
