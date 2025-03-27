import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "../Navbar/Navbar.module.css";
import logo from "/CozyNest.png?url";
import userW from "/userW.svg?url";
import userD from "/userD.svg?url";
import logoutD from "/LogoutD.svg?url";
import logoutW from "/LogoutW.svg?url";
import { Book, Moon, Sun } from "lucide-react";

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

  const API_URL = "http://localhost:5232";
  useEffect(() => {
    const fetchUserData = async () => {
      const accessToken = localStorage.getItem("accessToken");
      const refreshToken = localStorage.getItem("refreshToken");

      if (!accessToken || !refreshToken) {
        navigate("/auth");
        return;
      }

      const introspect = async (token: string) => {
        const response = await fetch(`${API_URL}/api/account/introspect`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });
        return response;
      };

      const tryRenewToken = async () => {
        const response = await fetch(`${API_URL}/api/account/renewtoken`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${refreshToken}`,
          },
        });

        if (!response.ok) {
          throw new Error("Token megújítás sikertelen");
        }

        const data = await response.json();
        localStorage.setItem("accessToken", data.accessToken);
        localStorage.setItem("refreshToken", data.refreshToken);
        return data.accessToken;
      };

      try {
        // Először próbáljuk az aktuális accessToken-nel
        let response = await introspect(accessToken);

        if (!response.ok) {
          // Ha nem 200-as, próbáljuk a megújítást
          const newAccessToken = await tryRenewToken();
          response = await introspect(newAccessToken);
        }

        const data = await response.json();

        if (!response.ok || !data.active) {
          throw new Error("Token érvénytelen vagy lejárt");
        }

        setUsername(data.userData.username);
        setRole(data.userData.roleName);
      } catch (error) {
        console.error("Hiba a felhasználói adatok lekérésénél:", error);
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
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
      console.log("logout done");
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
        <a href="/#home">Főoldal</a>
        <a href="/#story">Történet</a>
        <a href="/#info">Informacio</a>
        <a href="/#contact">Kontakt</a>
        <a href="/rooms">Szobák</a>
        {role ? <a href="/reservations">Foglalásaim</a> : ""}
        <a href="/docs" className={styles.docsLink} title="Dokumentáció">
        📚Dokumentáció
        </a>
        {username ? (
          <div className={styles.authSpace}>
            <a href={`/profile/`}>
              <img
                className={styles.pictogram}
                src={darkmode ? userW : userD}
              />
              {username.toUpperCase()}
            </a>
            <a href={`/auth`} onClick={handleLogout}>
              <img
                className={styles.pictogram}
                src={darkmode ? logoutW : logoutD}
              />
              Kijelentkezés
            </a>
          </div>
        ) : (
          <div className={styles.authSpace}>
            <a href="/Auth">
              <img
                className={styles.pictogram}
                src={darkmode ? userW : userD}
                alt=""
              />
              Bejelentkezés/Regisztráció
            </a>
          </div>
        )}
      </div>

      <div className={styles.topRightControls}>
        
        <button onClick={toggleDarkMode} className={styles.darkModeToggle}>
          {darkmode ? "☀️" : "🌙"}
        </button>
      </div>
    </nav>
  );
};

export default Navbar;
