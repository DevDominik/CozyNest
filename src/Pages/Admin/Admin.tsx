import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Styles from "./Admin.module.css";
import UserList from "./UserList";

export const Admin = () => {
  const navigate = useNavigate();
  const API_URL = "https://localhost:7290";
  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) {
        navigate("/auth");
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

        if (data.userData.roleName != "Admin") {
            navigate("/");
        }
      } catch (error) {
        console.error("Failed to fetch user data:", error);
        localStorage.removeItem("accessToken");
        navigate("/auth");
      }
    };

    fetchUserData();
  }, [navigate]);

  return <div className={Styles.Page}>
    
  <UserList></UserList>


  </div>;
};
