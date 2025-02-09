import { useEffect, useState } from "react";
import Styles from "./Profile.module.css";

const API_URL = "https://localhost:7290";

const Profile = () => {
  const [userData, setUserData] = useState({
    username: "",
    password: "",
    email: "",
    firstName: "",
    lastName: "",
    address: "",
  });
  const [message, setMessage] = useState("");

  useEffect(() => {
    const fetchProfileData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) return;

      try {
        const response = await fetch(`${API_URL}/api/account/getdata`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ accessToken: token }),
        });

        const data = await response.json();
        if (!response.ok) throw new Error(data.message);

        setUserData(data);
      } catch (error) {
        console.error("Failed to fetch profile data:", error);
      }
    };

    fetchProfileData();
  }, []);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setUserData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    const token = localStorage.getItem("accessToken");
    if (!token) return;

    try {
      const response = await fetch(`${API_URL}/api/account/updatedata`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ ...userData, accessToken: token }),
      });

      const data = await response.json();
      if (!response.ok) throw new Error(data.message);

      setMessage("Profile updated successfully!");
    } catch (error) {
      console.error("Failed to update profile:", error);
      setMessage("Failed to update profile.");
    }
  };

  return (
    <div className={Styles.profileContainer}>
      <h2>Profile</h2>
      {message && <p className={Styles.message}>{message}</p>}
      <form onSubmit={handleSubmit} className={Styles.profileForm}>
        <label>
          Email:
          <input type="email" name="email" value={userData.email} onChange={handleChange} />
        </label>
        <label>
          First Name:
          <input type="text" name="firstName" value={userData.firstName} onChange={handleChange} />
        </label>
        <label>
          Last Name:
          <input type="text" name="lastName" value={userData.lastName} onChange={handleChange} />
        </label>
        <label>
          Address:
          <input type="text" name="address" value={userData.address} onChange={handleChange} />
        </label>
        <label>
          Password:
          <input type="password" name="password" value={userData.password} onChange={handleChange} />
        </label>
        <button type="submit">Update Profile</button>
      </form>
    </div>
  );
};

export default Profile;
