import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "../Navbar/Navbar.module.css";
import logo from "/CozyNest.png?url";
import userW from "/userW.svg?url";
import userD from "/userD.svg?url";
import logoutD from "/LogoutD.svg?url";
import logoutW from "/LogoutW.svg?url";


interface NavbarProps {
  darkmode: boolean;
  setDarkMode: (value: boolean) => void;
}

const Navbar: React.FC<NavbarProps> = ({ darkmode, setDarkMode }) => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [username, setUsername] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const navigate = useNavigate();

  // Toggle the menu
  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  // Toggle dark mode
  const toggleDarkMode = () => {
    const newDarkMode = !darkmode;
    setDarkMode(newDarkMode);
    localStorage.setItem("darkmode", JSON.stringify(newDarkMode));
  };

  // Load dark mode preference from localStorage
  useEffect(() => {
    const savedDarkMode = localStorage.getItem("darkmode");
    if (savedDarkMode !== null) {
      setDarkMode(JSON.parse(savedDarkMode));
    }
  }, []);

  const API_URL = "https://localhost:7290";
  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) {
        return;
      }

      try {
        const response = await fetch(`${API_URL}/api/account/introspect`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}` // <-- Send token in header
          }
        });

        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message);
        }

        if (data.active) {
          setUsername(data.userData.username);
          setRole(data.userData.roleName);
        } else {
          localStorage.removeItem("accessToken");
          navigate("/auth");
        }
      } catch (error) {
        console.error("Failed to fetch user data:", error);
        localStorage.removeItem("accessToken");
        navigate("/auth");
      }
    };

    fetchUserData();
  }, [navigate]);

  const handleLogout = async () => {
    const token = localStorage.getItem("refreshToken");
  
    if (!token) {
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      navigate("/auth");
      return;
    }
  
    try {
      const response = await fetch(`${API_URL}/api/account/logout`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`, // Sending refresh token in header
        },
      });
  
      if (!response.ok) {
        console.error("Logout failed:", await response.text());
      }
    } catch (error) {
      console.error("Error during logout:", error);
    } finally {
      console.log("logout done")
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      navigate("/auth");
    }
  };
  

  return (
    <nav
      className={`${styles.navbar} ${darkmode ? styles.dark : styles.light}`}
    >
      <div className={styles.logoContainer}>
        <img src={logo} alt="Logo" className={styles.navLogo} />
      </div>

      <button onClick={toggleMenu} className={styles.menuToggle}>
        &#9776;
      </button>

      <div className={`${styles.menu} ${menuOpen ? styles.menuOpen : ""}`}>
        <a href="/#home">Main</a>
        <a href="/#story">Story</a>
        <a href="/#info">Info</a>
        <a href="/#contact">Contact</a>
        <a href="/rooms">Rooms</a>
        {role ? <a href="/reservations">Reservations</a> : ""}
        {username ? (
          <div className={styles.authSpace}>
            <a href={`/profile/`}><img className={styles.pictogram} src={darkmode ? userW : userD} />{username.toUpperCase()}</a>
            <a href={`/auth`} onClick={handleLogout}><img className={styles.pictogram} src={darkmode ? logoutW : logoutD} />Logout</a>
          </div>
        ) : (
          <div className={styles.authSpace}>
            <a href="/Auth"><img className={styles.pictogram} src={darkmode ? userW : userD} alt="" />Login/Register</a>
          </div>
        )}
      </div>

      <button onClick={toggleDarkMode} className={styles.darkModeToggle}>
        {darkmode ? "☀️" : "🌙"}
      </button>
    </nav>
  );
};

export default Navbar;
