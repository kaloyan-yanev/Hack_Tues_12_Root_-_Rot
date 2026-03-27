# Step-by-Step Testing Guide

## Prerequisites
- MariaDB running on localhost:3306
- Backend running on https://localhost:7123 (or configured port)
- Frontend running on http://localhost:5173 (or configured port)

## Step 1: Insert Test Data into Database

1. Open your MariaDB client (MySQL Workbench, phpMyAdmin, or CLI)
2. Select the `hacktues` database
3. Run all queries from `SQL_TEST_DATA.sql`
4. Verify by running the verification queries to see the created user and devices

**Credentials for testing:**
- Username: `admin`
- Password: `admin12345`

## Step 2: Test Authentication Flow

### Test Sign Up (New User)
1. Open browser to `http://localhost:5173`
2. You should be redirected to `/login`
3. Click "No account? Sign up"
4. Create new account:
   - Username: `testuser`
   - Password: `test12345` (min 6 chars)
5. Click "Sign up"
6. You should be automatically logged in and see the home page
7. You'll see an empty device list (new users have no devices)

### Test Login (Existing User)
1. Click "Sign in" link
2. Enter credentials:
   - Username: `admin`
   - Password: `admin12345`
3. Click "Sign in"
4. You should see the home page with 2 devices loaded

## Step 3: Test Device Display

On the home page with devices loaded, verify:

### Container Visual Elements
- [ ] Each device shows a container with 250px height
- [ ] Container has device MAC address in header
- [ ] Container shows 3 radio buttons for temperature threshold
- [ ] Container displays 4 metrics:
  - [ ] Methane (120% for first device)
  - [ ] CO₂ (85% for first device)
  - [ ] Humidity (65.2% for first device)
  - [ ] Temperature (45.5°C for first device)
- [ ] Progress bar shows with red→green gradient
- [ ] Containers stack vertically without gaps

### Container Selection
1. Click on a container
2. Border should turn blue (#3b82f6)
3. Background should lighten
4. Remove button (✕) should appear in header
5. Click outside container → deselected (border returns to gray)

## Step 4: Test Temperature Threshold

1. Select a container
2. Click the 3 radio buttons
3. Verify:
   - [ ] Only one can be selected at a time
   - [ ] Clicking a new one deselects the previous
   - [ ] First one defaults to selected
   - [ ] Visual feedback (blue checkmark) appears

## Step 5: Test Device Removal

### Enable Remove Mode
1. Click "Remove composter" button in navbar
2. Button should turn red and say "Remove mode ON"
3. All UI elements should have crosshair cursor

### Remove a Device
1. Click a container to select it (blue border)
2. Click the ✕ button in container header
3. Confirmation popup should appear asking "Remove Composter?"
4. Click "Yes, remove it"
5. Device should disappear from list
6. List refreshes to show remaining devices

### Cancel Remove
1. Click "Remove composter" button again to toggle off
2. Remove mode should deactivate
3. Container selections should clear
4. Cursor returns to normal

## Step 6: Test Device Addition

1. Click "+ Add composter" button
2. Input field should appear
3. Enter a MAC address (format: `XX:XX:XX:XX:XX:XX`)
   - Use an existing device MAC from database:
     - `00:1A:2B:3C:4D:5E` (first test device)
     - `AA:BB:CC:DD:EE:FF` (second test device)
4. Press Enter or click elsewhere
5. Success message appears ("✓ Success!")
6. Device should appear in the list if not already added

**Note:** Device must exist in the `Devices` table before it can be added to a user.

## Step 7: Test Logout

1. Click "Logout" button in top-right navbar
2. You should be redirected to `/login`
3. localStorage should be cleared (check browser DevTools → Application → LocalStorage)
4. All tokens should be gone

## Step 8: Test Session Persistence

1. Login with `admin` / `admin12345`
2. Refresh the page (F5)
3. You should stay logged in and devices should load
4. Check localStorage shows `authToken` and `refreshToken`

### Test Token Expiration
1. Open DevTools (F12)
2. Go to Application → LocalStorage
3. Delete the `authToken` entry
4. Refresh the page
5. You should be redirected to `/login`

## Step 9: Test Error Handling

### Invalid Credentials
1. Go to login page
2. Try: `admin` / `wrongpassword`
3. Error message should appear: "Invalid credentials"

### Invalid Username Format
1. Try signup with username `ab` (less than 3 chars)
2. Error should appear: "Username must be at least 3 characters"

### Invalid Password Format
1. Try signup with password `12345` (less than 6 chars)
2. Error should appear: "Password must be at least 6 characters"

### Duplicate Username
1. Try signup with `admin` (already exists)
2. Error should appear: "A user with that name already exists"

## Step 10: Monitor Network Requests

Open DevTools (F12) → Network tab and verify:

### Login Request
```
POST /api/authentication/login
Headers: Content-Type: application/json
Body: {"name":"admin","password":"admin12345"}
Response: {"accessToken":"...", "refreshToken":"..."}
```

### GetAllData Request
```
GET /api/composter/GetAllData
Headers: Authorization: Bearer <accessToken>
Response: [{"MAC":"00:1A:2B:3C:4D:5E","Temp":45.5, ...}]
```

### AddDevice Request
```
POST /api/composter/AddDevice
Headers: Authorization: Bearer <accessToken>
Body: {"MACAddress":"00:1A:2B:3C:4D:5E"}
```

### RemoveDevice Request
```
DELETE /api/composter/RemoveDevice?macAddress=00:1A:2B:3C:4D:5E
Headers: Authorization: Bearer <accessToken>
```

## Troubleshooting

### "Not authenticated" error
- [ ] Check if token is in localStorage
- [ ] Check token expiration (JWTs expire in 15 minutes)
- [ ] Clear localStorage and login again

### Devices not loading
- [ ] Verify user has devices in `UsersDevices` table
- [ ] Check Authorization header has valid token
- [ ] Check backend logs for errors

### Device addition fails
- [ ] Verify device MAC exists in `Devices` table
- [ ] Verify user has permission to add device
- [ ] Check MAC address format

### Remove operation fails
- [ ] Verify device is properly linked to user
- [ ] Check user has authorization to remove
- [ ] Verify device exists in database

## Expected Results Summary

✅ Signup creates new user with hashed password
✅ Login with correct credentials returns JWT tokens
✅ Devices display with real data from backend
✅ Containers are selectable (blue border)
✅ Remove mode works with crosshair cursor
✅ Device removal requires confirmation
✅ Logout clears tokens and redirects
✅ Unauthorized requests redirect to login
✅ All data operations use fetch API
✅ No email validation, only username
