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
          <h3>SERVICES</h3>
          <ul>
            <li><a href="/profile">Your Account</a></li>
            <li><a href="/profile#rooms">Your Rooms</a></li>
            <li><a href="/profile#edit">Account Information</a></li>
            <li><a href="/rooms">Order Room</a></li>
          </ul>
        </div>
        <div className={styles.column}>
          <h3>USEFUL LINKS</h3>
          <ul>
            <li><a href="/">Main page</a></li>
            <li><a href="/#contact">Contact Form</a></li>
          </ul>
        </div>
        <div className={styles.column}>
          <h3>CONTACT</h3>
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
