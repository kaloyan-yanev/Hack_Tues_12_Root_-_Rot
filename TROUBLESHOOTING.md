# RootAndRot - Quick Troubleshooting Guide

## Common Issues & Solutions

### Issue 1: "Not authenticated" Error on Home Page

**Symptoms:**
- Error message appears instead of devices
- No devices load
- Redirected to login page immediately

**Solutions:**
```
1. Check localStorage for tokens:
   - Open DevTools (F12)
   - Go to Application → LocalStorage
   - Look for "authToken" and "refreshToken"

2. If tokens don't exist:
   - Login again at /login
   - Make sure credentials are correct

3. If tokens exist:
   - Check if they're expired (15 min for access)
   - Logout and login again
   - Check Authorization header in Network tab

4. Check backend is running:
   - Visit https://localhost:7123 in browser
   - Should see ASP.NET Core welcome page
```

---

### Issue 2: Devices Not Loading After Login

**Symptoms:**
- Login successful (no error)
- Redirected to home
- Empty device list or loading spinner forever

**Solutions:**
```
1. Check network requests (DevTools → Network):
   - Look for GET /api/composter/GetAllData
   - Should show 200 status
   - Response should have device array

2. If 401 Unauthorized:
   - Token might be expired
   - Logout and login again

3. If CORS error:
   - Backend CORS needs configuration
   - Check Program.cs has CORS middleware

4. If 500 Server Error:
   - Check backend console for exceptions
   - Restart backend service
   - Verify database connection

5. Verify test data exists:
   - Check if devices are in database:
     SELECT * FROM Devices;
     SELECT * FROM UsersDevices WHERE UserID = (SELECT UserID FROM Users WHERE Name = 'admin');

6. Clear browser cache:
   - Hard refresh: Ctrl+Shift+R
   - Clear cookies/cache in DevTools
```

---

### Issue 3: Login/Signup Not Working

**Symptoms:**
- Form submission doesn't work
- No error message
- Page doesn't navigate after submit

**Solutions:**
```
1. Check network request:
   - Open DevTools → Network
   - Submit form
   - Look for POST request to /api/authentication/login or /register

2. If request not shown:
   - Check if fetch URL is correct
   - Verify endpoint path matches backend route

3. If 404 error:
   - Backend endpoint might not exist
   - Check route: [Route("api/[controller]")]
   - Should be: /api/authentication/login

4. If 400 error:
   - Request body format wrong
   - Should be: { "name": "username", "password": "password" }
   - Not: { "email": "...", "password": "..." }

5. If 500 error:
   - Backend error
   - Check backend console logs
   - Restart backend

6. Verify credentials:
   - Username not found: "Invalid credentials" error
   - Create user first via signup
   - For admin/admin12345: Verify in database
     SELECT * FROM Users WHERE Name = 'admin';
```

---

### Issue 4: Password Hash Mismatch

**Symptoms:**
- Can't login with admin/admin12345
- "Invalid credentials" error

**Solutions:**
```
1. Verify password hash in database:
   SELECT Name, Password FROM Users WHERE Name = 'admin';

2. Current test data uses SHA256 Base64:
   - "admin12345" hashes to: gVQnCHxDcGJHZmJKZHJHV3pUZVhyc05pU3dWWVJGMEE=

3. If hash doesn't match:
   - Delete old admin user:
     DELETE FROM Users WHERE Name = 'admin';
   - Run INSERT_TEST_DATA.sql again

4. Important: Hash is case-sensitive
   - Verify exact Base64 string matches

5. Manual verification:
   - Use online SHA256 tool to hash "admin12345"
   - Convert to Base64
   - Compare with database value
```

---

### Issue 5: CORS Error

**Symptoms:**
```
Access to XMLHttpRequest at 'https://localhost:7123/api/...'
from origin 'http://localhost:5173' has been blocked by CORS policy
```

**Solutions:**
```
1. Check Program.cs has CORS setup:
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowAll", policy =>
       {
           policy.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
       });
   });
   app.UseCors("AllowAll");

2. Verify CORS is called before auth:
   app.UseCors("AllowAll");
   app.UseAuthentication();

3. Restart backend after CORS changes

4. Check frontend URL is correct:
   - Fetch URL should match CORS allowed origin
   - Common: https://localhost:7123 (backend)
   - Frontend: http://localhost:5173
```

---

### Issue 6: "Device Not Found" When Adding Device

**Symptoms:**
- Add device button clicked
- Submit device MAC
- Error: "Failed to add device" or server 500 error

**Solutions:**
```
1. Verify device exists in database:
   SELECT * FROM Devices WHERE MACAddress = '00:1A:2B:3C:4D:5E';

2. MAC address format:
   - Must be exact match in database
   - Case-sensitive (if stored differently)
   - Test MAC: 00:1A:2B:3C:4D:5E

3. Check UsersDevices doesn't already have link:
   SELECT * FROM UsersDevices 
   WHERE UserID = (SELECT UserID FROM Users WHERE Name = 'admin')
   AND DeviceID = (SELECT DeviceID FROM Devices WHERE MACAddress = '00:1A:2B:3C:4D:5E');

4. Insert a test device if missing:
   INSERT INTO Devices (DeviceID, MACAddress, Temperature, Humidity, Methane, CO2)
   VALUES (UUID(), 'FF:EE:DD:CC:BB:AA', 40.0, 70.0, 100, 90);

5. Check backend error logs for exact failure reason
```

---

### Issue 7: Remove Device Not Working

**Symptoms:**
- Select container (gets blue border)
- Click remove button
- No confirmation popup or error message

