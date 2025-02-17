import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import Navbar from "./components/Navbar/Navbar";
import Home from "../src/Pages/Home/Home";
import { useEffect, useState } from "react";
import AuthPage from "./Pages/Auth/AuthPage";
import Footer from "./components/Footer/Footer";
import { Admin } from "./Pages/Admin/Admin";
import Profile from "./Pages/Profile/Profile";
import { Rooms } from "./Pages/Rooms/Rooms";

function App() {
  const [darkmode, setDarkMode] = useState(false);

  useEffect(() => {
    if (!localStorage.getItem("darkmode")) {
      localStorage.setItem("darkmode", JSON.stringify(true));
    }
    document.body.className = darkmode ? "dark" : "light";
  }, [darkmode]);

  return (
    <>
      <BrowserRouter>
        <Navbar darkmode={darkmode} setDarkMode={setDarkMode} ></Navbar>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/auth" element={<AuthPage />} />
          <Route path="/admin" element={<Admin />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/rooms" element={<Rooms />} />
        </Routes>
        <Footer></Footer>
      </BrowserRouter>
    </>
  );
}

export default App;
