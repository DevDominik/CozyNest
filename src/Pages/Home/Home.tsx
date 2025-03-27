import { motion, useScroll } from "framer-motion";
import styles from "./Home.module.css";
import Contact from "../../components/Contact-Form/Contact";

const Home = () => {
  const { scrollYProgress } = useScroll();
  return (
    <>
      <motion.div
        id="scroll-indicator"
        style={{
          scaleX: scrollYProgress,
          position: "fixed",
          top: 0,
          left: 0,
          right: 0,
          height: 1,
          originX: 0,
          backgroundColor: "var(--text-color-one)",
        }}
      />
      <Content />
    </>
  );
};

function Content() {
  return (
    <div className={styles.HomePage} id="home">
      <section className={styles.Section1}>
        <div className={styles.Inner}>
          <h1 className={styles.CompanyName}>COZYNEST</h1>
          <h2 className={styles.Slogan}>Egy kattintás a nyugodt pihenéshez.</h2>
        </div>
      </section>
      <section className={styles.Section2} id="story">
        <div className={styles.StoryContainer}>
          <motion.div
            className={styles.StoryImageContainer}
            initial={{ x: "-100%", opacity: 0 }}
            whileInView={{ x: 0, opacity: 1 }}
            transition={{ type: "spring", stiffness: 50, damping: 20 }}
            viewport={{ once: true }}
          ></motion.div>

          <div className={styles.StoryTextConatiner}>
            <motion.p
              className={styles.StoryText}
              initial={{ x: "100%", opacity: 0 }}
              animate={{ x: 0, opacity: 1 }}
              transition={{ type: "spring", stiffness: 50, damping: 20 }}
            >
              Engedd, hogy a CozyNest elvezessen a tökéletes kikapcsolódáshoz!
              Találd meg álmaid szállását gyorsan, egyszerűen és biztonságosan.
              Intuitív foglalási felületünk és megbízható partnereink
              garantálják a nyugodt pihenést – akár egy hétvégére, akár egy
              hosszabb vakációra készülsz.
            </motion.p>
          </div>
        </div>
      </section>
      <section className={styles.Section3} id="info">
        <div className={styles.informations}>
          <div className={styles.Style33}>
            <p>
              🌍 Széles választék: Több száz minőségi szálláshely közül
              válogathatsz országosan – városban, vidéken vagy akár vízparton
              is.
            </p>
          </div>
          <div className={styles.Style33}>
            <p>
              🔒 Biztonságos foglalás: Adatvédelmi szempontból garantáltan
              megbízható rendszerünk gondoskodik adataid védelméről és
              biztonságáról.
            </p>
          </div>
          <div className={styles.Style33}>
            <div className={styles.Card}>
              <h3>⭐⭐⭐⭐⭐</h3>
              <p>
                „Nagyon egyszerű volt a foglalás! A szállás tökéletes volt,
                minden pontosan olyan, mint a képeken. Csak ajánlani tudom a
                CozyNestet!”
              </p>
              <p>
                <strong>– Dóra, Budapest</strong>
              </p>
            </div>
            <div className={styles.Card}>
              <h3>⭐⭐⭐⭐</h3>
              <p>
                „Gyors visszaigazolás, remek ügyfélszolgálat és szuper árak. Egy
                hétvégés kiruccanáshoz tökéletes megoldás.”
              </p>
              <p>
                <strong>– Ádám, Debrecen</strong>
              </p>
            </div>
            <div className={styles.Card}>
              <h3>⭐⭐⭐⭐⭐</h3>
              <p>
                „Imádtam, hogy ennyire átlátható a rendszer! Nem kellett
                telefonálni, minden online ment, gördülékenyen.”
              </p>
              <p>
                <strong>– Lilla, Szeged</strong>
              </p>
            </div>
          </div>
        </div>
      </section>
      <section className={styles.Section4} id="contact">
        <Contact></Contact>
      </section>
    </div>
  );
}

export default Home;
