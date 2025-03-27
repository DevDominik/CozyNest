import { useState } from "react";
import styles from "./Contact.module.css";

export default function Contact() {
  const [result, setResult] = useState("");

  const onSubmit = async (event) => {
    event.preventDefault();
    setResult("Küldés....");
    const formData = new FormData(event.target);

    formData.append("access_key", "2d138e29-9c16-4ab9-8f8d-670deaa0d06e");

    const response = await fetch("https://api.web3forms.com/submit", {
      method: "POST",
      body: formData,
    });

    const data = await response.json();

    if (data.success) {
      setResult("Üzenet sikeresen elküldve!");
      event.target.reset();
    } else {
      console.log("Error", data);
      setResult(data.message);
    }
  };

  return (
    <div className={styles.FormContainer}>
      <div className={styles.ContactInfo}>
        <div>
          <img className={styles.gmailImage} src="./gmail.png" alt="Gmail Logo" />
          <h1>Kapcsolat</h1>
        </div>
        <p>
          Ha bármilyen kérdése van, vagy további információra van szüksége,
          lépjen kapcsolatba velünk! Töltse ki az alábbi űrlapot, és csapatunk
          hamarosan válaszol Önnek.
        </p>
  
        {/* További elérhetőségi információk */}
        <div className={styles.ContactDetails}>
          <h3>Support</h3>
          <p>Hétfő - Péntek: 9:00 - 17:00</p>
        </div>
      </div>
  
      <div className={styles.ContactForm}>
        <form onSubmit={onSubmit}>
          <div className={styles.Container}>
            <input type="hidden" name="from_name" value="COZYNEST" />
            <input type="text" name="subject" placeholder="Tárgy" required/>
            <input type="text" name="name" placeholder="Név" required />
            <input type="email" name="email" placeholder="Email" required />
            <textarea name="message" placeholder="Üzenet" required></textarea>
  
            <button type="submit">Elküldés</button>
            <br />
            <span>{result}</span>
          </div>
        </form>
      </div>
    </div>
  );
  
}
