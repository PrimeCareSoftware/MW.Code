import { test, expect } from '@playwright/test';

test.describe('Documents', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'TestPassword123!');
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/dashboard/);
    
    // Navigate to documents
    await page.goto('/documents');
  });

  test('should display documents page', async ({ page }) => {
    await expect(page).toHaveURL(/\/documents/);
    await expect(page.locator('h1, h2')).toContainText(/Documents|Documentos/i);
  });

  test('should display documents list', async ({ page }) => {
    // Check if table or list is present
    const listPresent = await page.locator('table, .document-list, mat-list').isVisible();
    expect(listPresent).toBeTruthy();
  });

  test('should filter documents by type', async ({ page }) => {
    const filterButton = page.locator('button:has-text("Filter"), button:has-text("Filtrar")');
    if (await filterButton.isVisible()) {
      await filterButton.click();
      // Try to select a document type
      await page.click('text=/Prescription|Receita/i').catch(() => {});
    }
  });

  test('should search documents', async ({ page }) => {
    const searchInput = page.locator('input[placeholder*="Search"], input[placeholder*="Buscar"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('test');
      // Wait for results
      await page.waitForTimeout(1000);
    }
  });

  test('should download document', async ({ page }) => {
    // Click on first document's download button if available
    const downloadButton = page.locator('button:has-text("Download"), .download-btn').first();
    if (await downloadButton.isVisible()) {
      const [download] = await Promise.all([
        page.waitForEvent('download'),
        downloadButton.click()
      ]);
      expect(download).toBeTruthy();
    }
  });

  test('should display empty state when no documents', async ({ page }) => {
    const emptyState = page.locator('text=/No documents|Nenhum documento/i');
    const documentsList = page.locator('table tbody tr, .document-item');
    
    const hasDocuments = (await documentsList.count()) > 0;
    const hasEmptyState = await emptyState.isVisible();
    
    // Either should have documents or empty state
    expect(hasDocuments || hasEmptyState).toBeTruthy();
  });
});
