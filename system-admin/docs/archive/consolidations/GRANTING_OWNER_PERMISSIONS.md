# Granting Owner Permissions to Other Users

## Overview

The owner permission can be granted to any user by the current clinic owner. This allows multiple people to manage the clinic with full administrative privileges, which is useful for:

- Clinics with multiple partners/owners
- Delegating administrative responsibilities
- Temporary administrative access for specific personnel

## How to Grant Owner Permissions

### Method 1: Creating a New User with Owner Role

1. **Login as Owner**
   - Access the application
   - Use owner credentials
   - ✅ Check "Login como Proprietário" (Login as Owner)

2. **Navigate to User Management**
   - Click "Administração" in the navigation bar
   - Select "Usuários" (Users)

3. **Create New User**
   - Click "Novo Usuário" (New User) or "Adicionar Usuário" (Add User)
   - Fill in the required information:
     - Full Name
     - Email
     - Phone
     - Username
     - Password
     - **Role: Select "Owner"** ⭐

4. **Save**
   - The new user will now have full owner privileges
   - Share the credentials securely with the new owner
   - Inform them to use "Login como Proprietário" when logging in

### Method 2: Changing an Existing User's Role to Owner

1. **Login as Owner**
   - Access the application with owner credentials

2. **Navigate to User Management**
   - Click "Administração" → "Usuários"

3. **Select User to Promote**
   - Find the user in the list
   - Click "Editar" (Edit) or the user's details

4. **Change Role**
   - In the edit form, find the "Role" or "Função" field
   - Change from current role (e.g., "Doctor", "Secretary") to **"Owner"**

5. **Save Changes**
   - The user now has owner privileges
   - Inform the user to:
     - Logout if currently logged in
     - Login again
     - Use "Login como Proprietário" option

## What Owners Can Do

Users with Owner role have **complete access** to all system functionality:

### ✅ Administrative Functions
- Create, edit, and delete users
- Create and manage access profiles
- Change user roles (including granting/revoking owner status)
- Configure clinic settings
- Manage subscription and billing

### ✅ Operational Functions
- All functions available to regular users:
  - Patient management
  - Appointments
  - Medical records
  - Prescriptions
  - Payments
  - Reports

### ✅ Menu Access
Owners see the "Administração" (Administration) menu with:
- Usuários (User Management)
- Perfis de Acesso (Access Profiles)
- Informações da Clínica (Clinic Information)
- Personalização (Customization)
- Assinatura (Subscription)

## Security Considerations

### Best Practices

1. **Limit Owner Accounts**
   - Only grant owner status to trusted individuals
   - Typically: clinic partners, medical directors, or chief administrators

2. **Document Owner Access**
   - Keep a record of who has owner access
   - Review periodically

3. **Use Strong Passwords**
   - Require complex passwords for owner accounts
   - Consider implementing 2FA (if available)

4. **Audit Owner Actions**
   - Review owner actions regularly
   - Check user creation/modification logs

5. **Revoke When Necessary**
   - If someone leaves the organization
   - If role changes
   - Change their role from "Owner" to appropriate level

### Revoking Owner Status

To remove owner privileges from a user:

1. Login as Owner
2. Navigate to Administração → Usuários
3. Find the user
4. Click "Editar" (Edit)
5. Change Role from "Owner" to appropriate role (e.g., "Doctor", "Secretary")
6. Save

**Important:** The user's next login will use the new role. They should NOT check "Login como Proprietário" anymore.

## Technical Implementation

### Frontend
- Owner check: `user.role === 'Owner' || user.isSystemOwner === true`
- UI shows admin menu only when `isOwner()` returns true

### Backend
- `RequirePermissionKeyAttribute` automatically grants all permissions to owners
- `ownerGuard` protects admin routes
- Owner role bypasses granular permission checks

### Database
```sql
-- User table stores the role
UPDATE Users 
SET Role = 'Owner'  -- or 'Doctor', 'Secretary', etc.
WHERE Id = @userId AND TenantId = @tenantId;
```

## Multiple Owners Scenario

### Example: Clinic with 3 Partners

**Setup:**
- Dr. João (Initial Owner) - Created during clinic registration
- Dr. Maria (Partner) - Promoted to Owner
- Dr. Carlos (Partner) - Promoted to Owner

**Process:**
1. Dr. João registers clinic → Automatically becomes Owner
2. Dr. João creates user account for Dr. Maria with Role = "Owner"
3. Dr. João creates user account for Dr. Carlos with Role = "Owner"
4. All three can now:
   - Manage users
   - Configure clinic
   - Access all functions

### Benefits of Multiple Owners
- Shared administrative responsibilities
- No single point of failure
- Flexible management structure
- Each partner can manage the clinic independently

## Troubleshooting

### Problem: User Can't See Admin Menu After Being Made Owner

**Cause:** User is still logged in with old role cached

**Solution:**
1. User should logout completely
2. Login again with "Login como Proprietário" checked
3. Admin menu should now appear

### Problem: Access Denied When Trying to Create Users

**Cause:** Not logged in as owner or role not properly set

**Solution:**
1. Verify user's role in database: `SELECT Role FROM Users WHERE Id = @userId`
2. Ensure role is exactly "Owner" (case-sensitive)
3. Logout and login again with owner option checked

### Problem: Can't Change Another Owner's Role

**Cause:** System protects against accidentally removing all owners

**Solution:**
- This is expected behavior for some systems
- If implemented, ensure at least one owner remains
- Contact system administrator if stuck

## API Endpoints (Technical Reference)

### Check Current User's Role
```http
GET /api/users/me
Authorization: Bearer {token}

Response:
{
  "id": "...",
  "username": "...",
  "role": "Owner",
  "isSystemOwner": false
}
```

### Update User Role (Owner Only)
```http
PUT /api/users/{userId}/role
Authorization: Bearer {token}
Content-Type: application/json

{
  "newRole": "Owner"
}
```

### List All Users (Owner Only)
```http
GET /api/users
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "username": "dr.joao",
    "fullName": "Dr. João Silva",
    "role": "Owner"
  },
  {
    "id": "...",
    "username": "dr.maria",
    "fullName": "Dra. Maria Santos",
    "role": "Owner"
  }
]
```

## Conclusion

The ability to grant owner permissions ensures flexible clinic management while maintaining security. Use this feature responsibly and follow best practices to maintain a secure and well-managed system.

---

**Last Updated:** January 14, 2026  
**Related Documents:**
- `OWNER_FIRST_LOGIN_GUIDE.md`
- `OWNER_MENU_FIX.md`
- `ACCESS_PROFILES_DOCUMENTATION.md`
