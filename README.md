# CozyNest Fejlesztői Dokumentáció

## Autentikációs attribútumok

### `RequireAccessToken` Attribute 🔐

#### Szerepe
A `RequireAccessToken` attribútum biztonsági rétegként működik, és gondoskodik arról, hogy az adott API végpont csak érvényes access token birtokában legyen elérhető.

#### Működés
- Ellenőrzi a `Bearer` típusú tokent az `Authorization` fejlécben
- A tokenhez tartozó felhasználót a `UserHandler.GetUserByAccessToken` keresési metódus segítségével azonosítja
- Lezárt félkész fók (user.Closed == true) esetén visszautasítja a kérést
- A felhasználó szerepkörének érvényességét is ellenőrzi
- A token, user és role objektumokat eltárolja a `HttpContext.Items`-ben

#### Válaszkódok
- `401 Unauthorized`: hiányzó vagy hibás token
- `403 Forbidden`: érvénytelen token, lezárt fók vagy érvénytelen szerepkör

### `RequireRefreshToken` Attribute 🔄

#### Szerepe
A `RequireRefreshToken` attribútum a frissítési token (refresh token) érvényességét vizsgálja token megújítás céljából.

#### Működés
- Ellenőrzi a fejlécet, majd kivonja a tokent
- Lekérdezi a `UserHandler.GetUserByRefreshToken` segítségével a felhasználót
- Ellenőrzi a felhasználó fókjának állapotát és szerepkörét
- Token, user és role a `HttpContext.Items`-be kerül

#### Válaszkódok
- `401 Unauthorized`: hiányzó vagy hibás token
- `403 Forbidden`: érvénytelen refresh token, lezárt fók, hibás szerepkör

---

## API Kontrollerek

### `AccountController` 👤
Felhasználói műveletek (bejelentkezés, regisztráció, tokenek kezelése stb.)

#### Végpontok
- `POST api/account/login` – Bejelentkezés
- `POST api/account/register` – Regisztráció
- `GET api/account/logout` – Kijelentkezés
- `GET api/account/introspect` – Token introspection
- `GET api/account/renewtoken` – Token megújítása
- `PUT api/account/updatedata` – Felhasználói adatok frissítése
- `DELETE api/account/deleteaccount` – Fók lezárása
- `GET api/account/logouteverywhere` – Kijelentkezés minden eszközről

---

### `AdminController` 🛠️
Adminisztrációs végpontok (felhasználók, foglalások, szolgáltatások kezelése)

#### Végpontok
- `GET api/admin/getusers` – Felhasználók lekérése
- `GET api/admin/getroles` – Szerepkörök lekérése
- `PUT api/admin/modifyuser` – Felhasználó módosítása
- `POST api/admin/adduser` – Új felhasználó létrehozása
- `POST api/admin/addreservation` – Új foglalás adminisztratív létrehozása
- `DELETE api/admin/cancelreservation` – Foglalás lemondása
- `GET api/admin/getreservations` – Foglalások listázása
- `POST api/admin/getreservations` – Foglalások lekérése adott felhasználóhoz
- `POST api/admin/addservice` – Új szolgáltatás létrehozása
- `POST api/admin/modifyservice` – Szolgáltatás módosítása

---

### `ReservationController` 🛎️
Foglalások felhasználó általi kezelésére szolgál.

#### Végpontok
- `GET api/reservation/getreservations` – Saját foglalások lekérése
- `DELETE api/reservation/reserve` – Foglalás létrehozása
- `POST api/reservation/cancel` – Saját foglalás lemondása
- `GET api/reservation/getrooms` – Szobák listázása
- `POST api/reservation/getrooms` – Szabad szobák adott időintervallumban

---

### `RoomController` 🚪
Szobák menedzselése menedzseri jogosultsággal.

#### Végpontok
- `GET api/room/list` – Szobák lekérése
- `POST api/room/create` – Új szoba létrehozása
- `DELETE api/room/delete` – Szoba logikai törlése
- `PUT api/room/modify` – Szoba módosítása

---

### `ServiceController` 💡
Szolgáltatások lekérdezése.

#### Végpontok
- `GET api/service/services` – Összes szolgáltatás lekérése
- `POST api/service/services` – Szolgáltatások lekérdezése foglaláshoz

---

## Backend Handler osztályok

### `ReservationHandler` 🛠️
Foglalások, szolgáltatások és foglalási állapotok kezelése.

#### Funkciók
- Foglalások lekérdezése, létrehozása, módosítása
- Felhasználó foglalásainak lekérdezése
- Szolgáltatások hozzárendelése foglalásokhoz
- Szabad szobák lekérdezése időintervallum szerint
- Foglalási állapotok kezelése (lekérdezés, azonosítás)
- Token-alapú konkurens olvasási/írási védelem biztosítása

---

### `RoomHandler` 🚪
A szobák és azok típusainak/státuszainak kezelése.

#### Funkciók
- Szobák lekérdezése (`GetRooms`, `GetRoomById`, `GetRoomByRoomNumber`)
- Szoba létrehozása: `CreateRoom`
- Szoba módosítása: `ModifyRoom`
- Szobatípusok/státuszok lekérdezése: `GetRoomTypes`, `GetRoomStatuses`
- Típus és státusz lekérdezés ID vagy név alapján: `GetRoomTypeById`, `GetRoomStatusById`, `GetRoomTypeByDescription`, `GetRoomStatusByDescription`

---

*Utolsó frissítés: 2025.03.31.*

