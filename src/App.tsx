import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import Navbar from "./components/Navbar/Navbar";
import Home from "../src/Pages/Home/Home";
import { Suspense, useEffect, useState } from "react";
import AuthPage from "./Pages/Auth/AuthPage";
import Footer from "./components/Footer/Footer";
import { Admin } from "./Pages/Admin/Admin";
import Profile from "./Pages/Profile/Profile";
import { Rooms } from "./Pages/Rooms/Rooms";
import CreateRoom from "./Pages/CreateRoom/CreateRoom";
import Reservations from "./Pages/Reservations/Reservations";
import ReserveRoom from "./Pages/ReserveRoom/ReserveRoom";
import Documentation from "./Pages/Documentation/Documentation";

function App() {
  const [darkmode, setDarkMode] = useState(false);

  // Combined useEffect for dark mode initialization
  useEffect(() => {
    const savedMode = localStorage.getItem("darkmode") === "true";
    setDarkMode(savedMode);
    document.body.className = savedMode ? "dark" : "light"; // Directly update the class
  }, []);

  // Update dark mode in localStorage on change
  useEffect(() => {
    localStorage.setItem("darkmode", JSON.stringify(darkmode));
    document.body.className = darkmode ? "dark" : "light"; // Apply dark/light mode class
  }, [darkmode]);

  return (
    <BrowserRouter>
      <Navbar darkmode={darkmode} setDarkMode={setDarkMode} />
      {/* Using Suspense here only for fallback loading UI, no lazy loading */}
      <Suspense fallback={<div className="fallback">Loading...</div>}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/auth" element={<AuthPage />} />
          <Route path="/admin" element={<Admin />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/rooms" element={<Rooms />} />
          <Route path="/CreateRoom" element={<CreateRoom />} />
          <Route path="/Reservations" element={<Reservations />} />
          <Route path="/reserve-room" element={<ReserveRoom />} />
          <Route path="/docs" element={<Documentation />} />
        </Routes>
      </Suspense>
      <Footer />
    </BrowserRouter>
  );
}

export default App;
