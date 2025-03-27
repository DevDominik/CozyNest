import React, { useState } from "react";
import ReactMarkdown from "react-markdown";
import style from "./Documentation.module.css";

const sections = [
  { id: "auth", label: "Auth (Bejelentkezés / Regisztráció)" },
  { id: "home", label: "Home (Főoldal)" },
  { id: "reserve", label: "ReserveRoom (Szoba foglalása)" },
  { id: "reservations", label: "Reservations (Foglalásaim)" },
  { id: "profile", label: "Profil" },
  { id: "contact", label: "Contact (Kapcsolatfelvétel)" },
  { id: "support", label: "Technikai támogatás" },
  { id: "faq", label: "GYIK (Gyakori kérdések)" },
  { id: "terms", label: "Felhasználási feltételek" },
  { id: "privacy", label: "Adatvédelem" },
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
  **Hibaüzenet:** „Az adatok betöltése sikertelen. Kérjük, frissítse az oldalt.”
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
auth: `
## 🔐 Auth (Bejelentkezés / Regisztráció)

### 📋 Leírás:
A bejelentkezés és regisztráció képernyő lehetővé teszi a felhasználók számára, hogy hozzáférjenek CozyNest fiókjukhoz, vagy új fiókot hozzanak létre.

### ✅ Funkciók:
- Bejelentkezés e-mail címmel és jelszóval.
- Regisztráció új fiók létrehozásához.
- Jelszó mezőben biztonsági ellenőrzések (min. 8 karakter, nagybetű, szám stb.).
- „Elfelejtett jelszó” funkció.

### 📑 Regisztrációs követelmények

#### ✅ Felhasználónév követelmények
- Nem kezdődhet számmal vagy speciális karakterrel
- Csak betűket (\`a–z\`, \`A–Z\`), számokat (\`0–9\`) és aláhúzásjeleket (\`_\`) tartalmazhat
- Legalább **3 karakter** hosszú legyen
- Legfeljebb **20 karakter** hosszú lehet

#### ✅ Jelszó követelmények
- Legalább **8 karakter** hosszú legyen
- Tartalmazzon legalább:
  - **1 nagybetűt** (pl. \`A\`)
  - **1 kisbetűt** (pl. \`a\`)
  - **1 számot** (pl. \`1\`)
  - **1 speciális karaktert** (pl. \`!@#$%^&*\`)
- **Ne tartalmazzon szóközt**
- **Ne tartalmazza a felhasználónevet vagy az e-mail címet**

### 🧭 Navigáció:
- Automatikus átirányítás a főoldalra sikeres belépés után.
- Hiba esetén a mezők alatt hibaüzenet jelenik meg.

### ⚠️ Lehetséges hibák:

#### 1. Hibás bejelentkezési adatok  
**Hibaüzenet:** „Hibás e-mail vagy jelszó.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 2. Már regisztrált e-mail  
**Hibaüzenet:** „Ez az e-mail cím már használatban van.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 3. Hiányzó mezők  
**Hibaüzenet:** „Kérjük, töltse ki az összes mezőt.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 4. Érvénytelen e-mail formátum  
**Hibaüzenet:** „Érvénytelen e-mail cím.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 5. Érvénytelen felhasználónév  
**Hibaüzenetek:**
- „A felhasználónév nem kezdődhet számmal vagy speciális karakterrel.”
- „A felhasználónév csak betűket, számokat és aláhúzásjeleket tartalmazhat.”
- „A felhasználónév túl rövid. Minimum 3 karakter szükséges.”
- „A felhasználónév túl hosszú. Maximum 20 karakter engedélyezett.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 6. Érvénytelen jelszó  
**Hibaüzenetek:**
- „A jelszónak legalább 8 karakter hosszúnak kell lennie.”
- „A jelszónak tartalmaznia kell legalább egy nagybetűt.”
- „A jelszónak tartalmaznia kell legalább egy kisbetűt.”
- „A jelszónak tartalmaznia kell legalább egy számot.”
- „A jelszónak tartalmaznia kell legalább egy speciális karaktert (!@#$%^&*).”
- „A jelszó nem tartalmazhat szóközt.”
- „A jelszó nem tartalmazhatja a felhasználónevet vagy az e-mail címet.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 7. Jelszavak nem egyeznek  
**Hibaüzenet:** „A megadott jelszavak nem egyeznek.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 8. Ismeretlen hiba / szerverhiba  
**Hibaüzenet:** „Ismeretlen hiba történt. Kérjük, próbálja újra később.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`
,
  contact: `
## ✉️ Contact (Kapcsolatfelvétel)

### 📋 Leírás:
Lehetőség üzenetet küldeni a CozyNest csapatának.

### ✅ Funkciók:
- Név, e-mail, üzenet mezők kitöltése.
- Küldés gomb.

### ⚠️ Lehetséges hibák:
- Üres mezők:  
  **Hibaüzenet:** „Kérjük, töltsön ki minden mezőt.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
- Hibás e-mail formátum:  
  **Hibaüzenet:** „Érvénytelen e-mail cím.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
- Sikertelen küldés:  
  **Hibaüzenet:** „Az üzenet elküldése nem sikerült. Kérjük, próbálja újra később.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
  reserve: `
## 🛏️ ReserveRoom (Szoba foglalása)

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
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
- Hibás dátum intervallum:  
  **Hibaüzenet:** „A kezdő dátum nem lehet későbbi a befejező dátumnál.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
- Be nem jelentkezett felhasználó:  
  **Hibaüzenet:** „Foglaláshoz kérjük, jelentkezzen be.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
  reservations: `
## 📆 Reservations (Foglalásaim)

### 📋 Leírás:
A felhasználó eddigi és jövőbeni foglalásai.

### ✅ Funkciók:
- Foglalások listázása dátummal, státusszal.
- Foglalás részleteinek megtekintése.
- Foglalás lemondása (ha engedélyezett).

### ⚠️ Lehetséges hibák:
- Nincs még foglalás:  
  **Üzenet:** „Még nem rendelkezik foglalással.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
- Betöltési hiba:  
  **Hibaüzenet:** „Nem sikerült betölteni a foglalásokat.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
  profile: `
## 👤 Profil

### 📋 Leírás:
Felhasználói adatok megtekintése és szerkesztése.

### ✅ Funkciók:
- Név, e-mail, jelszó módosítása.
- Profilkép feltöltése (ha van ilyen funkció).
- Előfizetés kezelése (ha van ilyen funkció).

### ⚠️ Lehetséges hibák:
- Üres kötelező mező:  
  **Hibaüzenet:** „Minden mező kitöltése kötelező.”  
  ![Hiba képernyőképe](/loginError.png)
- Jelszó túl rövid vagy gyenge:  
  **Hibaüzenet:** „A jelszónak minimum 8 karakter hosszúnak kell lennie.”  
  ![Hiba képernyőképe](/loginError.png)
- Sikertelen mentés:  
  **Hibaüzenet:** „A változtatások mentése nem sikerült.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
  faq: `
## ❓ GYIK (Gyakran Ismételt Kérdések)

### 🔹 Hogyan tudok szobát foglalni?
1. Jelentkezzen be fiókjába.
2. Kattintson a "Szoba foglalása" menüpontra.
3. Válassza ki az időpontot, árkategóriát és férőhelyet.

### 🔹 Elfelejtettem a jelszavam. Mit tegyek?
Használja az „Elfelejtett jelszó” linket a bejelentkezési oldalon.

### 🔹 Lemondhatom a foglalásom?
Igen, a „Foglalásaim” menüpontban található „Lemondás” opcióval.

### 🔹 Milyen böngészők támogatottak?
- Google Chrome (legfrissebb verzió)
- Mozilla Firefox
- Safari
- Microsoft Edge
`,
  terms: `
## 📜 Felhasználási feltételek

A CozyNest használatával Ön elfogadja az alábbi feltételeket:

- Valós adatokat ad meg regisztrációkor.
- Nem használja a rendszert visszaélésszerűen.
- A foglalási feltételek változhatnak, erről értesítést kap.

A teljes feltételek elérhetők a hivatalos weboldalon.
`,
  privacy: `
## 🔐 Adatvédelmi nyilatkozat

- Adatokat kizárólag a szolgáltatás működéséhez gyűjtünk.
- Nem adjuk tovább harmadik félnek.
- Kérésre bármikor törölhetők az adatok.

Részletek: [Adatkezelési szabályzat](https://localhost/adatvedelem)
`,
  support: `
## 🛠️ Technikai támogatás

📧 E-mail: support@cozynest.hu  
📞 Telefon: +36 1 234 5678  
💬 Élő chat: elérhető a jobb alsó sarokban.

### 🕘 Ügyfélszolgálati idő:
- Hétfőtől Péntekig: 09:00 - 18:00
- Hétvégén: korlátozott elérhetőség

### 💡 Tipp:
Először tekintse meg a GYIK szekciót a gyors megoldásokért.
`,
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
        <ReactMarkdown
          components={{
            img: ({ node, ...props }) => (
              <img
                {...props}
                style={{
                  maxWidth: "100%",
                  width: "300px",
                  borderRadius: "8px",
                }}
                alt={props.alt}
              />
            ),
          }}
        >
          {markdownContent[activeSection]}
        </ReactMarkdown>
      </main>
    </div>
  );
};

export default Documentation;
