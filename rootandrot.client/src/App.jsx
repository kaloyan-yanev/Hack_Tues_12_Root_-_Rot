//import { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import './App.css';
import '/src/assets/fonts/Satoshi/css/satoshi.css';

import Login from "./pages/Login";
import SignUp from "./pages/SignUp";
import Home from "./pages/Home";

function App() {
    
    return (
        <BrowserRouter>
        <div className="container">
        <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/signup" element={<SignUp />} />
                </Routes>

        </div>
        </BrowserRouter>
    );
    
}

export default App;