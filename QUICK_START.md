# RootAndRot Quick Reference Card

## 🚀 Quick Start (5 minutes)

### 1. Run SQL (1 min)
```sql
SOURCE INSERT_TEST_DATA.sql;
```

### 2. Restart Backend (2 min)
```
Visual Studio: Shift+F5, then F5
```

### 3. Start Frontend (1 min)
```bash
npm run dev
```

### 4. Login (1 min)
```
URL: http://localhost:5173
Username: admin
Password: admin12345
```

---

## 🔑 Login Credentials

| Username | Password | Devices |
|----------|----------|---------|
| `admin` | `admin12345` | 3 |
| `testuser` | `user12345` | 1 |

---

## 📱 Device Test MACs

```
00:1A:2B:3C:4D:5E  (45.5°C, 65.2% humidity)
AA:BB:CC:DD:EE:FF  (38.3°C, 72.5% humidity)
11:22:33:44:55:66  (42.1°C, 58.9% humidity)
```

---

## 🌐 URLs

| Service | URL | Port |
|---------|-----|------|
| Frontend | http://localhost:5173 | 5173 |
| Backend | https://localhost:7123 | 7123 |
| Database | localhost | 3306 |

---

## 🔄 API Endpoints

### Auth (No Token Required)
```
POST /api/authentication/register
POST /api/authentication/login
```

### Devices (Token Required)
```
GET    /api/composter/GetAllData
POST   /api/composter/AddDevice
DELETE /api/composter/RemoveDevice
POST   /api/composter/ChangeTempThreshold
```

---

## 🛠️ Troubleshooting One-Liners

| Problem | Fix |
|---------|-----|
| Stuck on login | Clear localStorage, hard refresh (Ctrl+Shift+R) |
| Can't login | Verify SQL data inserted, check admin user exists |
| No devices | Check UsersDevices table links |
| API 401 error | Token expired, logout and login again |
| CORS error | Restart backend |
| Hot-reload warning | Restart backend (normal) |

---

## 📊 Tech Stack

```
Frontend:  React 18+, Vite, CSS Modules
Backend:   ASP.NET Core 10, C# 14
Database:  MariaDB 10.11+
Auth:      JWT (Bearer tokens)
API:       REST with Fetch
```

---

## 📝 Key Features

✅ Username/password authentication  
✅ JWT token-based security  
✅ Device list display  
✅ Real-time sensor data  
✅ Device management (add/remove)  
✅ Temperature threshold control  
✅ Auto-logout on token expire  

---

## 🎯 File Structure

```
Frontend: rootandrot.client/src/
├── pages/
│   ├── Login.jsx         ← Auth
│   ├── SignUp.jsx        ← Registration
│   └── Home.jsx          ← Devices
├── components/
│   ├── NavBar.jsx        ← Top bar
│   ├── Container.jsx     ← Device card
│   ├── EmailInput.jsx    ← Username input
│   └── PassInput.jsx     ← Password input

Backend: RootAndRot.Server/
├── Controllers/
│   ├── AuthenticationController.cs
│   └── ComposterController.cs
├── Services/
│   └── ComposterService.cs
└── Data/
    ├── AppDbContext.cs
    ├── User.cs
    └── Device.cs
```

---

## 🔐 Database Tables

```
Users (UserID, Name, Password)
  └─ UsersDevices (UserID, DeviceID)
      └─ Devices (DeviceID, MAC, Temp, Humidity, etc)
RefreshTokens (for token rotation)
```

---

## ⚡ Common Commands

### Database
```sql
-- View all users
SELECT * FROM Users;

-- View all devices
SELECT * FROM Devices;

-- View user's devices
SELECT u.Name, d.MACAddress, d.Temperature
FROM Users u
JOIN UsersDevices ud ON u.UserID = ud.UserID
JOIN Devices d ON ud.DeviceID = d.DeviceID;

-- Reset data
DELETE FROM RefreshTokens;
DELETE FROM UsersDevices;
DELETE FROM Users;
DELETE FROM Devices;
```

### Terminal
```bash
# Frontend
npm run dev
npm run build
npm run preview

# Backend
dotnet run
dotnet build
```

### Browser Console
```javascript
// Check auth
localStorage.getItem('authToken')

// Clear storage
localStorage.clear()

// Test fetch
fetch('/api/composter/GetAllData', {
  headers: { 'Authorization': 'Bearer ' + localStorage.getItem('authToken') }
}).then(r => r.json()).then(console.log)
```

---

## ✅ Pre-Deployment Checklist

- [ ] SQL data inserted
- [ ] Backend restarted
- [ ] Frontend running
- [ ] Can login with admin/admin12345
- [ ] Devices display
- [ ] Can select/remove device
- [ ] Can logout
- [ ] No console errors

---

## 🎓 Required .NET Version

- .NET 10
- C# 14

---

## 🔍 Monitor Requests

Open DevTools (F12) → Network tab and watch for:

```
POST /api/authentication/login       → {accessToken, refreshToken}
GET  /api/composter/GetAllData       → [{MAC, Temp, ...}]
POST /api/composter/AddDevice        → 200 OK
DELETE /api/composter/RemoveDevice   → 200 OK
```

---

## 📚 Documentation

| File | Purpose |
|------|---------|
| README.md | Main overview |
| IMPLEMENTATION_SUMMARY.md | What was built |
| TESTING_GUIDE.md | How to test |
| VISUAL_OVERVIEW.md | Architecture & flow |
| TROUBLESHOOTING.md | Debug guide |
| FINAL_CHECKLIST.md | Verification checklist |

---

## 🎯 Goal: Working App in 5 Minutes

1. **Insert SQL** (1 min)
   ```
   Run: INSERT_TEST_DATA.sql
   ```

2. **Restart Backend** (2 min)
   ```
   VS: Shift+F5, then F5
   ```

3. **Check Frontend** (1 min)
   ```
   npm run dev
   ```

4. **Login & Verify** (1 min)
   ```
   admin / admin12345
   ```

**Result: 🎉 Working device management app**

---

## 🚨 If Something Breaks

```bash
# Step 1: Clear cache
DevTools → Application → LocalStorage → Clear All

# Step 2: Hard refresh
Ctrl+Shift+R

# Step 3: Restart backend
VS: Shift+F5, then F5

# Step 4: Clear database (if needed)
DELETE FROM UsersDevices;
DELETE FROM Users;
DELETE FROM Devices;
SOURCE INSERT_TEST_DATA.sql;

# Step 5: Try again
http://localhost:5173
```

---

## 📞 Help

1. Check TROUBLESHOOTING.md for common issues
2. Check browser console (F12) for errors
3. Check VS debug console for backend errors
4. Verify all 4 services running:
   - ✅ Frontend (npm run dev)
   - ✅ Backend (F5 in VS)
   - ✅ MariaDB (Services → MariaDB running)
   - ✅ Database has test data

---

## 🎉 You're All Set!

Everything is implemented and documented.

**Happy coding! 🚀**
