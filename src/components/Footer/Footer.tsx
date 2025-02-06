import styles from "./Footer.module.css";

const Footer = () => {
  return (
    <div className={styles.container}>
      <div className={styles.left}>
        <div className={styles.menu}>
          <h1 className={styles.name}>COZYNEST</h1>
          <div>
            <p>
              Elérhetőség: 
            </p>
          </div>
          <div></div>
        </div>
      </div>
      <div className={styles.middle}>
        <div className={styles.menu}>
          <a href="/#home">Főoldal</a>
          <a href="/#story">Történet</a>
          <a href="/#info">Információ</a>
          <a href="/#contact">Elérhetőség</a>
          <a href="/login">Bejelentkezés</a>
          <a href="/register">Regisztráció</a>
        </div>
      </div>
      <div className={styles.right}>
        <div className={styles.menu}>
          <a href="/#home">Főoldal</a>
          <a href="/#story">Történet</a>
          <a href="/#info">Információ</a>
          <a href="/#contact">Elérhetőség</a>
          <a href="/login">Bejelentkezés</a>
          <a href="/register">Regisztráció</a>
        </div>
      </div>
    </div>
  );
};

export default Footer;
