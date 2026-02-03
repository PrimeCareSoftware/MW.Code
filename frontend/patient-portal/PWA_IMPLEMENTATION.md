# PWA Implementation - Patient Portal

## Overview
The Patient Portal has been configured as a Progressive Web App (PWA), enabling users to install it on their devices, work offline, and receive automatic updates.

## Features Implemented

### 1. Service Worker Configuration
- **File**: `ngsw-config.json`
- **Features**:
  - Automatic caching of app shell (HTML, CSS, JS)
  - Lazy loading of assets (images, fonts)
  - API response caching with configurable strategies
  - Background sync for failed requests
  - Automatic updates with user notification

### 2. Web App Manifest
- **File**: `public/manifest.webmanifest`
- **Configuration**:
  - App name: "Omni Care - Portal do Paciente"
  - Short name: "Omni Care"
  - Theme color: #1e40af (primary blue)
  - Background color: #ffffff
  - Display mode: standalone
  - Orientation: portrait-primary
  - Icons: 8 sizes (72x72 to 512x512)

### 3. Offline Support
- **Offline Service**: `src/app/services/offline.service.ts`
  - Monitors network status
  - Provides observable for online/offline state
  - Automatically detects connectivity changes

- **Offline Indicator Component**: `src/app/shared/components/offline-indicator/`
  - Visual indicator when offline
  - Appears at top of screen
  - Orange gradient background for visibility
  - Animated entry/exit

### 4. Version Management
- **App Component**: `src/app/app.ts`
  - Checks for updates every 6 hours
  - Notifies user when new version is available
  - Prompts for reload to apply updates
  - Handles unrecoverable service worker errors

### 5. Installation Prompt
- **PWA Service**: `src/app/services/pwa.service.ts`
  - Detects when app can be installed
  - Captures beforeinstallprompt event
  - Provides method to trigger installation
  - Tracks installation status

- **Install Prompt Component**: `src/app/shared/components/install-prompt/`
  - Bottom sheet UI for installation
  - "Install" and "Dismiss" actions
  - Only shows when installation is available
  - Responsive design for mobile/desktop

### 6. Meta Tags & Icons
- **File**: `src/index.html`
- **Added**:
  - Theme color meta tags
  - Apple mobile web app meta tags
  - iOS touch icons (8 sizes)
  - Manifest link
  - PWA status bar styling

## Caching Strategies

### Asset Groups
1. **App Shell** (prefetch)
   - index.html
   - CSS files
   - JavaScript files
   - manifest.webmanifest
   - favicon.ico

2. **Assets** (lazy, prefetch on update)
   - Images (all formats)
   - Fonts (otf, ttf, woff, woff2)
   - Icons

### Data Groups
1. **api-patient** (freshness strategy)
   - Patient data endpoints
   - Appointments
   - Documents
   - Cache: 50 items, 1 hour max age
   - Timeout: 10 seconds

2. **api-cache** (performance strategy)
   - Profile data
   - Medical history
   - Cache: 20 items, 6 hours max age
   - Timeout: 5 seconds

## Testing

### Local Testing
1. Build for production:
   ```bash
   npm run build
   ```

2. Serve the production build:
   ```bash
   npx http-server dist/patient-portal/browser -p 4202
   ```

3. Open Chrome DevTools:
   - Application tab → Service Workers
   - Application tab → Manifest
   - Network tab → Throttling (Offline)

### Lighthouse Audit
```bash
npm install -g lighthouse
lighthouse http://localhost:4202 --view
```

**Target Scores**:
- PWA: > 90
- Performance: > 85
- Accessibility: > 90
- Best Practices: > 90
- SEO: > 90

### Installation Testing

#### Desktop (Chrome/Edge)
1. Open app in browser
2. Look for install icon in address bar
3. Click to install
4. Verify app opens in standalone window

#### Android
1. Open app in Chrome
2. Tap "Add to Home Screen" or banner
3. Confirm installation
4. Verify icon on home screen
5. Launch and verify standalone mode

#### iOS
1. Open app in Safari
2. Tap Share button
3. Select "Add to Home Screen"
4. Confirm
5. Verify icon on home screen

## Configuration Files

### angular.json
- Service worker enabled for production builds
- `serviceWorker: "ngsw-config.json"` in production config

### package.json
- Added `@angular/service-worker` dependency
- Version: matches Angular version (20.x)

### ngsw-config.json
- Defines caching strategies
- Configures asset groups and data groups
- Sets navigation URLs

## Usage

### Service Worker Updates
The app automatically checks for updates:
- On startup
- Every 6 hours while running
- When user returns to the app

When an update is available:
1. User sees a confirmation dialog
2. User clicks "OK" to reload
3. New version is activated

### Offline Functionality
When offline:
- Orange banner appears at top
- Previously loaded pages still work
- Cached API responses are served
- Failed requests are queued
- Requests retry when back online

### Installation
Desktop users:
- See install button in browser
- Or see custom install prompt at bottom

Mobile users:
- Browser may show native install banner
- Or see custom install prompt at bottom

## Best Practices

### 1. Cache Management
- Cache expires after configured time
- Service worker cleans up old caches
- Don't cache sensitive user data

### 2. Update Strategy
- Always prompt user before reload
- Check for updates periodically
- Handle errors gracefully

### 3. Offline Experience
- Show clear offline indicator
- Provide meaningful error messages
- Cache critical user data
- Queue failed operations

### 4. Performance
- Lazy load non-critical assets
- Use performance strategy for static data
- Use freshness strategy for dynamic data
- Keep cache sizes reasonable

## Troubleshooting

### Service Worker Not Registering
- Check that app is served over HTTPS
- Verify production build
- Check browser console for errors
- Clear browser cache and try again

### Updates Not Working
- Check SwUpdate.isEnabled is true
- Verify version changed in package.json
- Clear service worker in DevTools
- Hard refresh (Ctrl+Shift+R)

### Offline Mode Not Working
- Verify network status is actually offline
- Check service worker is active
- Review caching configuration
- Check network tab in DevTools

### Installation Not Available
- Must be HTTPS (or localhost)
- Must meet PWA criteria (manifest, service worker, icons)
- User may have already installed
- Check browser support

## Browser Support

### Full Support
- Chrome/Edge 67+ (Desktop & Mobile)
- Safari 11.1+ (iOS)
- Firefox 44+ (Desktop)
- Samsung Internet 4+

### Partial Support
- Safari Desktop (limited features)
- Firefox Mobile (limited features)

### Not Supported
- Internet Explorer (any version)
- Old mobile browsers

## Security Considerations

1. **HTTPS Required**: Service workers only work over HTTPS
2. **Same-Origin**: Service worker must be same origin as app
3. **Scope**: Service worker scope is limited to its directory
4. **Cache Security**: Don't cache sensitive data
5. **Update Verification**: Verify update integrity before applying

## Maintenance

### Updating Service Worker
1. Modify `ngsw-config.json`
2. Rebuild app
3. Deploy
4. Service worker updates automatically

### Adding New Routes
- No changes needed
- Service worker handles all navigation URLs
- Update `navigationUrls` if needed

### Changing Cache Strategy
1. Edit `ngsw-config.json`
2. Adjust `strategy`, `maxAge`, `maxSize`
3. Rebuild and deploy

## Resources

- [Angular PWA Documentation](https://angular.io/guide/service-worker-intro)
- [Google PWA Guide](https://web.dev/progressive-web-apps/)
- [MDN Service Worker API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
- [Web App Manifest](https://developer.mozilla.org/en-US/docs/Web/Manifest)
