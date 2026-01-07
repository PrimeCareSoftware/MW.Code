import { test, expect } from '@playwright/test';

test.describe('Authentication Flow', () => {
  const testEmail = `test${Date.now()}@example.com`;
  const testPassword = 'TestPassword123!';
  const testCPF = '12345678901';
  const testName = 'Test User';

  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('should display login page', async ({ page }) => {
    await expect(page).toHaveTitle(/Patient Portal/);
    await expect(page.locator('h1')).toContainText(/Login|Entrar/i);
  });

  test('should navigate to registration page', async ({ page }) => {
    await page.click('text=/Register|Registrar|Criar conta/i');
    await expect(page).toHaveURL(/\/register/);
    await expect(page.locator('h1')).toContainText(/Register|Registrar|Criar conta/i);
  });

  test('should register a new user', async ({ page }) => {
    // Navigate to registration page
    await page.click('text=/Register|Registrar|Criar conta/i');
    
    // Fill registration form
    await page.fill('input[name="email"]', testEmail);
    await page.fill('input[name="cpf"]', testCPF);
    await page.fill('input[name="fullName"]', testName);
    await page.fill('input[name="password"]', testPassword);
    await page.fill('input[name="confirmPassword"]', testPassword);
    await page.fill('input[name="phoneNumber"]', '+55 11 98765-4321');
    
    // Submit form
    await page.click('button[type="submit"]');
    
    // Should redirect to dashboard
    await expect(page).toHaveURL(/\/dashboard/, { timeout: 10000 });
  });

  test('should login with email and password', async ({ page }) => {
    // First, register a user
    await page.goto('/register');
    await page.fill('input[name="email"]', testEmail);
    await page.fill('input[name="cpf"]', testCPF);
    await page.fill('input[name="fullName"]', testName);
    await page.fill('input[name="password"]', testPassword);
    await page.fill('input[name="confirmPassword"]', testPassword);
    await page.fill('input[name="phoneNumber"]', '+55 11 98765-4321');
    await page.click('button[type="submit"]');
    
    // Wait for dashboard
    await page.waitForURL(/\/dashboard/);
    
    // Logout
    await page.click('text=/Logout|Sair/i');
    await page.waitForURL(/\/login/);
    
    // Login again
    await page.fill('input[name="email"]', testEmail);
    await page.fill('input[name="password"]', testPassword);
    await page.click('button[type="submit"]');
    
    // Should redirect to dashboard
    await expect(page).toHaveURL(/\/dashboard/, { timeout: 10000 });
  });

  test('should show error for invalid credentials', async ({ page }) => {
    await page.fill('input[name="email"]', 'invalid@example.com');
    await page.fill('input[name="password"]', 'WrongPassword123!');
    await page.click('button[type="submit"]');
    
    // Should show error message
    await expect(page.locator('text=/Invalid|Inválido|Error|Erro/i')).toBeVisible({ timeout: 5000 });
  });

  test('should validate password requirements', async ({ page }) => {
    await page.goto('/register');
    
    // Try weak password
    await page.fill('input[name="email"]', testEmail);
    await page.fill('input[name="password"]', 'weak');
    await page.fill('input[name="confirmPassword"]', 'weak');
    
    // Should show validation error
    await page.click('button[type="submit"]');
    await expect(page.locator('text=/password|senha/i')).toBeVisible();
  });

  test('should validate password confirmation', async ({ page }) => {
    await page.goto('/register');
    
    // Passwords don't match
    await page.fill('input[name="email"]', testEmail);
    await page.fill('input[name="password"]', testPassword);
    await page.fill('input[name="confirmPassword"]', 'DifferentPassword123!');
    
    // Should show validation error
    await page.click('button[type="submit"]');
    await expect(page.locator('text=/match|corresponder/i')).toBeVisible();
  });
});
