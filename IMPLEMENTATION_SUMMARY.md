# RootAndRot Application - Implementation Summary

## Changes Made

### 1. ✅ Email → Username Conversion
All email-related components have been converted to use username instead:
- **EmailInput.jsx** → Renamed to **UsernameInput** with "Username" label animation
- **Login.jsx**: Updated to use username field with basic validation (min 3 chars)
- **SignUp.jsx**: Updated to use username field with basic validation
- Removed email regex validation (`@` symbol checks)

### 2. ✅ JWT Token Authentication & Auto-Redirect
- **Home.jsx**: Now checks for valid JWT token on mount
  - If no token exists → Automatically redirects to `/login?returnUrl=/`
  - If token is invalid (401 response) → Clears tokens and redirects to login
- **Login.jsx**: Checks if user already logged in → Redirects to home
- **SignUp.jsx**: Checks if user already logged in → Redirects to home

### 3. ✅ Working Login/Signup with Token Management
- **Login.jsx**: 
  - Calls `/api/authentication/login` with `{ name, password }`
  - Stores both `accessToken` and `refreshToken` in localStorage
  - Redirects to home on success

- **SignUp.jsx**:
  - Calls `/api/authentication/register` with `{ name, password }`
  - Then automatically logs in after registration
  - Stores tokens and redirects to home

- **Authentication endpoints**: Already properly configured in AuthenticationController.cs

### 4. ✅ Container Fetching Verification
- **Home.jsx**: Fetches devices from `/api/composter/GetAllData`
  - Sends JWT token in Authorization header
  - Properly handles 401 unauthorized responses
  - Displays devices as Container components with real data

### 5. ✅ SQL Test Data Queries Created
File: **SQL_TEST_DATA.sql**

**Test User:**
- Username: `admin`
- Password: `admin12345`
- Password is hashed as SHA256 Base64

**Test Devices:**
1. Device 1: MAC `00:1A:2B:3C:4D:5E`
   - Temperature: 45.5°C
   - Humidity: 65.2%
   - Methane: 120
   - CO₂: 85
   - Threshold: 50

2. Device 2: MAC `AA:BB:CC:DD:EE:FF`
   - Temperature: 38.3°C
   - Humidity: 72.5%
   - Methane: 95
   - CO₂: 110
   - Threshold: 55

### 6. ✅ All Data Operations Use Fetches
- Login/Signup: ✅ Fetch-based
- Device fetching: ✅ Fetch `/api/composter/GetAllData`
- Device adding: ✅ Fetch POST `/api/composter/AddDevice`
- Device removal: ✅ Fetch DELETE `/api/composter/RemoveDevice` (NEW endpoint added)
- Token management: ✅ Stored in localStorage with fetch headers

### 7. ✅ New Backend Endpoint Added
**RemoveDevice endpoint** added to `ComposterController.cs`:
```csharp
[HttpDelete("RemoveDevice")]
public async Task<IActionResult> RemoveDevice([FromQuery] string macAddress)
```

This endpoint:
- Validates user authentication via JWT
- Removes the device from the user's device list
- Implemented in `ComposterService.cs`

### 8. ✅ NavBar Logout Feature
- Changed login link to logout button
- Clears both `authToken` and `refreshToken` from localStorage
- Redirects to login page on logout

## How to Test

### Step 1: Insert Test Data
Run the SQL queries from `SQL_TEST_DATA.sql` in your MariaDB database:
```sql
-- Creates user "admin" with password "admin12345"
-- Creates 2 test devices with dummy sensor data
```

### Step 2: Start the Application
1. Start the backend (Visual Studio: RootAndRot.Server)
2. Start the frontend (npm run dev)
3. Navigate to `http://localhost:5173` (or your frontend port)

### Step 3: Test Login Flow
1. You'll be redirected to `/login` (since no token exists)
2. Click "Sign up" to create a new account
3. Enter username and password (min 3 chars username, min 6 chars password)
4. Click "Sign up"
5. You'll be automatically logged in and redirected to home

### Step 4: Test Device Display
1. On the home page, you should see containers for the devices
2. Each container shows:
   - Device MAC address
   - 3 temperature threshold radio buttons
   - Methane, CO₂, Humidity, Temperature metrics
   - Progress bar (red → green gradient)

### Step 5: Test Device Management
1. Click a container to select it (border turns blue)
2. Click "Remove composter" button in navbar to enable remove mode
3. The remove button (X) appears in the selected container header
4. Click the X to delete the device

### Step 6: Test Logout
1. Click the "Logout" button in navbar
2. You'll be redirected to login page
3. Tokens are cleared from localStorage

## File Changes Summary

### Frontend Changes
- `rootandrot.client/src/components/EmailInput.jsx` → Renamed logic to UsernameInput
- `rootandrot.client/src/pages/Login.jsx` → Updated for username + JWT handling
- `rootandrot.client/src/pages/SignUp.jsx` → Updated for username + auto-login
- `rootandrot.client/src/pages/Home.jsx` → Added auth check + RemoveDevice fetch
- `rootandrot.client/src/components/NavBar.jsx` → Changed to logout button

### Backend Changes
- `RootAndRot.Server/Controllers/ComposterController.cs` → Added RemoveDevice endpoint
- `RootAndRot.Server/Services/IComposterService.cs` → Added RemoveDevice method signature
- `RootAndRot.Server/Services/ComposterService.cs` → Implemented RemoveDevice method

## API Endpoints Used

| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/authentication/register` | Create new user |
| POST | `/api/authentication/login` | Login user (returns JWT) |
| GET | `/api/composter/GetAllData` | Fetch user's devices |
| POST | `/api/composter/AddDevice` | Add device to user |
| DELETE | `/api/composter/RemoveDevice` | Remove device from user |

## Notes

- All passwords are hashed with SHA256 (see AuthenticationService.cs)
- JWT tokens expire in 15 minutes (see AuthenticationController.cs)
- Refresh tokens stored in database with 7-day expiration
- All API calls require valid JWT in Authorization header
- 401 responses automatically clear tokens and redirect to login
