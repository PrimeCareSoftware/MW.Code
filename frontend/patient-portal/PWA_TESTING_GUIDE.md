# PWA Testing Guide

## Prerequisites
- Node.js 18+ installed
- Chrome or Edge browser (for best PWA support)
- HTTPS enabled (or localhost for testing)

## Local Testing Setup

### 1. Build Production Version
```bash
npm run build
```

### 2. Serve Production Build
Since service workers require production mode, use a static server:

```bash
# Install http-server globally (if not installed)
npm install -g http-server

# Serve the production build
npx http-server dist/patient-portal/browser -p 4202 --gzip -c-1
```

### 3. Open in Browser
```
http://localhost:4202
```

**Note**: For full PWA testing on localhost, you may need to enable service workers in development mode.

## Testing Checklist

### ✅ Service Worker Registration
1. Open Chrome DevTools (F12)
2. Go to **Application** tab
3. Click **Service Workers** in left menu
4. Verify `ngsw-worker.js` is registered and activated
5. Check that status shows "activated and running"

**Expected**: Service worker registered with scope `/`

### ✅ Manifest Validation
1. In DevTools, go to **Application** tab
2. Click **Manifest** in left menu
3. Verify all fields are populated:
   - Name: "PrimeCare - Portal do Paciente"
   - Short name: "PrimeCare"
   - Start URL: "/"
   - Theme color: #1e40af
   - Icons: 8 icons displayed

**Expected**: No errors or warnings

### ✅ Cache Storage
1. In DevTools, go to **Application** tab
2. Click **Cache Storage** in left menu
3. Verify caches are created:
   - `ngsw:/:db:control`
   - `ngsw:/:active`
   - Asset caches

**Expected**: Caches created with app files

### ✅ Offline Functionality

#### Test 1: Basic Offline Access
1. Load the app normally
2. Open DevTools → Network tab
3. Select **Offline** from throttling dropdown
4. Refresh the page (F5)

**Expected**: 
- App still loads
- Orange offline indicator appears at top
- Previously cached pages work
- Network requests fail gracefully

#### Test 2: API Caching
1. Navigate to appointments page (while online)
2. Wait for data to load
3. Go offline (DevTools → Network → Offline)
4. Navigate away and back to appointments

**Expected**: Previously loaded appointments still display

#### Test 3: Network Recovery
1. While offline, try to load new data
2. Go back online
3. Refresh or navigate

**Expected**: 
- Offline indicator disappears
- Data loads successfully
- Failed requests retry

### ✅ Update Detection

#### Test Updates
1. Build the app (note version)
2. Serve and open in browser
3. Keep the app open
4. Change version in `package.json` (e.g., 0.0.0 → 0.0.1)
5. Rebuild: `npm run build`
6. Restart server
7. Wait or manually trigger update check

**Expected**: 
- Confirmation dialog appears
- "Nova versão disponível!" message
- Option to reload

#### Force Update Check
In browser console:
```javascript
// Get the SwUpdate service
// This requires modifying the app component temporarily
```

### ✅ Installation

#### Desktop Installation (Chrome/Edge)
1. Open app in Chrome/Edge
2. Look for install icon (⊕) in address bar
3. Click to install
4. Verify:
   - Standalone window opens
   - No browser UI (address bar, tabs)
   - App appears in Start Menu/Applications

**Alternative**: Custom install prompt at bottom of screen

#### Android Installation
1. Open app in Chrome
2. Look for "Add to Home Screen" banner or in menu
3. Tap to install
4. Verify:
   - Icon on home screen
   - Splash screen on launch
   - Standalone mode (no browser UI)
   - Theme color applied to status bar

#### iOS Installation
1. Open app in Safari
2. Tap Share button (⬆️)
3. Select "Add to Home Screen"
4. Verify:
   - Icon on home screen
   - Splash screen on launch
   - Status bar styled correctly

### ✅ Icons & Splash Screen

#### Verify Icons
1. Install app on device
2. Check home screen icon quality
3. Try different icon sizes

**Expected**: Sharp, clear icons at all sizes

#### Verify Splash Screen (Mobile)
1. Install app
2. Close app completely
3. Reopen from home screen

**Expected**: 
- White background
- PrimeCare icon
- Smooth transition to app

### ✅ Theme Color

#### Desktop
1. Install app
2. Check window title bar color

