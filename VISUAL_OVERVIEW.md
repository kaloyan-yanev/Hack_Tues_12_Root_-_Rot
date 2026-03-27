# RootAndRot Application - Visual Overview

## Application Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                    RootAndRot Application Flow                      │
└─────────────────────────────────────────────────────────────────────┘

                              START
                                │
                                ▼
                    ┌──────────────────────┐
                    │ Check for JWT Token  │
                    └──────────────────────┘
                           │
                ┌──────────┴──────────┐
                │                     │
            TOKEN EXISTS          NO TOKEN
                │                     │
                ▼                     ▼
          ┌──────────┐          ┌──────────┐
          │  HOME    │          │  LOGIN   │
          │  PAGE    │          │  PAGE    │
          └──────────┘          └──────────┘
                │                     │
                │         ┌───────────┴────────────┐
                │         │                        │
                │         ▼                        ▼
                │    ┌─────────────┐         ┌──────────────┐
                │    │ Sign In     │         │ Sign Up      │
                │    │ (Existing   │         │ (New User)   │
                │    │  User)      │         │              │
                │    └─────────────┘         └──────────────┘
                │         │                        │
                │         │ Valid Credentials      │ Create User
                │         └────────────┬───────────┘
                │                      │
                │         ┌────────────▼──────────┐
                │         │  Request Token Pair   │
                │         │  POST /login or       │
                │         │  POST /register       │
                │         └────────────┬──────────┘
                │                      │
                │         ┌────────────▼──────────┐
                │         │ Store in localStorage │
                │         │ - accessToken         │
                │         │ - refreshToken        │
                │         └────────────┬──────────┘
                │                      │
                └──────────┬───────────┘
                           │
                           ▼
                    ┌──────────────────┐
                    │  Load Devices    │
                    │ GET /GetAllData  │
                    │ (with Bearer)    │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ Display Device   │
                    │ Containers       │
                    └────────┬─────────┘
                             │
                ┌────────────┼────────────┐
                │            │            │
                ▼            ▼            ▼
            ┌────────┐  ┌────────┐  ┌────────┐
            │SELECT  │  │VIEW    │  │MANAGE  │
            │DEVICE  │  │DATA    │  │REMOVE  │
            └────────┘  └────────┘  └────────┘
                │
                ▼
            ┌────────────────────┐
            │ Logout             │
            │ Clear localStorage │
            │ Redirect to login  │
            └────────────────────┘
```

---

## Data Flow Diagram

```
┌────────────────────────────────────────────────────────────────┐
│                      CLIENT SIDE (React)                       │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌──────────────┐                                            │
│  │ Login/Signup │                                            │
│  │ Component    │                                            │
│  └──────┬───────┘                                            │
│         │                                                    │
│         │ fetch(POST)                                       │
│         │ {name, password}                                  │
│         ▼                                                    │
│  ┌──────────────────┐        ┌──────────────────────┐       │
│  │ Store Token in   │──────▶ │ LocalStorage         │       │
│  │ localStorage     │        │ - authToken          │       │
│  └──────────────────┘        │ - refreshToken       │       │
│         │                    └──────────────────────┘       │
│         │ Redirect to Home                                  │
│         ▼                                                    │
│  ┌──────────────────┐                                       │
│  │ Home Component   │                                       │
│  │ useEffect()      │                                       │
│  └────────┬─────────┘                                       │
│           │                                                 │
│           │ Check localStorage.authToken                    │
│           │ if (!token) redirect to /login                  │
│           │                                                 │
│           │ fetch(GET) with Bearer token                    │
│           ▼                                                 │
│  ┌──────────────────────────────────────┐                   │
│  │ /api/composter/GetAllData            │                   │
│  │ Headers: Authorization: Bearer<token>│                   │
│  └────────┬─────────────────────────────┘                   │
│           │                                                 │
│           │ Response: [{MAC, Temp, ...}]                    │
│           ▼                                                 │
│  ┌──────────────────────┐                                   │
│  │ Render Containers    │                                   │
│  │ For Each Device      │                                   │
│  └────────┬─────────────┘                                   │
│           │                                                 │
│  ┌────────┴────────────────────────┐                        │
│  │                                 │                        │
│  ▼                                 ▼                        │
│ ┌──────────────┐            ┌──────────────┐              │
│ │ Select/Deselect          │ Remove Device│              │
│ │ (Border Color)           │              │              │
│ └──────────────┘            └──────┬───────┘              │
│                                    │                      │
│                        fetch(DELETE) with Bearer          │
│                        /RemoveDevice?mac=...              │
│                                    │                      │
│                                    ▼                      │
│                        Refresh: GET /GetAllData           │
│                                                          │
└────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP Requests
                              │ (with JWT Authorization)
                              ▼
