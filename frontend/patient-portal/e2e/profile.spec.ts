import { test, expect } from '@playwright/test';

test.describe('Profile Management', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'TestPassword123!');
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/dashboard/);
    
    // Navigate to profile
    await page.goto('/profile');
  });

  test('should display profile page', async ({ page }) => {
    await expect(page).toHaveURL(/\/profile/);
    await expect(page.locator('h1, h2')).toContainText(/Profile|Perfil/i);
  });

  test('should display user information', async ({ page }) => {
    // Check if email is displayed
    await expect(page.locator('text=/test@example.com/i')).toBeVisible();
  });

  test('should allow editing profile information', async ({ page }) => {
    const editButton = page.locator('button:has-text("Edit"), button:has-text("Editar")');
    if (await editButton.isVisible()) {
      await editButton.click();
      
      // Should enable form fields
      const phoneInput = page.locator('input[name="phoneNumber"]');
      await expect(phoneInput).toBeEnabled();
    }
  });

  test('should update phone number', async ({ page }) => {
    const editButton = page.locator('button:has-text("Edit"), button:has-text("Editar")');
    if (await editButton.isVisible()) {
      await editButton.click();
      
      const phoneInput = page.locator('input[name="phoneNumber"]');
      await phoneInput.fill('+55 11 91234-5678');
      
      const saveButton = page.locator('button:has-text("Save"), button:has-text("Salvar")');
      await saveButton.click();
      
      // Should show success message
      await expect(page.locator('text=/Success|Sucesso/i')).toBeVisible({ timeout: 5000 });
    }
  });

  test('should change password', async ({ page }) => {
    const changePasswordButton = page.locator('button:has-text("Change Password"), button:has-text("Alterar Senha")');
    if (await changePasswordButton.isVisible()) {
      await changePasswordButton.click();
      
      // Fill password form
      await page.fill('input[name="currentPassword"]', 'TestPassword123!');
      await page.fill('input[name="newPassword"]', 'NewPassword123!');
      await page.fill('input[name="confirmNewPassword"]', 'NewPassword123!');
      
      const submitButton = page.locator('button[type="submit"]');
      await submitButton.click();
      
      // Should show success message
      await expect(page.locator('text=/Success|Sucesso/i')).toBeVisible({ timeout: 5000 });
    }
  });

  test('should display account security information', async ({ page }) => {
    // Check for security-related information
    const securitySection = page.locator('text=/Security|Segurança/i');
    if (await securitySection.isVisible()) {
      await expect(page.locator('text=/Last login|Último acesso/i')).toBeVisible();
    }
  });
});
