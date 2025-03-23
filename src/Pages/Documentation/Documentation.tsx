import React, { useState } from "react";
import ReactMarkdown from "react-markdown";
import style from "./Documentation.module.css";

const sections = [
  { id: "home", label: "Home (Főoldal)" },
  { id: "auth", label: "Auth (Bejelentkezés / Regisztráció)" },
  { id: "contact", label: "Contact (Kapcsolatfelvétel)" },
  { id: "reserve", label: "ReserveRoom (Szoba foglalása)" },
  { id: "reservations", label: "Reservations (Foglalásaim)" },
  { id: "profile", label: "Profil" },
];

const markdownContent = {
  home: `
## 🏠 Home (Főoldal)

### 📋 Leírás:
A nyitóoldal, ahol rövid bemutatkozás és áttekintés található a CozyNest rendszerről.

### ✅ Funkciók:
- Alap információk a szolgáltatásról.
- Navigáció más oldalakra (foglalás, profil stb.).

### ⚠️ Lehetséges hibák:
- Ha valami hiba történik az adatok betöltése közben:  
  **Hibaüzenet:** „Az adatok betöltése sikertelen. Kérjüc, frissítse az oldalt.”
`,
  auth: `
## 🔐 Auth (Bejelentkezés / Regisztráció)

### 📋 Leírás:
Felhasználók beléphetnek a fiókjukba vagy új fiókot hozhatnak létre.

### ✅ Funkciók:
- Bejelentkezés e-maillel és jelszóval.
- Új fiók létrehozása.
- Jelszó biztonsági szabályok ellenőrzése.

### ⚠️ Lehetséges hibák:
- Hibás bejelentkezési adatok:  
  **Hibaüzenet:** „Hibás e-mail vagy jelszó.”
- Már regisztrált e-mail:  
  **Hibaüzenet:** „Ez az e-mail cím már használatban van.”
`,
  contact: `
## ✉️ Contact (Kapcsolatfelvétel)

### 📋 Leírás:
Lehetőség üzenetet küldeni a CozyNest csapatának.

### ✅ Funkciók:
- Név, e-mail, üzenet mezők kitöltése.
- Küldés gomb.

### ⚠️ Lehetséges hibák:
- Üres mezők:  
  **Hibaüzenet:** „Kérjüc, töltsön ki minden mezőt.”
- Hibás e-mail formátum:  
  **Hibaüzenet:** „Érvénytelen e-mail cím.”
- Sikertelen küldés:  
  **Hibaüzenet:** „Az üzenet elküldése nem sikerült. Kérjüc, próbálja újra később.”
`,
  reserve: `## 🛏️ ReserveRoom (Szoba foglalása)

### 📋 Leírás:
Foglalás létrehozása szűrési feltételek alapján (időpont, ár, férőhely).

### ✅ Funkciók:
- Dátum kiválasztása.
- Ár- és férőhely szűrés.
- Szabad szobák listázása.
- Foglalás leadása.

### ⚠️ Lehetséges hibák:
- Nincs találat:  
  **Hibaüzenet:** „Nincs elérhető szoba a megadott feltételekkel.”
- Hibás dátum intervallum:  
  **Hibaüzenet:** „A kezdő dátum nem lehet későbbi a befejező dátumnál.”
- Be nem jelentkezett felhasználó:  
  **Hibaüzenet:** „Foglaláshoz kérjük, jelentkezzen be.”`, // truncated for brevity
  reservations: `## 📆 Reservations (Foglalásaim)

### 📋 Leírás:
A felhasználó eddigi és jövőbeni foglalásai.

### ✅ Funkciók:
- Foglalások listázása dátummal, státusszal.
- Foglalás részleteinek megtekintése.
- (Opcionálisan) Foglalás lemondása.

### ⚠️ Lehetséges hibák:
- Nincs még foglalás:  
  **Üzenet:** „Még nem rendelkezik foglalással.”
- Betöltési hiba:  
  **Hibaüzenet:** „Nem sikerült betölteni a foglalásokat.”`, // truncated for brevity
  profile: `## 👤 Profil

### 📋 Leírás:
Felhasználói adatok megtekintése és szerkesztése.

### ✅ Funkciók:
- Név, e-mail, jelszó módosítása.
- Profilkép feltöltés (ha van ilyen funkció).
- Előfizetés kezelése (ha van ilyen funkció).

### ⚠️ Lehetséges hibák:
- Üres kötelező mező:  
  **Hibaüzenet:** „Minden mező kitöltése kötelező.”
- Jelszó túl rövid vagy gyenge:  
  **Hibaüzenet:** „A jelszónak minimum 8 karakter hosszúnak kell lennie.”
- Sikertelen mentés:  
  **Hibaüzenet:** „A változtatások mentése nem sikerült.”`, // truncated for brevity
};

const Documentation = () => {
  const [activeSection, setActiveSection] = useState("home");

  return (
    <div className={style.container}>
      <aside className={style.sidebar}>
        <h2>Dokumentáció</h2>
        <ul>
          {sections.map((section) => (
            <li key={section.id}>
              <button
                className={
                  activeSection === section.id ? style.activeLink : style.link
                }
                onClick={() => setActiveSection(section.id)}
              >
                {section.label}
              </button>
            </li>
          ))}
        </ul>
      </aside>
      <main className={style.content}>
        <ReactMarkdown>{markdownContent[activeSection]}</ReactMarkdown>
      </main>
    </div>
  );
};

export default Documentation;
