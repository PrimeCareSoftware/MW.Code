# PWA Quick Reference

## 🚀 Quick Start

### Build & Test Locally
```bash
# Build production version
npm run build

# Serve with service worker
npx http-server dist/patient-portal/browser -p 4202 -c-1

# Open browser
open http://localhost:4202
```

### Check Service Worker
1. Open DevTools (F12)
2. Application tab → Service Workers
3. Verify `ngsw-worker.js` is active

## 📦 What's Included

### Core Services
- **OfflineService**: Network status monitoring
- **PwaService**: Installation management  
- **SwUpdate**: Automatic version updates

### Components
- **OfflineIndicator**: Orange banner when offline
- **InstallPrompt**: Bottom sheet for installation

### Configuration
- **ngsw-config.json**: Caching strategies
- **manifest.webmanifest**: App metadata
- **angular.json**: Service worker enabled in production

## 🔧 Common Tasks

### Change Cache Duration
Edit `ngsw-config.json`:
```json
"dataGroups": [{
  "cacheConfig": {
    "maxAge": "6h"  // Change duration here
  }
}]
```

### Add New API Endpoint to Cache
Edit `ngsw-config.json`:
```json
"dataGroups": [{
  "urls": [
    "/api/patient/**",
    "/api/your-new-endpoint/**"  // Add here
  ]
}]
```

### Update App Version
1. Edit `package.json` version
2. Run `npm run build`
3. Deploy
4. Users get update notification

### Force Service Worker Update
In DevTools → Application → Service Workers:
- Click "Update" button
- Or check "Update on reload"

### Clear All Data
In DevTools → Application:
- Click "Clear storage"
- Select all
- Click "Clear site data"

## 🧪 Testing

### Test Offline Mode
1. Load app normally
2. DevTools → Network → Offline
3. Refresh page
4. ✅ App should still work

### Test Update Detection
1. Change version in `package.json`
2. Rebuild: `npm run build`
3. Keep browser open
4. ✅ Update prompt should appear

### Test Installation
**Desktop**: Look for install button in address bar
**Mobile**: "Add to Home Screen" banner

## 🐛 Troubleshooting

### Service Worker Not Registering
```bash
# Check it's production build
npm run build

# Clear browser data
# DevTools → Application → Clear storage

# Hard refresh
Ctrl + Shift + R
```

### Offline Not Working
```javascript
// Check in console:
navigator.serviceWorker.controller  // Should return object
```

### Updates Not Detected
```bash
# Unregister old service worker
# DevTools → Application → Service Workers → Unregister

# Clear caches
# DevTools → Application → Cache Storage → Delete all

# Rebuild and reload
npm run build
```

## 📊 Cache Strategies

| Type | Strategy | Use Case |
|------|----------|----------|
| **App Shell** | Prefetch | HTML, CSS, JS |
| **Assets** | Lazy | Images, fonts |
| **API (Fresh)** | Network-first | Real-time data |
| **API (Static)** | Cache-first | Rarely-changing data |

## 🔍 Debugging

### Check Registration
```javascript
navigator.serviceWorker.getRegistrations()
  .then(regs => console.log(regs))
```

### Check Cache Contents
```javascript
caches.keys().then(keys => console.log(keys))
caches.open('ngsw:/:active')
  .then(cache => cache.keys())
  .then(keys => console.log(keys))
```

### Monitor Network Status
```javascript
window.addEventListener('online', () => console.log('Online'))
window.addEventListener('offline', () => console.log('Offline'))
```

## 📱 Installation

### Desktop (Chrome/Edge)
- Install icon in address bar (⊕)
- Or custom prompt at bottom

### Android (Chrome)
- "Add to Home Screen" in menu
- Or browser banner
- Or custom prompt at bottom

### iOS (Safari)
- Share button (⬆️)
- "Add to Home Screen"

## 📚 Files to Know

```
frontend/patient-portal/
├── ngsw-config.json           ← Service worker config
├── public/
│   ├── manifest.webmanifest   ← App metadata
│   └── icons/                 ← PWA icons
├── src/
│   ├── index.html             ← PWA meta tags
│   └── app/
│       ├── app.ts             ← Update checking
│       ├── services/
│       │   ├── offline.service.ts  ← Network status
│       │   └── pwa.service.ts      ← Installation
│       └── shared/components/
│           ├── offline-indicator/  ← Offline UI
│           └── install-prompt/     ← Install UI
└── angular.json               ← Build config
```

## 🎯 Key URLs

- Manifest: `/manifest.webmanifest`
- Service Worker: `/ngsw-worker.js`
- Service Worker Config: `/ngsw.json` (generated)

## 💡 Tips

1. **Always test in production mode** - SW disabled in dev
2. **Use HTTPS** - Required for service workers
3. **Clear cache often** - During development
4. **Check version** - After each deploy
5. **Monitor errors** - In production console

## 📖 Documentation

- [PWA_IMPLEMENTATION.md](./PWA_IMPLEMENTATION.md) - Full details
- [PWA_TESTING_GUIDE.md](./PWA_TESTING_GUIDE.md) - Test procedures
- [PWA_SUMMARY.md](./PWA_SUMMARY.md) - Overview

## 🆘 Need Help?

1. Check browser console for errors
2. Review service worker status in DevTools
3. Verify production build
4. Check HTTPS/localhost
5. Clear browser data and retry

---

**Quick Command Reference**

```bash
# Development
npm start                    # Dev server (no SW)

# Production
npm run build               # Build with SW
npx http-server dist/... -p 4202  # Serve

# Testing
npx tsc --noEmit           # Type check
npm run e2e                # E2E tests
lighthouse http://localhost:4202  # PWA audit

# Debugging
# Open: chrome://inspect
# Or: chrome://serviceworker-internals
```
