import React, { useState } from "react";
import ReactMarkdown from "react-markdown";
import style from "./Documentation.module.css";

const sections = [
  { id: "home", label: "Főoldal" },
  { id: "auth", label: "Autentikáció" },
  { id: "rooms", label: "Szállások" },
  { id: "reserve", label: "Szállás foglalása" },
  { id: "reservations", label: "Foglalások" },
  { id: "profile", label: "Profil" },
  { id: "contact", label: "Kapcsolatfelvétel" },
  { id: "support", label: "Technikai támogatás" },
  { id: "faq", label: "GYIK (Gyakori kérdések)" },
  { id: "terms", label: "Felhasználási feltételek" },
  { id: "privacy", label: "Adatvédelem" },
];

const markdownContent = {
  home: `
  ## 🏠 Home (Főoldal)
  
  ### 📋 Leírás:
  A CozyNest főoldala a nyitóképernyő, amely bemutatja a szolgáltatás lényegét, kiemeli annak előnyeit, és bizalmat épít a látogatókban. A letisztult design és a gördülékeny animációk célja, hogy barátságos és modern élményt nyújtson már az első pillanattól.
  
  ### 🗂️ Szekciók áttekintése:
  
  #### 1. Bemutatkozás
  - **Logó:** COZYNEST
  - **Szlogen:** „Egy kattintás a nyugodt pihenéshez.”
  - A márka bemutatása, bizalomépítés, első benyomás.
  
  #### 2. Rólunk szekció
  - Rövid szöveges bemutató arról, hogyan segít a CozyNest szállást találni.
  - Kiemelések:
    - Gyors, egyszerű és biztonságos foglalás
    - Intuitív kezelőfelület
  
  #### 3. Előnyök és vélemények
  - **Szolgáltatás előnyei:**
    - 🌍 Széles szálláskínálat
    - 🔒 Biztonságos adatkezelés
  - **Felhasználói vélemények:**
    - 3 vendég visszajelzése (név, város, értékelés)
    - Segít a döntésben, hitelességet ad
  
  #### 4. Kapcsolat
  - Kapcsolatfelvételi űrlap (\`Contact\` szekció)
  - A látogatók közvetlenül üzenhetnek kérdés vagy érdeklődés esetén
  
  ### ✅ Funkciók:
  - Átlátható szerkezet, szakaszokra bontva
  - Scroll indikátor mutatja a felhasználónak, hol tart az oldalon
  - Animált elemek a tartalom gördülékeny megjelenítéséhez
  - Reszponzív kialakítás mobilra és tabletre is
  
  ### 🧭 Navigáció:
  - Az oldal szakaszai belső hivatkozásokkal érhetők el:
    - \`#story\` – Rólunk
    - \`#info\` – Előnyök és vélemények
    - \`#contact\` – Kapcsolat
  - Sikeres űrlapküldés után visszajelzés jelenik meg
  
  ### ⚠️ Lehetséges hibák:
  
  #### 1. Adatok betöltési hiba  
  **Hibaüzenet:** „Az adatok betöltése sikertelen. Kérjük, frissítse az oldalt.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
  
  #### 2. Kapcsolatfelvétel sikertelen  
  **Hibaüzenet:** „Hiba történt az űrlap elküldésekor.”  
  <div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
  
  `,  
  auth: `
## 🔐 Auth (Bejelentkezés / Regisztráció)

### 📋 Leírás:
A bejelentkezés és regisztráció képernyő lehetővé teszi a felhasználók számára, hogy hozzáférjenek CozyNest fiókjukhoz, vagy új fiókot hozzanak létre.

## ✅ Funkciók
- Bejelentkezés e-mail címmel és jelszóval.
- Regisztráció új fiók létrehozásához.
- Jelszó mezőben biztonsági ellenőrzések (min. 8 karakter, nagybetű, szám stb.).
- „Elfelejtett jelszó” funkció.

## 📑 Regisztrációs követelmények

#### ✅ Felhasználónév követelmények
- A mező kitöltése kötelező
- Nem kezdődhet számmal vagy speciális karakterrel
- Csak betűket (\`a–z\`, \`A–Z\`), számokat (\`0–9\`) és aláhúzásjeleket (\`_\`) tartalmazhat
- Legalább **3 karakter** hosszú legyen
- Legfeljebb **20 karakter** hosszú lehet

#### ✅ Jelszó követelmények
- A mező kitöltése kötelező
- Legalább **8 karakter** hosszú legyen
- Tartalmazzon legalább:
  - **1 nagybetűt** (pl. \`A\`)
  - **1 kisbetűt** (pl. \`a\`)
  - **1 számot** (pl. \`1\`)
  - **1 speciális karaktert** (pl. \`!@#$%^&*\`)
- **Ne tartalmazzon szóközt**
- **Ne tartalmazza a felhasználónevet vagy az e-mail címet**

#### ✅ E-mail követelmények
- A mező kitöltése kötelező
- Érvényes e-mail formátum szükséges (pl. \`nev@domain.hu\`)

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
- „A felhasználónév megadása kötelező.”
- „A felhasználónév nem kezdődhet számmal vagy speciális karakterrel.”
- „A felhasználónév csak betűket, számokat és aláhúzásjeleket tartalmazhat.”
- „A felhasználónév túl rövid. Minimum 3 karakter szükséges.”
- „A felhasználónév túl hosszú. Maximum 20 karakter engedélyezett.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 6. Érvénytelen jelszó  
**Hibaüzenetek:**
- „A jelszó megadása kötelező.”
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

#### 8. Érvénytelen vagy hiányzó e-mail  
**Hibaüzenetek:**
- „Az e-mail cím megadása kötelező.”
- „Érvénytelen e-mail cím formátum.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>

#### 9. Ismeretlen hiba / szerverhiba  
**Hibaüzenet:** „Ismeretlen hiba történt. Kérjük, próbálja újra később.”  
<div className="image-container">📷 *(Ide jön a hiba képernyőképe)*</div>
`,
rooms : `
## 🛏️ Rooms (Szobák)

### 📋 Leírás:
A "Szobák" oldal lehetővé teszi a felhasználók számára, hogy elérhető szállásokat böngésszenek, szűrjenek és foglaljanak a kívánt dátumokra a CozyNest rendszeren belül.


## ✅ Funkciók

- Szobák listázása a kiválasztott érkezési és távozási dátum alapján.
- Szűrés szobatípus szerint (Standard, Deluxe, Suite).
- Szűrés elérhetőség szerint (Elérhető / Nem Elérhető).
- Kulcsszavas keresés szobaszám vagy leírás alapján.
- Ár szerinti szűrés (minimum és maximum érték megadása).
- Foglalás indítása elérhető szobákra.


## 🗓️ Dátumválasztás

- **Érkezési dátum**: minimum 8 nappal a mai nap után választható.
- **Távozási dátum**: legalább 1 nappal az érkezési dátum után választható.
- Ha a dátumok érvénytelenek, figyelmeztető üzenet jelenik meg.


---
## 🔍 Szűrési lehetőségek

### 🛏️ Szobatípus
- Standard
- Deluxe
- Suite

### 🚦 Elérhetőség
- Elérhető
- Nem elérhető

### 💬 Keresés
- Kulcsszavas keresés a szobaszám vagy leírás alapján.

### 💰 Ár szűrés
- Minimum ár (pl. 0 HUF)
- Maximum ár (pl. 300000 HUF)

---

## 📦 Szobakártyák tartalma
- Szobaszám (#)
- Szobatípus
- Leírás
- Ár / éjszaka (HUF)
- Ágyak száma (kapacitás)
- Elérhetőségi státusz
- "Foglalás" gomb (csak elérhető szobák esetén aktív)

---

## 🧭 Navigáció
- A "Foglalás" gomb kattintására átirányítás történik a foglalási oldalra, a kiválasztott szoba adatainak átadásával.

---

## ⚠️ Lehetséges hibák

#### 1. Hibás dátumválasztás
**Hibaüzenet:** „A távozási dátumnak későbbinek kell lennie, mint az érkezési dátum. Kérjük, módosítsa a dátumokat.”

#### 2. Nincs találat a szűrők alapján
**Hibaüzenet:** „Nincs elérhető szoba a megadott szűrők alapján. Próbálja meg módosítani a keresési feltételeket.”

#### 3. Hálózati / szerverhiba
**Hibaüzenet:** „Failed to fetch rooms.”

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