┌────────────────────────────────────────────────────────────────┐
│                   SERVER SIDE (ASP.NET Core)                   │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  POST /api/authentication/login                               │
│  ├─▶ AuthenticationController.LogIn()                        │
│  ├─▶ AuthenticationService.LogIn(name, password)             │
│  ├─▶ Find User, Verify Password (SHA256)                     │
│  ├─▶ IssueTokenPair(user)                                    │
│  └─▶ Return { accessToken, refreshToken }                    │
│                                                                │
│  POST /api/authentication/register                            │
│  ├─▶ AuthenticationController.Register()                     │
│  ├─▶ AuthenticationService.Register(name, password)          │
│  ├─▶ Hash Password (SHA256)                                  │
│  ├─▶ Create User in Database                                 │
│  └─▶ Return 200 OK                                           │
│                                                                │
│  GET /api/composter/GetAllData                               │
│  ├─▶ [Authorize] ComposterController                         │
│  ├─▶ Extract JWT token (NameIdentifier claim)                │
│  ├─▶ ComposterService.GetAllDataPerProfile(username)         │
│  ├─▶ Query Users table JOIN Devices                          │
│  ├─▶ DeviceDataDTO.FromDevice()                              │
│  └─▶ Return [ {...}, {...}, ... ]                            │
│                                                                │
│  POST /api/composter/AddDevice                               │
│  ├─▶ [Authorize] ComposterController                         │
│  ├─▶ Extract JWT token (NameIdentifier claim)                │
│  ├─▶ ComposterService.AddDevice(MAC, username)               │
│  ├─▶ Find User, Find Device by MAC                           │
│  ├─▶ Add Device to User.Devices collection                   │
│  ├─▶ SaveChangesAsync()                                      │
│  └─▶ Return 200 OK                                           │
│                                                                │
│  DELETE /api/composter/RemoveDevice                           │
│  ├─▶ [Authorize] ComposterController                         │
│  ├─▶ Extract JWT token (NameIdentifier claim)                │
│  ├─▶ ComposterService.RemoveDevice(MAC, username)            │
│  ├─▶ Find User, Find Device by MAC                           │
│  ├─▶ Remove Device from User.Devices collection              │
│  ├─▶ SaveChangesAsync()                                      │
│  └─▶ Return 200 OK                                           │
│                                                                │
└────────────────────────────────────────────────────────────────┘
                              │
                              │ EF Core Queries
                              │
                              ▼
┌────────────────────────────────────────────────────────────────┐
│                   DATABASE (MariaDB)                           │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  Users                  UsersDevices            Devices        │
│  ┌─────────┐            ┌─────────┐            ┌─────────┐   │
│  │ UserID  │────────┐   │ UserID  │───────┬───│ DeviceID│   │
│  │ Name    │        │   │ DeviceID│       │   │ MAC     │   │
│  │ Password│        │   └─────────┘       │   │ Temp    │   │
│  └─────────┘        └─────────────────┐   │   │ Humidity│   │
│                                       │   │   │ Methane │   │
│  RefreshTokens                        │   │   │ CO2     │   │
│  ┌──────────┐                         │   │   │ Threshold   │
│  │ Token    │                         │   │   └─────────┘   │
│  │ UserId   │◄────────────────────────┴───┴───────┐        │
│  │ ExpiresAt│                                     │        │
│  │ Consumed │                         Many-to-Many         │
│  └──────────┘                         Relationship         │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

---

## Component Structure

```
App.jsx
├── /login
│   ├── Login.jsx
│   │   ├── UsernameInput (animated)
│   │   ├── PassInput (animated)
│   │   └── Form validation
│   │       └── Fetch: POST /authentication/login
│   │
├── /signup
│   ├── SignUp.jsx
│   │   ├── UsernameInput (animated)
│   │   ├── PassInput (animated)
│   │   └── Form validation
│   │       ├── Fetch: POST /authentication/register
│   │       └── Fetch: POST /authentication/login (auto-login)
│   │
└── /
    └── Home.jsx
        ├── Auth check (JWT token)
        ├── NavBar.jsx
        │   ├── Brand logo
        │   ├── Add Composter Input
        │   │   └── Fetch: POST /composter/AddDevice
        │   ├── Remove Composter Mode Toggle
        │   └── Logout Button
        │       └── Clear localStorage + redirect
        │
        └── Container List
            ├── Fetch: GET /composter/GetAllData
            │
            └── Container.jsx (for each device)
                ├── Header
                │   ├── Device MAC ID
                │   └── Remove Button (✕)
                │       └── Fetch: DELETE /composter/RemoveDevice
                │
                ├── Content
                │   ├── Temp Threshold (3 radio buttons)
                │   ├── Metrics Grid (4 columns)
                │   │   ├── Methane %
                │   │   ├── CO2 %
                │   │   ├── Humidity %
                │   │   └── Temperature °C
                │   │
                │   └── Progress Bar
                │       └── Red → Green gradient
                │
                └── Selection State
                    └── Border color change (gray ↔ blue)
```

