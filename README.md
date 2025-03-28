
# 📘 CozyNest Dokumentációs Portál (React + TypeScript)

Ez a projekt egy dokumentációs oldal a **CozyNest szállodakezelő rendszerhez**, amely React és TypeScript technológiával készült. Az oldal segít a felhasználóknak megismerni a rendszer különböző moduljait, funkcióit, hibakezelését, valamint általános használati útmutatót nyújt.

---

## 💻 Rendszerkövetelmények

### 🖥️ Operációs rendszer
- Windows 10 (64 bites)

### 🧰 Alkalmazások és eszközök
| Eszköz | Verzió / Javasolt |
|-------|--------------------|
| [Node.js](https://nodejs.org/) | v18.x vagy újabb |
| [npm](https://www.npmjs.com/) vagy [Yarn](https://yarnpkg.com/) | npm 9.x vagy újabb |
| [Git](https://git-scm.com/) | Javasolt verzió: 2.30+ |
| [Visual Studio Code](https://code.visualstudio.com/) | (ajánlott fejlesztéshez) |
| Modern böngésző | Google Chrome, Firefox, Edge, Safari |

---

## 📦 Telepített csomagok

A projekt az alábbi kulcsfontosságú technológiákat használja:

- React `^18.3.1`
- TypeScript `~5.6.2`
- Vite `^6.0.5` – fejlesztési szerver és build tool
- React Router DOM `^7.1.5` – útvonalkezelés
- React Markdown `^10.1.0` – markdown tartalom renderelése
- Lucide React `^0.484.0` – ikonkészlet
- Motion `^11.18.1` – animációk
- hCaptcha React `^1.11.1` – captcha védelem (ha aktiválva van)
- ESLint & TypeScript linting

---

## 🛠️ Telepítés lépései

1. **Projekt klónozása:**

```bash
git clone https://github.com/felhasznalo/boros-website.git
cd boros-website
```

2. **Függőségek telepítése:**

```bash
npm install
# vagy ha Yarn-t használsz:
# yarn install
```

3. **Fejlesztői szerver indítása:**

```bash
npm run dev
# vagy
# yarn dev
```

A projekt ezután elérhető lesz böngészőben:  
👉 [http://localhost:5173](http://localhost:5173)

---

## 🧪 Hasznos parancsok

| Parancs | Leírás |
|--------|--------|
| `npm run dev` | Fejlesztői szerver indítása (Vite) |
| `npm run build` | Production build (TypeScript + Vite) |
| `npm run preview` | Build előnézet futtatása |
| `npm run lint` | Kódellenőrzés (ESLint) |

---

## 🧭 Navigáció a Dokumentációs oldalon

A dokumentáció oldal tartalmazza az alábbi modulokat:

- 🏠 **Főoldal** – Áttekintés, előnyök, kapcsolat
- 🔐 **Bejelentkezés / Regisztráció**
- 🛏️ **Szobák listázása**
- 📆 **Foglalásaim**
- 🛌 **Szoba foglalása**
- 👤 **Profil szerkesztés**
- ✉️ **Kapcsolatfelvétel**
- ❓ **GYIK**
- 📜 **Felhasználási feltételek**
- 🔐 **Adatvédelem**
- 🛠️ **Technikai támogatás**

A felhasználó a bal oldali menüből választhat szekciót, a tartalom Markdown formátumban dinamikusan betöltődik.

---

## 🖱️ Perifériás eszközök (általános követelmény)

- Billentyűzet és egér (vagy érintőképernyő)
- Képernyőfelbontás: minimum **1280x720 (HD)** ajánlott
- Internetkapcsolat szükséges a `npm install` és fejlesztői szerver futtatásához

---

## 📁 Projekt struktúra (kivonat)

```
boros-website/
├── src/
│   ├── components/
│   │   └── Documentation.tsx   // Dokumentáció fő komponens
│   ├── assets/                 // Képek, ikonok
│   ├── styles/
│   │   └── Documentation.module.css
├── public/
│   └── ... (pl. loginError.png stb.)
├── package.json
├── tsconfig.json
└── vite.config.ts
```

---

## 🧯 Hibakezelés

- Ellenőrizd, hogy a `node_modules` sikeresen települt-e
- Ha nem indul el a projekt:
  - Ellenőrizd, hogy a Node.js verziód megfelelő
  - Ellenőrizd, hogy nincs más szolgáltatás a 5173-as porton
- Ha valamilyen hiba jelenik meg az oldalon, ellenőrizd a konzolt (DevTools → Console)

---

## 📤 Build & Deployment (haladó)

Production build elkészítése:

```bash
npm run build
```

A build kimenete a `dist/` mappába kerül. Ez a mappa szolgáltatható bármely statikus webkiszolgálón keresztül (pl. Netlify, Vercel, GitHub Pages, Nginx).

---

## 📬 Kapcsolat

Ha hibát találsz, vagy javaslatod van, nyugodtan nyiss egy issue-t a GitHubon.

---

Készült a **CozyNest** rendszerhez – 2025  
Készítő: [Takács Balázs]


---

## 🗄️ Adatbázis struktúra (MariaDB / MySQL)

Az alábbi táblák alkotják a CozyNest rendszer backend adatbázisát. Az adatbázis neve: `cozynest`.

### 🔹 1. `users`
- A felhasználók adatai (admin, recepciós, vendég)
- Fontos mezők:
  - `username`, `hashed_password`, `email`, `role_id`
- Példák:
  - `admin`, `rec`, `user`

### 🔹 2. `roles`
- Szerepkörök: `Guest`, `Receptionist`, `Manager`
- Ezek határozzák meg a felhasználói jogosultságokat.

### 🔹 3. `room`
- Szobák adatai: szobaszám, típus, ár, kapacitás
- Külső kulcs: `roomtype`, `roomstatus`

### 🔹 4. `roomtype`
- Szobatípusok: `Standard`, `Deluxe`, `Suite`

### 🔹 5. `roomstatus`
- Szobastátusz: `Available`, `Occupied`, `Maintenance`

### 🔹 6. `reservations`
- Foglalások adatai: `check_in_date`, `check_out_date`, `status`
- Példa státusz: `Incomplete`, `Complete`, `Cancelled`
- Külső kulcs: `guest_id`, `room_id`, `status`

### 🔹 7. `reservationstatuses`
- Foglalási státuszok:
  - `1` – Incomplete
  - `2` – Complete
  - `3` – Cancelled

### 🔹 8. `reservationservices`
- A foglaláshoz tartozó extra szolgáltatások
- Mennyiséget is tárol (`quantity`)

### 🔹 9. `services`
- Elérhető szolgáltatások:
  - Prémium reggeli (5000 Ft), Wellness (8000 Ft), stb.
- `is_active = 1`: elérhető
- A legtöbb szolgáltatás napi ár alapján számol

### 🔹 10. `tokens`
- Hozzáférési és frissítő tokenek JWT formátumban
- Felhasználók bejelentkezését és biztonságát kezeli

---

## 🧪 Tesztadatok – magyarázat

### 👤 Felhasználók (`users`)
- `admin` – rendszergazda (Manager)
- `rec` – recepciós (Receptionist)
- `user` – vendég (Guest)

### 🏨 Szobák (`room`)
- 5 szoba, változó típusokkal:
  - `101`: Standard, 8000 Ft/éj
  - `102`: Deluxe, 14000 Ft/éj
  - `103`: Suite, 22000 Ft/éj

### 📅 Foglalások (`reservations`)
- 15 foglalás van regisztrálva
- Státusz értékek:
  - `1`: Incomplete
  - `2`: Complete
  - `3`: Cancelled
- Tesztelhetők:
  - dátumellenőrzés
  - vendégkapacitás
  - foglalás státuszváltás

### 🧴 Szolgáltatások (`services`)
- Példák:
  - `Prémium reggeli` – 5000 Ft/nap
  - `Wellness` – 8000 Ft/nap
  - `VIP takarítás` – 3000 Ft/nap
- `is_active = 0`: nem elérhető a frontend számára

### 🔐 Tokenek (`tokens`)
- Tartalmaznak hozzáférési és frissítő tokeneket
- `is_active`: meghatározza, hogy a token aktív-e

---

## 🔗 Kapcsolatok (külső kulcsok)

| Tábla | Külső kulcs | Csatlakozik |
|-------|-------------|-------------|
| `users` | `role_id` → `roles.id` |
| `room` | `type` → `roomtype.id`<br>`status` → `roomstatus.id` |
| `reservations` | `status` → `reservationstatuses.id` |
| `reservationservices` | `reservation_id` → `reservations.id`<br>`service_id` → `services.id` |
| `tokens` | `user_id` → `users.id` |

---

