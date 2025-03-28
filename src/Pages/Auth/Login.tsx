import { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Auth.module.css";

interface LoginFormState {
  Username: string;
  Password: string;
}

const API_URL = "http://localhost:5232";

const LoginForm = () => {
  const [formData, setFormData] = useState<LoginFormState>({
    Username: "",
    Password: "",
  });
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      const response = await fetch(`${API_URL}/api/account/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      const data = await response.json();
      if (!response.ok) {
        throw new Error("Ismeretlen hiba történt. Kérjük, próbálja újra később");
      }


      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);

      navigate("/profile");
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className={styles.card}>
      <div className={styles.icon}></div>
      <h1 className={styles.title}>BEJELENTKEZÉS</h1>
      <form onSubmit={handleSubmit} className={styles.form}>
        <input
          type="username"
          name="Username"
          placeholder="Falhasználónév"
          value={formData.Username}
          onChange={handleChange}
          required
          className={styles.input}
        />
        <input
          type="password"
          name="Password"
          placeholder="Jelszó"
          value={formData.Password}
          onChange={handleChange}
          required
          className={styles.input}
        />
        {error && <p className={styles.error}>{error}</p>}
        <button type="submit" className={styles.button}>
          BEJELENTKEZÉS
        </button>
      </form>
    </div>
  );
};

export default LoginForm;
