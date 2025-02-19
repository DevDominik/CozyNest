import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "../Navbar/Navbar.module.css";

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
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ accessToken: token }),
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

  const handleLogout = () =>{
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
  }

  return (
    <nav
      className={`${styles.navbar} ${darkmode ? styles.dark : styles.light}`}
    >
      <div className={styles.logoContainer}>
        <img src="./CozyNest.png" alt="Logo" className={styles.navLogo} />
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
        {role == "Manager" ? <a href="/Admin">Admin Panel</a> : <p></p>}
        {role == "Manager" ? <a href="/CreateRoom">Create Room</a> : <p></p>}
        {username ? (
          <div className={styles.authSpace}>
            <a href={`/profile/`}>{username.toUpperCase()}</a>
            <a href={`/auth`} onClick={handleLogout}>Logout</a>
          </div>
        ) : (
          <div className={styles.authSpace}>
            <a href="/Auth">Login/Register</a>
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
