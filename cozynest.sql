-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 20. 13:00
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `cozynest`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservations`
--

CREATE TABLE `reservations` (
  `id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `check_in_date` datetime NOT NULL,
  `check_out_date` datetime NOT NULL,
  `status` int(11) NOT NULL,
  `notes` text DEFAULT NULL,
  `capacity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `reservations`
--

INSERT INTO `reservations` (`id`, `guest_id`, `room_id`, `check_in_date`, `check_out_date`, `status`, `notes`, `capacity`) VALUES
(1, 1, 1, '2025-03-10 14:00:00', '2025-03-12 11:00:00', 3, 'Need extra towels', 0),
(2, 1, 2, '2025-03-15 14:00:00', '2025-03-16 11:00:00', 1, 'Celebrating anniversary', 0),
(3, 1, 3, '2025-03-20 14:00:00', '2025-03-23 11:00:00', 2, 'Business trip', 0),
(4, 1, 4, '2025-03-25 14:00:00', '2025-03-26 11:00:00', 1, 'Conference stay', 0),
(5, 1, 5, '2025-03-30 14:00:00', '2025-04-02 11:00:00', 3, 'Family vacation', 0),
(6, 2, 1, '2025-03-11 14:00:00', '2025-03-13 11:00:00', 1, 'Regular business stay', 0),
(7, 2, 2, '2025-03-14 14:00:00', '2025-03-17 11:00:00', 2, 'Holiday with family', 0),
(8, 2, 3, '2025-03-19 14:00:00', '2025-03-22 11:00:00', 1, 'Long-term stay for work', 0),
(9, 2, 4, '2025-03-24 14:00:00', '2025-03-27 11:00:00', 1, 'Weekend getaway', 0),
(10, 2, 5, '2025-03-28 14:00:00', '2025-03-31 11:00:00', 3, 'Friends reunion', 0),
(11, 3, 1, '2025-03-12 14:00:00', '2025-03-14 11:00:00', 1, 'Short stay for business', 0),
(12, 3, 2, '2025-03-16 14:00:00', '2025-03-17 11:00:00', 1, 'Weekend retreat', 0),
(13, 3, 3, '2025-03-21 14:00:00', '2025-03-24 11:00:00', 2, 'Romantic getaway', 0),
(14, 3, 4, '2025-03-26 14:00:00', '2025-03-27 11:00:00', 1, 'Stay for meeting', 0),
(15, 3, 5, '2025-04-01 14:00:00', '2025-04-03 11:00:00', 3, 'Relaxation and spa visit', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservationservices`
--

