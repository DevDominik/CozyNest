import styles from "./Footer.module.css";

const Footer = () => {
  return (
    <div className={styles.footerContainer}>
      <div className={styles.footerContent}>
        <div className={styles.column}>
          <h3>COZYNEST</h3>
          <p>Here you can use rows and columns to organize your footer content. Lorem ipsum dolor sit amet, consectetur adipisicing elit.</p>
        </div>
        <div className={styles.column}>
          <h3>SZOLGÁLTATÁSOK</h3>
          <ul>
            <li><a href="/profile">Profil</a></li>
            <li><a href="/reservations">Meglévő foglalásaid</a></li>
            <li><a href="/rooms">Szoba Foglalás</a></li>
          </ul>
        </div>
        <div className={styles.column}>
          <h3>HASZNOS LINKEK</h3>
          <ul>
            <li><a href="/">Main page</a></li>
            <li><a href="/#contact">Contact Form</a></li>
          </ul>
        </div>
        <div className={styles.column}>
          <h3>KONTAKT</h3>
          <ul>
            <li>📍 Debrecen, Valahol, HU</li>
            <li>📧 info@cozynest.com</li>
            <li>📞 + 36 30 567 88</li>
            <li>📠 + 36 30 567 89</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default Footer;
