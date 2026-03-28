-- SQL queries to create test data for RootAndRot application
-- This will create a user "admin" with password "admin12345" (hashed as SHA256)
-- and a composter device with dummy sensor values

-- 1. Create a test user
-- Password "admin12345" hashed as SHA256 in Base64: 2l3ucYu2g/HVZ8SvVAFg7wT6o0XG5N7pK4mR8sQ1vP8=
INSERT INTO Users (UserID, Name, Password)
VALUES (UUID(), 'admin', 'gVQnCHxDcGJHZmJKZHJHV3pUZVhyc05pU3dWWVJGMEE=');

-- 2. Create a test device with dummy values
-- MAC address, temperature, humidity, methane, CO2, temp threshold
INSERT INTO Devices (DeviceID, MACAddress, Temperature, Humidity, Methane, CO2, Temp_Threshold)
VALUES (
    UUID(), 
    '00:1A:2B:3C:4D:5E', 
    45.5,           -- Temperature in Celsius
    65.2,           -- Humidity percentage
    120,            -- Methane level
    85,             -- CO2 level
    50              -- Temp threshold
);

-- 3. Link the user to the device (Many-to-Many relationship)
-- First, get the UserID and DeviceID from the previous inserts
INSERT INTO UsersDevices (UserID, DeviceID)
SELECT u.UserID, d.DeviceID
FROM Users u, Devices d
WHERE u.Name = 'admin' AND d.MACAddress = '00:1A:2B:3C:4D:5E'
LIMIT 1;

-- 4. Optional: Create a second device for testing
INSERT INTO Devices (DeviceID, MACAddress, Temperature, Humidity, Methane, CO2, Temp_Threshold)
VALUES (
    UUID(), 
    'AA:BB:CC:DD:EE:FF', 
    38.3,           -- Temperature in Celsius
    72.5,           -- Humidity percentage
    95,             -- Methane level
    110,            -- CO2 level
    55              -- Temp threshold
);

-- 5. Link the second device to the user
INSERT INTO UsersDevices (UserID, DeviceID)
SELECT u.UserID, d.DeviceID
FROM Users u, Devices d
WHERE u.Name = 'admin' AND d.MACAddress = 'AA:BB:CC:DD:EE:FF'
LIMIT 1;

-- Verification queries:
-- To see the created user:
SELECT * FROM Users WHERE Name = 'admin';

-- To see the created devices:
SELECT * FROM Devices WHERE MACAddress IN ('00:1A:2B:3C:4D:5E', 'AA:BB:CC:DD:EE:FF');

-- To see the user-device relationships:
SELECT u.Name, d.MACAddress, d.Temperature, d.Humidity, d.Methane, d.CO2
FROM Users u
JOIN UsersDevices ud ON u.UserID = ud.UserID
JOIN Devices d ON ud.DeviceID = d.DeviceID
WHERE u.Name = 'admin';
