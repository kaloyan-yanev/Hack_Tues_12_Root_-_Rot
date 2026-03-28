import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import NavBar from '../components/NavBar'
import Container from '../components/Container'
import styles from './Home.module.css';

function Home() {
    const navigate = useNavigate();
    const [devices, setDevices] = useState([]);
    const [selectedDevice, setSelectedDevice] = useState(null);
    const [removeMode, setRemoveMode] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Check authentication on mount
    useEffect(() => {
        const token = localStorage.getItem("authToken");
        if (!token) {
            navigate("/login?returnUrl=/", { replace: true });
            return;
        }
        fetchDevices();

        // Set up polling interval to check for changes every 10 seconds
        const intervalId = setInterval(() => {
            fetchDevices();
        }, 10000); // 10 seconds

        // Cleanup interval on unmount
        return () => clearInterval(intervalId);
    }, [navigate]);

    const fetchDevices = async () => {
        try {
            setLoading(true);
            const token = localStorage.getItem("authToken");
            if (!token) {
                navigate("/login?returnUrl=/", { replace: true });
                return;
            }

            const resp = await fetch("https://localhost:61954/api/composter/GetAllData", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                }
            });

            console.log("GetAllData Response Status:", resp.status);
            console.log("GetAllData Response Headers:", resp.headers);

            if (resp.status === 401) {
                localStorage.removeItem("authToken");
                localStorage.removeItem("refreshToken");
                navigate("/login?returnUrl=/", { replace: true });
                return;
            }

            if (!resp.ok) {
                const text = await resp.text();
                console.error("Response Error Text:", text);
                throw new Error(`HTTP ${resp.status}: ${text || "Unknown error"}`);
            }

            const text = await resp.text();
            console.log("Raw Response Body:", text);

            if (!text) {
                throw new Error("Empty response from server");
            }

            const data = JSON.parse(text);
            console.log("Parsed Device Data:", data);
            setDevices(data);
            setError("");

            // Check for high methane levels and trigger StirTor if needed
            for (const device of data) {
                if (device.methane > 500) {
                    console.log(`⚠️ High methane detected (${device.methane} PPM) in device ${device.mac}. Triggering StirTor...`);
                    try {
                        const stirResp = await fetch("https://localhost:61954/api/composter/StirTor", {
                            method: "POST",
                            headers: {
                                "Authorization": `Bearer ${token}`,
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify({ DeviceId: device.deviceId })
                        });

                        if (stirResp.ok) {
                            console.log(`✓ StirTor triggered successfully for device ${device.mac}`);
                        } else {
                            console.error(`Failed to trigger StirTor for device ${device.mac}:`, stirResp.status);
                        }
                    } catch (stirErr) {
                        console.error("Error triggering StirTor:", stirErr);
                    }
                }
            }
        } catch (err) {
            console.error("Full Error Object:", err);
            setError(err.message || "Error loading devices");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleAddComposter = async (macAddress) => {
        try {
            const token = localStorage.getItem("authToken");
            if (!token) {
                setError("Not authenticated");
                return;
            }

            const resp = await fetch("https://localhost:61954/api/composter/AddDevice", {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ MACAddress: macAddress })
            });

            if (resp.status === 401) {
                localStorage.removeItem("authToken");
                localStorage.removeItem("refreshToken");
                navigate("/login?returnUrl=/", { replace: true });
                return;
            }

            if (!resp.ok) {
                const errorData = await resp.json().catch(() => ({}));
                throw new Error(errorData.message || "Failed to add device");
            }

            // Refresh the device list
            await fetchDevices();
        } catch (err) {
            setError(err.message || "Error adding device");
            console.error(err);
        }
    };

    const handleRemoveDevice = async (deviceId) => {
        try {
            const token = localStorage.getItem("authToken");
            if (!token) {
                setError("Not authenticated");
                return;
            }

            const resp = await fetch(`https://localhost:61954/api/composter/RemoveDevice?deviceId=${deviceId}`, {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                }
            });

            if (resp.status === 401) {
                localStorage.removeItem("authToken");
                localStorage.removeItem("refreshToken");
                navigate("/login?returnUrl=/", { replace: true });
                return;
            }

            if (!resp.ok) {
                throw new Error("Failed to remove device");
            }

            setSelectedDevice(null);
            await fetchDevices();
        } catch (err) {
            setError(err.message || "Error removing device");
            console.error(err);
        }
    };

    const handleContainerSelect = (macAddress) => {
        setSelectedDevice(macAddress);
    };

    const handleContainerRemove = (deviceId) => {
        if (removeMode) {
            handleRemoveDevice(deviceId);
        }
    };

    const handleRemoveMode = (isActive) => {
        setRemoveMode(isActive);
        if (!isActive) {
            setSelectedDevice(null);
        }
    };

    const handlePageClick = () => {
        setSelectedDevice(null);
    };

    return (
        <div className={styles.page} onClick={handlePageClick}>
            <NavBar onAddComposter={handleAddComposter} onRemoveMode={handleRemoveMode} />

            <div className={styles.containerList}>
                {error && <div className={styles.error}>{error}</div>}

                {loading ? (
                    <div className={styles.loading}>Loading devices...</div>
                ) : devices.length === 0 ? (
                    <div className={styles.empty}>No devices yet. Add one using the button above!</div>
                ) : (
                    devices.map((device) => (
                        <Container
                            key={device.mac}
                            device={device}
                            isSelected={selectedDevice === device.mac}
                            onSelect={() => handleContainerSelect(device.mac)}
                            onRemove={handleContainerRemove}
                        />
                    ))
                )}
            </div>
        </div>
    );
}

export default Home;