**Expected**: Blue (#1e40af) title bar

#### Mobile
1. Install app
2. Check status bar color
3. Check task switcher

**Expected**: Blue theme color applied

## Lighthouse Audit

### Run Audit
```bash
# Install Lighthouse
npm install -g lighthouse

# Run audit (with server running)
lighthouse http://localhost:4202 --output html --output-path ./lighthouse-report.html --view
```

### Expected Scores
- **PWA**: > 90 ✅
- **Performance**: > 85 ⚠️ (may be lower in dev)
- **Accessibility**: > 90 ✅
- **Best Practices**: > 90 ✅
- **SEO**: > 90 ✅

### PWA Checklist Items
All should pass:
- ✅ Registers a service worker
- ✅ Responds with 200 when offline
- ✅ Has a web app manifest
- ✅ Configured for custom splash screen
- ✅ Sets theme color
- ✅ Content is sized correctly for viewport
- ✅ Uses HTTPS
- ✅ Redirects HTTP traffic to HTTPS (in production)

## Browser DevTools Testing

### Service Worker Debugging
```javascript
// In console:

// Check registration
navigator.serviceWorker.getRegistrations()

// Check if SW is controlling page
navigator.serviceWorker.controller

// Unregister (for testing)
navigator.serviceWorker.getRegistrations().then(registrations => {
  registrations.forEach(r => r.unregister())
})
```

### Update Service Worker
In DevTools → Application → Service Workers:
1. Click "Update" button
2. Or check "Update on reload"
3. Or click "Skip waiting"

### Clear Data
In DevTools → Application:
1. Click "Clear storage" in left menu
2. Select all options
3. Click "Clear site data"
4. Refresh page

## Common Issues & Solutions

### Issue: Service Worker Not Registering
**Symptoms**: No SW in DevTools
**Solutions**:
- Ensure production build
- Check HTTPS/localhost
- Clear browser cache
- Check console for errors

### Issue: Offline Mode Not Working
**Symptoms**: App doesn't load offline
**Solutions**:
- Verify SW is active
- Check cache storage has files
- Ensure SW scope is correct
- Try hard refresh (Ctrl+Shift+R)

### Issue: Updates Not Detected
**Symptoms**: Old version keeps loading
**Solutions**:
- Unregister old SW
- Clear caches
- Hard refresh
- Check version changed

### Issue: Installation Not Available
**Symptoms**: No install prompt
**Solutions**:
- Must be HTTPS (or localhost)
- Check manifest is valid
- Verify all PWA criteria met
- May already be installed

### Issue: Icons Not Showing
**Symptoms**: Generic icon or broken image
**Solutions**:
- Verify icon paths in manifest
- Check icons exist in public/icons/
- Clear browser cache
- Verify icon format (PNG)

## Performance Testing

### Network Throttling
Test app performance on slow connections:
1. DevTools → Network tab
2. Select throttling profile:
   - Fast 3G
   - Slow 3G
   - Offline

### Measure Load Time
```javascript
// In console:
performance.timing.loadEventEnd - performance.timing.navigationStart
```

**Target**: < 3000ms on 3G

### Check Cache Hit Rate
1. Load app (clear cache first)
2. Check Network tab
3. Refresh page
4. Count requests from cache

**Expected**: > 90% from cache

## Mobile Device Testing

### Android Testing
1. Connect device via USB
2. Enable USB debugging
3. Open `chrome://inspect` in desktop Chrome
4. Select device and app
5. Use remote DevTools

### iOS Testing (Limited)
1. Open Safari on Mac
2. Connect iPhone via cable
3. Develop → [Device] → [App]
4. Use Web Inspector

## Automated Testing

### Playwright E2E Tests
Create PWA-specific tests:

```typescript
// e2e/pwa.spec.ts
import { test, expect } from '@playwright/test';

test('service worker registers', async ({ page }) => {
  await page.goto('http://localhost:4202');
  
  const swState = await page.evaluate(async () => {
    const registration = await navigator.serviceWorker.getRegistration();
    return registration?.active?.state;
  });
  
  expect(swState).toBe('activated');
});

test('manifest is valid', async ({ page }) => {
  await page.goto('http://localhost:4202');
  
  const manifest = await page.evaluate(async () => {
    const link = document.querySelector('link[rel="manifest"]');
    const response = await fetch(link.href);
    return response.json();
  });
  
  expect(manifest.name).toBe('PrimeCare - Portal do Paciente');
  expect(manifest.short_name).toBe('PrimeCare');
});

test('works offline', async ({ page, context }) => {
  // Load app online
  await page.goto('http://localhost:4202');
  await page.waitForLoadState('networkidle');
  
  // Go offline
  await context.setOffline(true);
  
  // Refresh and verify still works
  await page.reload();
  await expect(page.locator('app-root')).toBeVisible();
  
  // Check offline indicator
  await expect(page.locator('app-offline-indicator')).toBeVisible();
});
```

Run tests:
```bash
npm run e2e
```

## Production Testing

### Staging Environment
1. Deploy to staging with HTTPS
2. Test on real devices
3. Verify service worker works
4. Check update mechanism
5. Test installation flow

### Performance Monitoring
- Monitor cache hit rates
- Track service worker errors
- Measure update adoption
- Track installation rates

## Reporting Issues

When reporting PWA issues, include:
1. Browser/device info
2. Service worker state (DevTools screenshot)
3. Console errors
4. Network tab screenshot
5. Steps to reproduce

## Resources

- Chrome DevTools: https://developer.chrome.com/docs/devtools/
- Lighthouse: https://developers.google.com/web/tools/lighthouse
- PWA Checklist: https://web.dev/pwa-checklist/
- Service Worker Cookbook: https://serviceworke.rs/
