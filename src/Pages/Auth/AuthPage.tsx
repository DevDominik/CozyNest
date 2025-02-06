import Login from "./Login.tsx"
import styles from "./Auth.module.css";
import { useState } from "react";
import Register from "./Register.tsx";


const AuthPage = () => {

  const [isLogin, setIsLogin] = useState<boolean>();

  const handlePanelChange = () =>{
    setIsLogin(!isLogin)
  }

  return (
    <div className={styles.container}>
      {isLogin ? <Login></Login> : <Register></Register>}  
    <button onClick={handlePanelChange}>{isLogin ? "Login" : "Register"}</button>
    </div>
  )
}

export default AuthPage