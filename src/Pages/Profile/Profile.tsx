import { useEffect, useState } from "react";
import Styles from "./Profile.module.css";

const BASEURL = "https://localhost:7290";

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
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProfileData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) return;

      try {
        const response = await fetch(`${BASEURL}/api/account/introspect`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ accessToken: token }),
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
    setUserData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const token = localStorage.getItem("accessToken");
    if (!token) return;
  
    try {
      const response = await fetch(`${BASEURL}/api/account/updatedata`, {
        method: "PUT",  // Changed from PATCH to PUT
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ ...userData, accessToken: token }),  // Token included in body
      });
      console.log(response);
  
      const data = await response.json();
      if (!response.ok) throw new Error(data.message);
  
      setMessage("Profile updated successfully!");
    } catch (error) {
      console.error("Failed to update profile:", error);
      setMessage("Failed to update profile.");
    }
  };
  
  

  if (loading) {
    return <p>Loading profile...</p>;
  }

  return (
    <div className={Styles.profileContainer}>
      <h2 className={Styles.profileTitle}>Profile</h2>
      {message && <p className={Styles.message}>{message}</p>}
      <form onSubmit={handleSubmit} className={Styles.profileForm}>
        <label className={Styles.label}>
          Email:
          <input
            type="email"
            name="email"
            value={userData?.email}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          First Name:
          <input
            type="text"
            name="firstName"
            value={userData?.firstName}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          Last Name:
          <input
            type="text"
            name="lastName"
            value={userData?.lastName}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          Address:
          <input
            type="text"
            name="address"
            value={userData?.address || ""}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <button type="submit" className={Styles.submitButton}>
          Update Profile
        </button>
      </form>
    </div>
  );
};

export default Profile;