CREATE TABLE `reservationservices` (
  `id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `service_id` int(11) NOT NULL,
  `quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `reservationservices`
--

INSERT INTO `reservationservices` (`id`, `reservation_id`, `service_id`, `quantity`) VALUES
(1, 1, 1, 2),
(2, 2, 2, 1),
(3, 3, 3, 1),
(4, 4, 4, 1),
(5, 5, 5, 2),
(6, 6, 1, 1),
(7, 7, 2, 1),
(8, 8, 3, 2),
(9, 9, 4, 1),
(10, 10, 5, 3),
(11, 11, 1, 1),
(12, 12, 2, 2),
(13, 13, 3, 1),
(14, 14, 4, 1),
(15, 15, 5, 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservationstatuses`
--

CREATE TABLE `reservationstatuses` (
  `id` int(11) NOT NULL,
  `description` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `reservationstatuses`
--

INSERT INTO `reservationstatuses` (`id`, `description`) VALUES
(1, 'Incomplete'),
(2, 'Complete'),
(3, 'Cancelled');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `roles`
--

INSERT INTO `roles` (`id`, `name`) VALUES
(1, 'Guest'),
(2, 'Receptionist'),
(3, 'Manager');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `room`
--

CREATE TABLE `room` (
  `id` int(11) NOT NULL,
  `room_number` varchar(50) NOT NULL,
  `type` int(11) NOT NULL,
  `price_per_night` decimal(10,2) NOT NULL,
  `status` int(11) NOT NULL,
  `description` text DEFAULT NULL,
  `deleted` tinyint(1) DEFAULT 0,
  `capacity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `room`
--

INSERT INTO `room` (`id`, `room_number`, `type`, `price_per_night`, `status`, `description`, `deleted`, `capacity`) VALUES
(1, '101', 1, 8000.00, 1, 'Standard room with a nice view', 0, 1),
(2, '102', 2, 14000.00, 1, 'Deluxe room with king-sized bed', 0, 2),
(3, '103', 3, 22000.00, 1, 'Suite with living room and kitchenette', 0, 4),
(4, '104', 1, 8000.00, 1, 'Standard room with a desk and chair', 0, 1),
(5, '105', 2, 14000.00, 1, 'Deluxe room with balcony', 0, 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roomstatus`
--

CREATE TABLE `roomstatus` (
  `id` int(11) NOT NULL,
  `description` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `roomstatus`
--

INSERT INTO `roomstatus` (`id`, `description`) VALUES
(1, 'Available'),
(2, 'Maintenance'),
(3, 'Occupied');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roomtype`
--

CREATE TABLE `roomtype` (
  `id` int(11) NOT NULL,
  `description` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `roomtype`
--

INSERT INTO `roomtype` (`id`, `description`) VALUES
(1, 'Standard'),
(2, 'Deluxe'),
(3, 'Suite');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `services`
--

CREATE TABLE `services` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `price` decimal(10,2) NOT NULL,
  `is_active` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `services`
--

INSERT INTO `services` (`id`, `name`, `description`, `price`, `is_active`) VALUES
(1, 'Prémium reggeli csomag', ' Extra választék (pl. frissen facsart gyümölcslé, minőségi kávé, helyi bio termékek', 5000.00, 1),
(2, 'All-inclusive italcsomag', ' Szauna, gőzfürdő, jacuzzi, sókamra, egyéb relaxációs lehetőségek.Korlátlan alkoholmentes italok, kávék, teák vagy akár bor és koktélcsomag.', 10000.00, 1),
(3, 'Wellness és spa belépő', 'Teljes hozzáférés a hotel wellness részlegéhez', 8000.00, 1),
(4, 'Edzőterem és sportlehetőségek', 'Access to the hotel’s gym facilitiesKülönleges felszerelések, személyi edző, vagy csoportos órák (pl. jóga, pilates)', 3000.00, 1),
(5, 'VIP szoba takarítás', 'Napi kétszeri takarítás, extra törölközők, illatosító szerviz, prémium kozmetikumok.', 3000.00, 1),
(6, 'Animációs programok / Gyermekmegőrzés', 'Napközbeni szórakoztató programok gyerekeknek, esti mesélés, kézműves foglalkozások.', 3500.00, 0),
(7, 'Biciklikölcsönzés vagy elektromos roller', 'Napi használatra fenntartott járművek városi felfedezésekhez.', 4500.00, 1),
(8, 'Privát strandterület vagy napágy foglalás', 'Access to the hotel’s gym facilitiesKülönleges felszerelések, személyi edző, vagy csoportos órák (pl. jóga, pilates)', 7000.00, 1),
(9, 'Gasztroélmény csomag', 'Napi egy gourmet vacsora vagy helyi specialitások kóstolója.', 18000.00, 1),
(10, 'Korlátlan minibár', 'Naponta feltöltött, ingyenesen fogyasztható minibár prémium termékekkel.', 5000.00, 0),
(11, 'Teljes panziós ellátás', 'Reggeli, ebéd és vacsora.', 12000.00, 1);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tokens`
--

CREATE TABLE `tokens` (
  `id` bigint(20) NOT NULL,
  `access_token` varchar(255) NOT NULL,
  `refresh_token` varchar(255) NOT NULL,
  `access_expiry` datetime NOT NULL,
  `refresh_expiry` datetime NOT NULL,
  `user_id` int(11) NOT NULL,
  `is_active` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `tokens`
--

INSERT INTO `tokens` (`id`, `access_token`, `refresh_token`, `access_expiry`, `refresh_expiry`, `user_id`, `is_active`) VALUES
(1, 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImViMzJiZjg1LWFjYTctNDcyNS1iYTM4LWNhYzUyMjI4ZWJhMSIsInVzZXJJZCI6MSwiZXhwIjoxNzQyNDU4MDQzfQ.4OlwANWPADYeIKnBQP1rDJdwOJV5OZwsMbxBzzIuBXY', 'gg8SkCh//zPLEIDlZcy3vfeCGtfCkUQiIdPNzYf+CCw=', '2025-03-20 08:07:23', '2025-03-27 07:37:23', 1, 1),
(2, 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjAyNTUwYzIzLTRiOTctNDcyNC04NGZkLTRkNDkwOGMyZDExNyIsInVzZXJJZCI6MSwiZXhwIjoxNzQyNDY1MjUwfQ.3MAix-rrHI6L3O9iYKE8LIQMxNCX3xtd4w1IFom_GX8', 'zJYkWCKPVpf7IY1O0Z5AiUDp2x1v7B6b6Rt+YxmnIIA=', '2025-03-20 10:07:30', '2025-03-27 09:37:30', 1, 0),
(3, 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImNhMGVmMGVjLTVmNWYtNGU2MC1iNTE0LTAwMjY4M2EzZjFkYSIsInVzZXJJZCI6MSwiZXhwIjoxNzQyNDY1NjA4fQ.rOLSk5Gu1NPMkwrVQHQcoyuv2EzLI-VbtCASk3m3v2o', 'MO54CSoD8g9Dar/tF5YE7ry/25UQkMQL7b8ajcJhHwU=', '2025-03-20 10:13:28', '2025-03-27 09:43:28', 1, 0),
(4, 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjMzYjhlNzIwLTg0ZmYtNDlkYy1iMzRjLTMyOWQ1MGM3M2JmNiIsInVzZXJJZCI6MSwiZXhwIjoxNzQyNDY1Njc3fQ.Y0pe2iLNOeW7ae6K3bA8LmvJzITUKgOlysZCcTas2VA', 'wGDBYzmw1GkbRYAWp2jG28qUKo1KUYqEVRoEEg0bbpY=', '2025-03-20 10:14:37', '2025-03-27 09:44:37', 1, 1),
(5, 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjVkN2M3YWUyLTRmNTktNDM2ZS04MjMzLTA4OTRlY2Q4MDI2ZSIsInVzZXJJZCI6MSwiZXhwIjoxNzQyNDcyMTg0fQ.RFWLS5Sw1LmtIBIKjLPBUESZrqUogYF2bgWbmec1wpo', '7jf2Kk940ZXxj6uxF6Ig1lasmuzoL/tuUOqkjZQAMQo=', '2025-03-20 12:03:04', '2025-03-27 11:33:04', 1, 1);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `username` varchar(255) NOT NULL,
  `hashed_password` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `first_name` varchar(255) NOT NULL,
  `last_name` varchar(255) NOT NULL,
  `address` text NOT NULL,
  `join_date` datetime NOT NULL,
  `closed` tinyint(1) DEFAULT 0,
  `role_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`id`, `username`, `hashed_password`, `email`, `first_name`, `last_name`, `address`, `join_date`, `closed`, `role_id`) VALUES
(1, 'admin', 'bIk2uLKRCVByfck9ZgcqmA==$dntJjU4VovPdeZLMfvMKPF63DCLiqWTk2wepKnmew+I=', 'admin@admin.admin', 'a', '', '', '2025-03-05 07:38:52', 0, 3),
(2, 'rec', '6+gKnWNIw2uiInsg7OtHmQ==$ooSFSJuS4NxlmBT3h3pUhUmeEPbLvgEwnZwcR7TyF6U=', 'rec@rec.rec', '', '', '', '2025-03-05 07:39:02', 0, 2),
(3, 'user', 'nWOUC17uEydlAb1oeNt8Sw==$xYl0RoM7OnEmevfsmb6gP2ieU8ByHvpK1LnMAdq8Z2o=', 'user@user.user', '', '', '', '2025-03-05 07:39:17', 0, 1);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `reservations`
--
ALTER TABLE `reservations`
  ADD PRIMARY KEY (`id`),
  ADD KEY `status` (`status`);

--
-- A tábla indexei `reservationservices`
--
ALTER TABLE `reservationservices`
  ADD PRIMARY KEY (`id`),
  ADD KEY `reservation_id` (`reservation_id`),
  ADD KEY `service_id` (`service_id`);

--
-- A tábla indexei `reservationstatuses`
--
ALTER TABLE `reservationstatuses`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `room`
--
ALTER TABLE `room`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `room_number` (`room_number`),
  ADD KEY `type` (`type`),
  ADD KEY `status` (`status`);

--
-- A tábla indexei `roomstatus`
--
ALTER TABLE `roomstatus`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `roomtype`
--
ALTER TABLE `roomtype`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `tokens`
--
ALTER TABLE `tokens`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD KEY `role_id` (`role_id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `reservations`
--
ALTER TABLE `reservations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `reservationservices`
--
ALTER TABLE `reservationservices`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `reservationstatuses`
--
ALTER TABLE `reservationstatuses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `room`
--
ALTER TABLE `room`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `roomstatus`
--
ALTER TABLE `roomstatus`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `roomtype`
--
ALTER TABLE `roomtype`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `services`
--
ALTER TABLE `services`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT a táblához `tokens`
--
ALTER TABLE `tokens`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `reservations`
--
ALTER TABLE `reservations`
  ADD CONSTRAINT `reservations_ibfk_1` FOREIGN KEY (`status`) REFERENCES `reservationstatuses` (`id`);

--
-- Megkötések a táblához `reservationservices`
--
ALTER TABLE `reservationservices`
  ADD CONSTRAINT `reservationservices_ibfk_1` FOREIGN KEY (`reservation_id`) REFERENCES `reservations` (`id`),
  ADD CONSTRAINT `reservationservices_ibfk_2` FOREIGN KEY (`service_id`) REFERENCES `services` (`id`);

--
-- Megkötések a táblához `room`
--
ALTER TABLE `room`
  ADD CONSTRAINT `room_ibfk_1` FOREIGN KEY (`type`) REFERENCES `roomtype` (`id`),
  ADD CONSTRAINT `room_ibfk_2` FOREIGN KEY (`status`) REFERENCES `roomstatus` (`id`);

--
-- Megkötések a táblához `tokens`
--
ALTER TABLE `tokens`
  ADD CONSTRAINT `tokens_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

--
-- Megkötések a táblához `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