**Solutions:**
```
1. Check remove mode is ON:
   - "Remove composter" button should be red
   - Should say "Remove mode ON"

2. Verify container is selected:
   - Should have blue border (#3B82F6)
   - Remove button (✕) should be visible

3. Check authorization:
   - Device must be linked to current user
   - Verify in database:
     SELECT * FROM UsersDevices
     WHERE UserID = (SELECT UserID FROM Users WHERE Name = 'admin')
     AND DeviceID = (SELECT DeviceID FROM Devices WHERE MACAddress = 'XX:XX:...');

4. Check network request:
   - DevTools → Network
   - Should see DELETE /api/composter/RemoveDevice
   - Check status code (should be 200)

5. If 404:
   - RemoveDevice endpoint not found
   - Restart backend (interface change requires restart)

6. If 500:
   - Backend error
   - Check backend console logs
   - Verify database constraints
```

---

### Issue 8: Logout Not Working

**Symptoms:**
- Click logout button
- Nothing happens
- Still on home page

**Solutions:**
```
1. Check if logout button exists:
   - Should be in top-right corner
   - Replaces the old "Login" link

2. Verify navbar render:
   - Hard refresh: Ctrl+Shift+R
   - Clear cache

3. Check browser console for errors:
   - DevTools → Console
   - Look for JavaScript errors

4. Verify localStorage is cleared:
   - After logout, should be empty
   - Check DevTools → Application → LocalStorage
   - authToken should be gone
   - refreshToken should be gone

5. Manual logout:
   - Open DevTools
   - Clear localStorage
   - Refresh page
   - Should redirect to /login

6. If not redirecting:
   - Check Home.jsx useEffect logic
   - Make sure it checks for token on mount
```

---

### Issue 9: Hot Reload Warnings

**Symptoms:**
```
ENC0023: Adding an abstract method or overriding an inherited method 
requires restarting the application.
```

**Solutions:**
```
This is a known Visual Studio limitation with hot-reload, NOT a code error.

1. Simply restart the backend:
   - Stop debugging (Shift+F5)
   - Start debugging (F5)
   - Changes will be applied

2. This warning appears for interface changes like:
   - Adding methods to IComposterService
   - Changing method signatures
   - It's normal and expected

3. Code is still correct - just needs restart
```

---

### Issue 10: Database Connection Failed

**Symptoms:**
```
Unable to connect to MariaDB server
Connection refused on localhost:3306
```

**Solutions:**
```
1. Verify MariaDB is running:
   - Windows: Check Services (services.msc)
   - Look for "MySQL80" or "MariaDB"
   - Should be running

2. Check connection string in appsettings.json:
   "server=localhost;database=hacktues;uid=root;pwd=admin"

3. Verify credentials:
   - Try connecting with MySQL client:
     mysql -h localhost -u root -p
     (password: admin)

4. Check firewall:
   - MariaDB on port 3306 might be blocked
   - Allow port 3306 in Windows Firewall

5. Restart MariaDB service:
   - Windows Services → Right-click MariaDB → Restart

6. Verify database exists:
   SHOW DATABASES;
   (look for "hacktues")

7. If database missing, create it:
   CREATE DATABASE hacktues;
   USE hacktues;
   (then run schema/migration scripts)
```

---

## Quick Restart Procedure

If everything breaks, do this:

```
1. Clear browser localStorage:
   - DevTools → Application → LocalStorage → Clear All

2. Hard refresh browser:
   - Ctrl+Shift+R (or Cmd+Shift+R on Mac)

3. Stop backend:
   - Visual Studio: Shift+F5 (Stop Debugging)

4. Restart backend:
   - Visual Studio: F5 (Start Debugging)

5. Reload frontend:
   - Browser: F5

6. Clear any pending operations:
   - npm run dev (restart if needed)

7. Login again:
   - Username: admin
   - Password: admin12345
```

---

## Useful Debug Commands

### Browser Console Commands
```javascript
// Check if token exists
localStorage.getItem('authToken')

// Clear all storage
localStorage.clear()

// See all stored items
JSON.stringify(localStorage)

// Check if logged in
Boolean(localStorage.getItem('authToken'))
```

### Database Check Commands
```sql
-- Check users exist
SELECT * FROM Users;

-- Check devices exist
SELECT * FROM Devices;

-- Check user-device links
SELECT * FROM UsersDevices;

-- Check complete setup
SELECT u.Name, d.MACAddress, d.Temperature, d.Humidity
FROM Users u
JOIN UsersDevices ud ON u.UserID = ud.UserID
JOIN Devices d ON ud.DeviceID = d.DeviceID;
```

### Backend Log Locations
```
Visual Studio Debug Console:
- Shows real-time backend logs
- Look for exceptions and errors
- Check authentication claims

SQL Error Logs:
- MariaDB logs usually in:
  C:\Program Files\MariaDB X.X\data\<hostname>.err
```

---

## When to Restart What

| Issue | Solution |
|-------|----------|
| Frontend UI not updating | Hard refresh (Ctrl+Shift+R) |
| Styles not applying | Clear CSS cache + hard refresh |
| Hot-reload warnings | Restart backend (Shift+F5, then F5) |
| Database not updating | Restart MariaDB service |
| API endpoint not found | Restart backend (interface changes) |
| Token issues | Clear localStorage + re-login |
| Everything broken | Follow "Quick Restart Procedure" |

---

## Support Checklist

Before asking for help, verify:

- [ ] MariaDB is running
- [ ] Backend is running (F5 in Visual Studio)
- [ ] Frontend is running (npm run dev)
- [ ] Test data is inserted (run INSERT_TEST_DATA.sql)
- [ ] Browser localStorage is cleared
- [ ] Page is hard-refreshed (Ctrl+Shift+R)
- [ ] No errors in browser console (DevTools)
- [ ] No errors in Visual Studio debug console
- [ ] Correct URL: http://localhost:5173
- [ ] Correct credentials: admin / admin12345
