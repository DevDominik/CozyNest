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
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
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
      accessToken: token,
    };

    // Only add password if it's valid and matches the confirmation
    if (password && password === confirmPassword) {
      updatedData.password = password;
    } else if (password || confirmPassword) {
      setMessage("Passwords do not match.");
      return;
    }

    console.log("Sending updated data:", updatedData); // Log data being sent

    try {
      const response = await fetch(`${BASEURL}/api/account/updatedata`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedData),
      });

      const data = await response.json(); // Wait for the response to be parsed
      console.log("Response data:", data); // Log the entire response data

      if (!response.ok) {
        console.error("Failed to update profile:", data); // Log detailed response if the request fails
        setMessage(data.message || "Failed to update profile.");
        return;
      }

      setMessage("Profile updated successfully.");
      setUserData(data.userData); // Assuming the updated user data is returned in the response

      // If the password was updated, update tokens as well
      if (password && password === confirmPassword) {
        if (
          data.newTokens &&
          data.newTokens.accessToken &&
          data.newTokens.refreshToken
        ) {
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
    <div className={Styles.profileContainer}>
    
      <h2 className={Styles.profileTitle}>Profile</h2>
      <form onSubmit={handleSubmit} className={Styles.profileForm}>
        <label className={Styles.label}>
          Email
          <input
            type="email"
            name="email"
            value={userData?.email}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          First Name
          <input
            type="text"
            name="firstName"
            value={userData?.firstName}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          Last Name
          <input
            type="text"
            name="lastName"
            value={userData?.lastName}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          Address
          <input
            type="text"
            name="address"
            value={userData?.address || ""}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          New Password
          <input
            type="password"
            name="password"
            value={password}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <label className={Styles.label}>
          Confirm Password
          <input
            type="password"
            name="confirmPassword"
            value={confirmPassword}
            onChange={handleChange}
            className={Styles.input}
          />
        </label>
        <button type="submit" className={Styles.submitButton}>
          Update Profile
        </button>
        {message && <p className={Styles.message}>{message}</p>}
      </form>
    </div>
  );
};

export default Profile;
