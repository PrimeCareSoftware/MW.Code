# Public Clinic Listing Feature - Implementation Guide

## Overview
This feature allows clinics to be displayed on the public website with modern AI-styled design. Clinic owners can control their public visibility through their admin settings.

## Backend Changes

### 1. New Clinic Properties
- `ShowOnPublicSite` (bool): Controls whether the clinic is displayed publicly (default: false)
- `ClinicType` (enum): Classification of the clinic (Medical, Dental, Nutritionist, Psychology, PhysicalTherapy, Veterinary, Other)
- `WhatsAppNumber` (string, optional): WhatsApp contact number for public inquiries

### 2. Database Migration
Migration file: `20260121130859_AddClinicPublicDisplaySettings.cs`
- Adds three new columns to the Clinics table
- Includes index on ShowOnPublicSite for efficient filtering

### 3. API Endpoints

#### Public Endpoints (No Auth Required)
- `GET /api/public/clinics/search` - Search clinics with filters
  - Query parameters: name, city, state, clinicType, pageNumber, pageSize
  - Returns only clinics where ShowOnPublicSite = true

- `GET /api/public/clinics/{id}` - Get specific clinic details

#### Owner Endpoints (Auth Required)
- `GET /api/ClinicAdmin/public-display-settings` - Get current settings
- `PUT /api/ClinicAdmin/public-display-settings` - Update settings
  - Request body:
    ```json
    {
      "showOnPublicSite": true,
      "clinicType": "Medical",
      "whatsAppNumber": "+5511999999999"
    }
    ```

## Frontend Changes

### 1. Enhanced Clinic Search Page
Location: `/frontend/medicwarehouse-app/src/app/pages/site/clinics/clinic-search.*`

**Features:**
- AI-styled modern design with gradients and animations
- Clinic type filter dropdown
- WhatsApp integration with pre-filled messages
- Direct contact buttons (WhatsApp, Phone, Email)
- Responsive mobile-friendly layout
- Smooth hover effects and transitions

**Clinic Types Supported:**
- Médica (Medical)
- Odontológica (Dental)
- Nutricionista (Nutritionist)
- Psicologia (Psychology)
- Fisioterapia (Physical Therapy)
- Veterinária (Veterinary)
- Outros (Other)

### 2. Contact Options
Each clinic card displays:
- **WhatsApp button**: Opens WhatsApp with pre-filled message
- **Phone button**: Direct tel: link
- **Email button**: Direct mailto: link
- **Schedule button**: Navigates to appointment booking

## Design Features

### AI-Styled Elements
1. **Gradient Backgrounds**: Purple/blue gradients throughout
2. **Smooth Animations**: Fade-in, slide-up effects
3. **Modern Cards**: Rounded corners, soft shadows
4. **Interactive Hovers**: Transform and shadow effects
5. **Typography**: Bold headings with gradient text
6. **Icons**: Emoji-based icons for visual appeal

### Color Palette
- Primary: `#667eea` to `#764ba2` (gradient)
- Success: `#25d366` (WhatsApp green)
- Background: `#f5f7fa` to `#e8f0fe` (gradient)
- Text: `#1e293b` (dark), `#64748b` (muted)

## Usage Guide for Clinic Owners

### Enabling Public Display

1. **Login** to the clinic admin panel
2. **Navigate** to Settings → Public Display
3. **Select** your clinic type
4. **Enter** WhatsApp number (optional but recommended)
5. **Toggle** "Show on public site" to ON
6. **Save** changes

### WhatsApp Integration
- Format: Include country code (e.g., +55 for Brazil)
- Example: +5511999999999
- When visitors click WhatsApp button, they'll see:
  ```
  Olá! Gostaria de agendar uma consulta na [Your Clinic Name].
  ```

## Security & Privacy

### LGPD Compliance
- Only minimal, publicly-appropriate information is displayed
- No sensitive data (full CNPJ, internal IDs, etc.) is exposed
- Clinics must explicitly opt-in (ShowOnPublicSite = false by default)
- Clinic owners have full control over their public visibility

### Data Displayed Publicly
- ✅ Clinic name and trade name
- ✅ Address, city, state
- ✅ Phone and email (public contact)
- ✅ Operating hours
- ✅ Clinic type
- ✅ WhatsApp number (if provided)
- ❌ CNPJ (document)
- ❌ Internal IDs
- ❌ Financial information
- ❌ Patient data

## Testing

### Manual Testing Steps
1. Create or use an existing clinic
2. Enable public display via API or admin panel
3. Visit `/site/clinics` page
4. Verify clinic appears in search results
5. Test all filters (name, city, state, type)
6. Test contact buttons (WhatsApp, phone, email)
7. Verify mobile responsive design

### API Testing
```bash
# Search clinics
curl -X GET "http://localhost:5000/api/public/clinics/search?clinicType=Medical&city=São Paulo"

# Get clinic details
curl -X GET "http://localhost:5000/api/public/clinics/{clinicId}"

# Update settings (requires auth)
curl -X PUT "http://localhost:5000/api/ClinicAdmin/public-display-settings" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"showOnPublicSite": true, "clinicType": "Medical", "whatsAppNumber": "+5511999999999"}'
```

## Future Enhancements

### Planned Features
- [ ] Star ratings and reviews
- [ ] Advanced search with distance/geolocation
- [ ] Appointment scheduling integration
- [ ] Clinic photos and gallery
- [ ] Doctor profiles and specialties
- [ ] Insurance acceptance display
- [ ] Operating hours by day of week
- [ ] Holiday/vacation schedule

### Technical Improvements
- [ ] Unit tests for backend endpoints
- [ ] E2E tests for frontend
- [ ] Performance optimization for large datasets
- [ ] Caching layer for frequently accessed data
- [ ] Search result relevance scoring

## Troubleshooting

### Clinic Not Appearing in Search
1. Verify `ShowOnPublicSite` is set to `true`
2. Check `IsActive` is `true`
3. Ensure filters match clinic attributes
4. Clear browser cache

### WhatsApp Button Not Working
1. Verify WhatsAppNumber format includes country code
2. Check phone number has only digits (no spaces/dashes)
3. Test on mobile device (WhatsApp app required)

### Build Errors
- Run `npm install --legacy-peer-deps` in frontend directory
- Ensure Angular CLI version matches project requirements
- Check for TypeScript errors in modified files

## Support

For questions or issues:
1. Check this documentation first
2. Review API endpoints in Swagger/OpenAPI docs
3. Contact development team

## License

Copyright © 2026 Omni Care Software. All rights reserved.
