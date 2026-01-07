import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'TestPassword123!');
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/dashboard/);
  });

  test('should display dashboard after login', async ({ page }) => {
    await expect(page).toHaveURL(/\/dashboard/);
    await expect(page.locator('h1, h2')).toContainText(/Dashboard|Painel/i);
  });

  test('should display user welcome message', async ({ page }) => {
    await expect(page.locator('text=/Welcome|Bem-vindo/i')).toBeVisible();
  });

  test('should navigate to appointments', async ({ page }) => {
    await page.click('text=/Appointments|Agendamentos|Consultas/i');
    await expect(page).toHaveURL(/\/appointments/);
  });

  test('should navigate to documents', async ({ page }) => {
    await page.click('text=/Documents|Documentos/i');
    await expect(page).toHaveURL(/\/documents/);
  });

  test('should navigate to profile', async ({ page }) => {
    await page.click('text=/Profile|Perfil/i');
    await expect(page).toHaveURL(/\/profile/);
  });

  test('should logout successfully', async ({ page }) => {
    await page.click('text=/Logout|Sair/i');
    await expect(page).toHaveURL(/\/login/);
  });
});