---

## Authentication Flow

```
User Opens App
      │
      ▼
Check localStorage.authToken
      │
  ┌───┴────┐
  │         │
  NO      YES
  │         │
  ▼         ▼
LOGIN     HOME
  │         │
  ├─────────┤
  │ SIGNUP  │
  │   OR    │ Skip if already
  │ LOGIN   │ logged in
  │         │
  ▼         │
FETCH       │
 │register  │
 │ or login │
 │          │
 ├─POST with name + password
 │
 ▼
SERVER:
 - Hash password (SHA256)
 - Verify credentials
 - Create JWT token (15 min expiry)
 - Create refresh token (7 day expiry)
 - Return tokens
 │
 ▼
CLIENT:
 - Store in localStorage
 - Set state
 - Redirect to HOME
 │
 ▼
HOME PAGE:
 - Read token from localStorage
 - Add to Authorization header
 - Fetch devices
 - Display containers
 │
 ├─ SELECT: Click container (blue border)
 ├─ VIEW:   See device data (MAC, temp, etc)
 ├─ REMOVE: Enable remove mode, click ✕, confirm
 │   └─ DELETE request with MAC address
 │
 └─ LOGOUT: Click logout
     - Clear localStorage
     - Redirect to /login
```

---

## State Management

```
Global State (localStorage):
├── authToken (JWT access token)
├── refreshToken (JWT refresh token for token rotation)

Component State:

Login.jsx:
├── username: string
├── password: string
├── loading: boolean
├── error: string

SignUp.jsx:
├── username: string
├── password: string
├── loading: boolean
├── error: string

Home.jsx:
├── devices: Device[]
├── selectedDevice: string (MAC) | null
├── removeMode: boolean
├── loading: boolean
├── error: string

NavBar.jsx:
├── addState: 'idle' | 'input' | 'success'
├── inputValue: string (MAC address)
├── removeActive: boolean
├── showPopup: boolean
├── pendingTarget: { target, onConfirmed } | null

Container.jsx:
├── tempThreshold: 0 | 1 | 2
```

---

## API Response Examples

### Login Success
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64encodedlongrandomstring=="
}
```

### GetAllData Success
```json
[
  {
    "MAC": "00:1A:2B:3C:4D:5E",
    "Temp": 45.5,
    "Humidity": 65.2,
    "Methane": 120,
    "CO2": 85,
    "progress": 65.5
  },
  {
    "MAC": "AA:BB:CC:DD:EE:FF",
    "Temp": 38.3,
    "Humidity": 72.5,
    "Methane": 95,
    "CO2": 110,
    "progress": 72.0
  }
]
```

### Error Response
```json
{
  "message": "Invalid credentials."
}
```

---

## Color Scheme

```
Brand Colors:
├── Primary Green:     #86C764 (Light green - buttons)
├── Dark Background:   #0A0C10 (NavBar)
├── Light Background:  #F5F7FA (Containers)
├── Secondary:         #C3CFE2 (Gradient)

Interactive:
├── Selected Border:   #3B82F6 (Blue - container selected)
├── Hover Shadow:      RGBA(0,0,0,0.15)
├── Error:             #DC2626 (Red - remove mode)
├── Success:           #64DC82 (Green - success message)

Progress Bar Gradient:
├── 0%:   #FEE2E2 (Red)
├── 50%:  #FEF3C7 (Yellow)
└── 100%: #DCFCE7 (Green)
```

---

## Deployment Requirements

1. **Backend**: .NET 10, C# 14
2. **Frontend**: React 18+, Node.js 16+
3. **Database**: MariaDB 10.11+
4. **Server Port**: 7123 (or configured)
5. **Frontend Port**: 5173 (Vite dev server)

---

## Security Notes

✅ **Implemented:**
- Password hashing (SHA256)
- JWT token authentication
- Bearer token in Authorization header
- CORS validation on backend
- 401 error handling with redirect

⚠️ **For Production:**
- Use HTTPS only
- Store tokens in httpOnly cookies (not localStorage)
- Implement CSRF protection
- Use stronger password hashing (bcrypt)
- Rate limiting on auth endpoints
- CORS whitelist specific origins
