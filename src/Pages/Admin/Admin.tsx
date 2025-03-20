import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Styles from "./Admin.module.css";
import UserList from "./UserList";

export const Admin = () => {
  const navigate = useNavigate();
  const API_URL = "http://localhost:5232";

  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) {
        navigate("/auth");
        return;
      }

      try {
        const response = await fetch(`${API_URL}/api/account/introspect`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });

        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message);
        }

        if (data.userData.roleName !== "Manager") {
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

  return (
    <div className={Styles.Page}>
      <div className={Styles.UserListContainer}>
        <UserList />
      </div>
    </div>
  );
};
