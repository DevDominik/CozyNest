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
          <div id="google-maps-display">
            <iframe className="google-maps" src="https://www.google.com/maps/embed/v1/place?q=budapest&key=AIzaSyBFw0Qbyq9zTFTd-tUY6dZWTgaQzuU17R8"></iframe>
          </div>
        </div>
        <p>
          Lorem ipsum, dolor sit amet consectetur adipisicing elit. Magni,
          corrupti! A id consequuntur necessitatibus omnis natus minus
          voluptatibus aperiam voluptate maxime earum accusantium laboriosam
          animi eius aliquam nam, obcaecati labore?
        </p>
      </div>
      <div className={styles.ContactForm}>
        <form onSubmit={onSubmit}>
          <div className={styles.Container}>
            <input type="hidden" name="from_name" value="BOROS WEBSITE"></input>
            <input type="text" name="subject" placeholder="Tárgy"></input>
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
