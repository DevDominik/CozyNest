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
          <h2 className={styles.Slogan}>Valami Slogan</h2>
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
              Foglalj álmaid szállodájába a CozyNest segítségével!
              Fedezd fel a tökéletes szálláshelyet a CozyNest szállodamenedzsment rendszerével. Egyszerű, gyors és biztonságos foglalás néhány kattintással. Exkluzív ajánlatok, valós vendégértékelések és gondtalan pihenés vár rád!
            </motion.p>
          </div>
        </div>
      </section>
      <section className={styles.Section3} id="info">
        <div className={styles.informations}>
          <div className={styles.Style33}>
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit. Id
              asperiores consectetur ducimus dolores culpa, sint vero atque non
              saepe reiciendis ipsam suscipit tenetur eaque excepturi, cum, fugit
              iste totam fuga?
            </p>

          </div>
          <div className={styles.Style33}>
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit. Id
              asperiores consectetur ducimus dolores culpa, sint vero atque non
              saepe reiciendis ipsam suscipit tenetur eaque excepturi, cum, fugit
              iste totam fuga?
            </p>
          </div>
          <div className={styles.Style33}>
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit. Id
              asperiores consectetur ducimus dolores culpa, sint vero atque non
              saepe reiciendis ipsam suscipit tenetur eaque excepturi, cum, fugit
              iste totam fuga?
            </p>
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
