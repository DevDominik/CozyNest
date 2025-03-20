import { useEffect, useState } from "react";
import Styles from "./Profile.module.css";

const BASEURL = "http://localhost:5232";

const Profile = () => {
  const [userData, setUserData] = useState({
    username: "",
    password: "",
    email: "",
    firstName: "",
    lastName: "",
    address: "",
  });
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);
  const [pw, setPw] = useState(false);
  const [counter, setCounter] = useState(1);

  useEffect(() => {
    const fetchProfileData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) return;

      try {
        const response = await fetch(`${BASEURL}/api/account/introspect`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });

        const data = await response.json();
        if (!response.ok || !data.active) throw new Error("Invalid session");

        setUserData(data.userData);
      } catch (error) {
        console.error("Failed to fetch profile data:", error);
      } finally {
        setLoading(false);
      }
    };


    fetchProfileData();
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;
    if (name === "password") {
      setPassword(value);
    } else if (name === "confirmPassword") {
      setConfirmPassword(value);
    } else {
      setUserData((prevData) => ({ ...prevData, [name]: value }));
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
  
    const token = localStorage.getItem("accessToken");
    if (!token) {
      setMessage("Access token not found. Please log in.");
      return;
    }
  
    const updatedData = {
      username: userData.username,
      email: userData.email,
      firstName: userData.firstName,
      lastName: userData.lastName,
      address: userData.address,
    };
  
    if (password && password === confirmPassword) {
      updatedData.password = password;
    } else if (password || confirmPassword) {
      setMessage("Passwords do not match.");
      return;
    }
  
    try {
      const response = await fetch(`${BASEURL}/api/account/updatedata`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify(updatedData),
      });
  
      const data = await response.json();
  
      if (!response.ok) {
        console.error("Failed to update profile:", data);
        setMessage(data.message || "Failed to update profile.");
        return;
      }
  
      setMessage(`Profile updated successfully. (${counter})`);
  
      const fetchProfileData = async () => {
        const token = localStorage.getItem("accessToken");
        if (!token) return;
  
        try {
          const response = await fetch(`${BASEURL}/api/account/introspect`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`,
            }
          });
  
          const data = await response.json();
          if (!response.ok || !data.active) throw new Error("Invalid session");
  
          setUserData(data.userData);
          setCounter(counter+1);
        } catch (error) {
          console.error("Failed to fetch profile data:", error);
        } finally {
          setLoading(false);
        }
      };
  
      fetchProfileData();  
  
      if (password && password === confirmPassword) {
        if (data.newTokens?.accessToken && data.newTokens?.refreshToken) {
          localStorage.setItem("accessToken", data.newTokens.accessToken);
          localStorage.setItem("refreshToken", data.newTokens.refreshToken);
          console.log("New tokens stored in localStorage");
        } else {
          console.error("No new tokens found in the response.");
        }
      }
    } catch (error) {
      console.error("Error updating profile:", error);
      setMessage("Error updating profile.");
    }
  };
  


  if (loading) {
    return <p>Loading profile...</p>;
  }

  return (
    <div className={Styles.Container}>
      <div className={Styles.profileContainer}>
        <h1>Adatok beállítása</h1>
        <p>Kérjük, először állítsa be adatait, hogy szobát tudjon foglalni. Ha jelszót szeretne változtatni, írja be kétszer az új jelszót. Ha csak a többi adatát kívánja frissíteni, hagyja üresen a jelszó mezőket.</p>

      </div>
      <div className={Styles.profileContainer}>

        <h2 className={Styles.profileTitle}>Profile</h2>
        <form onSubmit={handleSubmit} className={Styles.profileForm}>
          <label className={Styles.label}>
            E-mail
            <input
              type="email"
              name="email"
              value={userData?.email}
              onChange={handleChange}
              className={Styles.input}
            />
          </label>
          <label className={Styles.label}>
            Családnév
            <input
              type="text"
              name="firstName"
              value={userData?.firstName}
              onChange={handleChange}
              className={Styles.input}
            />
          </label>
          <label className={Styles.label}>
          Vezetéknév
            <input
              type="text"
              name="lastName"
              value={userData?.lastName}
              onChange={handleChange}
              className={Styles.input}
            />
          </label>
          <label className={Styles.label}>
            Lakcím
            <input
              type="text"
              name="address"
              value={userData?.address || ""}
              onChange={handleChange}
              className={Styles.input}
            />
          </label>
          {!pw ? (<div className={Styles.setpwbutton} onClick={()=> setPw(!pw)}>Új jelszó megadása</div>) : ("")}
          
         {pw ? ( <><label className={Styles.label}>
            Új Jelszó
            <input
              type="password"
              name="password"
              value={password}
              onChange={handleChange}
              className={Styles.input} />
          </label><label className={Styles.label}>
              Jelszó mégegyszer
              <input
                type="password"
                name="confirmPassword"
                value={confirmPassword}
                onChange={handleChange}
                className={Styles.input} />
            </label>
            {pw ? (<div className={Styles.setpwbutton} onClick={()=>{setPw(!pw); setConfirmPassword(""); setPassword("")}}>Mégsem</div>): ("")}
            </>) : ("")}
          <button type="submit" className={Styles.submitButton}>
            Felhasználó Módosítása
          </button>
          {message && <p className={Styles.message}>{message}</p>}
        </form>
      </div>
    </div>
  );
};

export default Profile;
