# CozyNest Fejlesztői Dokumentáció
## 🖥️ Futtatási követelmények

- Minimum: Intel i3 processzoros PC
- 8GB RAM, 128GB SSD javasolt
- Operációs rendszer: Windows 10 / 11
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) + [Node.js 18+](https://nodejs.org/)
- MySQL / MariaDB (ajánlott: XAMPP vagy MAMP)

---

## ⚙️ Telepítés

### 🔧 Backend
- Repository: [https://github.com/DevDominik/CozyNest](https://github.com/DevDominik/CozyNest)
- Branch: `backend`
- ASP.NET Core Web API (.NET 8)
- Telepítés: Visual Studio 2022 ajánlott

### 🌐 Frontend
- Branch: `frontend`
- React + Vite + TypeScript projekt
- Telepítés:
  ```bash
  cd frontend
  npm install
  npm run dev
  ```

### 🖥️ WPF (Admin Panel)
- Szintén a `CozyNest` repó része
- Visual Studio 2022-ben nyitható meg (`.csproj` alapján)
## 🗃️ Adatbázis ismertetése

- MariaDB 10.4.32 / MySQL-kompatibilis
- Alapértelmezett adatbázis: `cozynest`

### Típusok és táblák

- **users** – felhasználói adatok
- **roles** – szerepkörök (Guest, Receptionist, Manager)
- **room**, **roomtype**, **roomstatus** – szobák és állapotok
- **reservations**, **reservationstatuses** – foglalások és státuszuk
- **services** – igénybe vehető szolgáltatások
- **reservationservices** – szolgáltatás-foglalás kapcsolatok
- **tokens** – access és refresh tokenek

### Kapcsolatok

- `users.role_id → roles.id`
- `reservations.guest_id → users.id`
- `reservations.room_id → room.id`
- `room.type → roomtype.id`
- `room.status → roomstatus.id`
- `reservationservices.reservation_id → reservations.id`
- `reservationservices.service_id → services.id`
- `tokens.user_id → users.id`

---

## 🧰 Fejlesztői eszközök

- Backend: ASP.NET Core, `mysql.data`
- Frontend: React + TypeScript (Vite)
- Titkosítás: Argon2 (password hashing)
- Verziókezelés: Git + GitHub

---

## 🛠️ Fejlesztési környezet

- Visual Studio 2022 (backend, WPF admin panel)
- Visual Studio Code (frontend)

---

## 🔁 Fejlesztés menete

A fejlesztés során moduláris architektúrát alkalmazunk: külön `Handler` osztályok és `Controller` végpontok.

---
## Autentikációs attribútumok

### `RequireAccessToken` Attribute 🔐

#### Szerepe
A `RequireAccessToken` attribútum biztonsági rétegként működik, és gondoskodik arról, hogy az adott API végpont csak érvényes access token birtokában legyen elérhető.
í
#### Működés
- Ellenőrzi a `Bearer` típusú tokent az `Authorization` fejlécben
- A tokenhez tartozó felhasználót a `UserHandler.GetUserByAccessToken` keresési metódus segítségével azonosítja
- Lezárt félkész fiók (user.Closed == true) esetén visszautasítja a kérést
- A felhasználó szerepkörének érvényességét is ellenőrzi
- A token, user és role objektumokat eltárolja a `HttpContext.Items`-ben

#### Válaszkódok
- `401 Unauthorized`: hiányzó vagy hibás token
- `403 Forbidden`: érvénytelen token, lezárt fiók vagy érvénytelen szerepkör

### `RequireRefreshToken` Attribute 🔄

#### Szerepe
A `RequireRefreshToken` attribútum a frissítési token (refresh token) érvényességét vizsgálja token megújítás céljából.

#### Működés
- Ellenőrzi a fejlécet, majd kivonja a tokent
- Lekérdezi a `UserHandler.GetUserByRefreshToken` segítségével a felhasználót
- Ellenőrzi a felhasználó fiókjának állapotát és szerepkörét
- Token, user és role a `HttpContext.Items`-be kerül

#### Válaszkódok
- `401 Unauthorized`: hiányzó vagy hibás token
- `403 Forbidden`: érvénytelen refresh token, lezárt fiók, hibás szerepkör

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
- `DELETE api/account/deleteaccount` – fiók lezárása
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

### `UserHandler` 👤
Felhasználók, szerepkörök, tokenek és hitelesítés kezelése.

#### Funkciók
- Felhasználók lekérdezése, létrehozása, módosítása (`GetUsers`, `CreateUser`, `ModifyUser`)
- Felhasználók lekérdezése ID vagy felhasználónév alapján (`GetUserById`, `GetUserByUsername`)
- Tokenek generálása, érvényesség vizsgálata, visszavonás (`CreateToken`, `ValidateAccessToken`, `RevokeToken`, stb.)
- JWT access token generálás és aláírás
- Felhasználó beazonosítása access vagy refresh token alapján (`GetUserByAccessToken`, `GetUserByRefreshToken`)
- Szerepkörök lekérdezése név vagy ID szerint, illetve teljes lista (`GetRoleById`, `GetRoleByName`, `GetRoles`)
- Tokenek konkurens védelme olvasás/írás közben

---

### `RoleAttribute` 🔐
Attribútum, amely biztosítja, hogy a metódushoz való hozzáférés csak adott szerepkör(ök) esetén engedélyezett legyen.

#### Használat
```csharp
[Role("Admin")]
public IActionResult GetAdminData() { ... }
```

#### Működés
- Lekéri a `Role` objektumot a `HttpContext.Items`-ből
- Ellenőrzi, hogy a szerepkör szerepel-e az attribútumban megadott listában
- Ha nem, 403-as hibát ad vissza

---

### `RequestTypes` 📦
Az alábbi típusok használatosak a bejövő kérésekhez:

- `LoginRequest`, `RegisterRequest`, `AdminRegisterRequest`: bejelentkezés/regisztráció
- `UserSelfUpdateRequest`, `UserUpdateRequest`: felhasználói adatmódosítás
- `RoomCreateRequest`, `RoomModifyRequest`, `RoomDeleteRequest`: szobakezelés
- `AddServiceRequest`, `ModifyServiceRequest`: szolgáltatás létrehozás/módosítás
- `ReservationRequest`, `ReservationAdminRequest`, `ReservationCancelRequest`, `ReservationTimesRequest`, `ReservationServicesRequest`, `GetReservationsByUserIdRequest`, `UserReservationsRequest`: foglalásokkal kapcsolatos lekérdezések és módosítások
- `AddServiceToReservationRequest`: szolgáltatás foglaláshoz rendelése

---

### `GlobalMethods` 🌐
Globális segédmetódusokat tartalmaz:

- `GetItemFromContext<T>()`: lekér egy objektumot a `HttpContext.Items`-ből
- `HashPassword()`, `VerifyPassword()`: Argon2 jelszókódolás és ellenőrzés
- `GenerateRandomPassword()`: véletlenszerű numerikus jelszógenerálás (8 karakter)

---

### `Program.cs` ⚙️
Alkalmazásindító fájl, ahol beállításra kerül:

- CORS (`AllowAll`)
- Tokenkezelés inicializálása (`UserHandler.Initialize()`, stb.)
- Swagger / HTTPS / Controller mapping

---

### `Modellek` 🧩
Az alábbi osztályok reprezentálják az adatbázis entitásokat:

- `User`: felhasználói adatok (név, email, jelszó, cím, szerepkör, zártság, csatlakozás dátuma)
- `Token`: access és refresh token + lejárati idő, aktivitás
- `Role`: szerepkör ID és név
- `Room`: szobák adatai (szám, típus, ár, státusz, kapacitás, törlés flag)
- `RoomType`, `RoomStatus`: típus/státusz ID és leírás
- `Reservation`: foglalás adatok (vendég, szoba, időpontok, státusz, megjegyzés, kapacitás)
- `ReservationStatus`: foglalási státusz leírás
- `ReservationService`: foglaláshoz tartozó szolgáltatás és mennyiség
- `Service`: szolgáltatás név, leírás, ár, aktív státusz
- `Payment`: fizetés adatai (foglalás, dátum, összeg, mód, státusz)
- `PaymentMethod`, `PaymentStatus`: fizetési módok és státuszok leírással

---

### 📥 Példák a Request body és header használatra

#### 🔐 `LoginRequest`
**POST** `api/account/login`
```json
{
  "username": "john_doe",
  "password": "mySecret123"
}
```

#### 📝 `RegisterRequest`
**POST** `api/account/register`
```json
{
  "username": "john_doe",
  "password": "mySecret123",
  "email": "john@example.com"
}
```

#### 👤 `AdminRegisterRequest`
**POST** `api/admin/adduser`
```json
{
  "username": "admin_user",
  "password": "admin1234",
  "email": "admin@cozynest.com",
  "role": "Admin",
  "firstName": "Anna",
  "lastName": "Kovács",
  "address": "Budapest, Andrássy út 1."
}
```

#### ✏️ `UserSelfUpdateRequest`
**PUT** `api/account/updatedata`
```json
{
  "username": "johnny",
  "password": "newPassword",
  "email": "newemail@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "address": "New Street 12"
}
```
**Header:** `Authorization: Bearer <access_token>`

#### 🔒 `UserUpdateRequest`
**PUT** `api/admin/modifyuser`
```json
{
  "id": 4,
  "username": "admin1",
  "closed": false,
  "roleName": "Manager",
  "passwordReset": true
}
```
**Header:** `Authorization: Bearer <admin_access_token>`

#### 🏨 `RoomCreateRequest`
**POST** `api/room/create`
```json
{
  "roomNumber": "B203",
  "capacity": 3,
  "typeDescription": "Deluxe",
  "pricePerNight": 169.90,
  "statusDescription": "Elérhető",
  "description": "Tágas szoba erkéllyel"
}
```
**Header:** `Authorization: Bearer <manager_access_token>`

#### 🛠️ `RoomModifyRequest`
**PUT** `api/room/modify`
```json
{
  "roomId": 2,
  "roomNumber": "B204",
  "capacity": 4,
  "typeDescription": "Suite",
  "pricePerNight": 200.00,
  "statusDescription": "Karbantartás",
  "description": "Felújítás alatt"
}
```

#### ❌ `RoomDeleteRequest`
**DELETE** `api/room/delete`
```json
{
  "roomId": 5
}
```

#### ➕ `AddServiceRequest`
**POST** `api/admin/addservice`
```json
{
  "name": "Reggeli",
  "description": "Kontinentális reggeli a szállodában",
  "price": 15.00
}
```

#### ✏️ `ModifyServiceRequest`
**POST** `api/admin/modifyservice`
```json
{
  "id": 1,
  "name": "Reggeli",
  "description": "Frissített leírás",
  "price": 17.00,
  "isActive": true
}
```

#### 🛏️ `ReservationRequest`
**DELETE** `api/reservation/reserve`
```json
{
  "roomNumber": "B203",
  "capacity": 2,
  "checkInDate": "2025-06-01T14:00:00",
  "checkOutDate": "2025-06-07T10:00:00",
  "services": [
    {
      "serviceId": 1,
      "quantity": 2
    }
  ],
  "notes": "Kérjük tengerre néző szobát."
}
```
**Header:** `Authorization: Bearer <access_token>`

#### 🛏️ `ReservationAdminRequest`
**POST** `api/admin/addreservation`
```json
{
  "userId": 2,
  "roomNumber": "B203",
  "capacity": 2,
  "checkInDate": "2025-06-01T14:00:00",
  "checkOutDate": "2025-06-07T10:00:00",
  "services": [],
  "notes": "VIP vendég."
}
```

#### ❌ `ReservationCancelRequest`
**POST** `api/reservation/cancel`
```json
{
  "reservationId": 12
}
```

#### 📅 `ReservationTimesRequest`
**POST** `api/reservation/getrooms`
```json
{
  "start": "2025-06-01T00:00:00",
  "end": "2025-06-05T00:00:00"
}
```

#### 📦 `ReservationServicesRequest`
**POST** `api/service/services`
```json
{
  "reservationId": 3
}
```

#### 👤 `UserReservationsRequest`
**POST** `api/admin/getreservations`
```json
{
  "username": "john_doe"
}
```

#### 🔍 `GetReservationsByUserIdRequest`
```json
{
  "id": 2
}
```

#### ➕ `AddServiceToReservationRequest`
```json
{
  "serviceId": 3,
  "quantity": 1
}
```

---

*Utolsó frissítés: 2025.03.31.*
