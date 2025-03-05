-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 05. 08:43
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
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `reservations`
--

INSERT INTO `reservations` (`id`, `guest_id`, `room_id`, `check_in_date`, `check_out_date`, `status`, `notes`) VALUES
(1, 1, 101, '2025-04-05 08:42:35', '2025-04-08 08:42:35', 2, 'Business trip'),
(2, 1, 201, '2025-06-05 08:42:35', '2025-06-10 08:42:35', 2, 'Family vacation'),
(3, 2, 102, '2025-09-05 08:42:35', '2025-09-07 08:42:35', 1, 'Weekend getaway'),
(4, 2, 301, '2025-12-05 08:42:35', '2025-12-12 08:42:35', 2, 'Anniversary trip'),
(5, 3, 202, '2026-03-05 08:42:35', '2026-03-09 08:42:35', 1, 'Honeymoon'),
(6, 3, 302, '2026-09-05 08:42:35', '2026-09-11 08:42:35', 3, 'Cancelled reservation'),
(7, 1, 201, '2026-11-05 08:42:35', '2026-11-10 08:42:35', 2, 'Work conference'),
(8, 2, 101, '2027-01-05 08:42:35', '2027-01-08 08:42:35', 2, 'Short trip'),
(9, 3, 301, '2027-03-05 08:42:35', '2027-03-12 08:42:35', 2, 'Luxury vacation');

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
(1, 1, 1, 3),
(2, 1, 4, 1),
(3, 2, 2, 2),
(4, 3, 3, 1),
(5, 4, 5, 2),
(6, 5, 1, 4),
(7, 6, 2, 1),
(8, 7, 3, 1),
(9, 8, 4, 2),
(10, 9, 5, 3);

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
  `deleted` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `room`
--

INSERT INTO `room` (`id`, `room_number`, `type`, `price_per_night`, `status`, `description`, `deleted`) VALUES
(1, '101', 1, 100.00, 1, 'Standard Room with a single bed', 0),
(2, '102', 1, 110.00, 1, 'Standard Room with a queen bed', 0),
(3, '201', 2, 150.00, 1, 'Deluxe Room with ocean view', 0),
(4, '202', 2, 160.00, 1, 'Deluxe Room with king bed', 0),
(5, '301', 3, 250.00, 1, 'Suite with jacuzzi', 0),
(6, '302', 3, 260.00, 1, 'Suite with balcony and city view', 0);

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
(1, 'Breakfast', 'Buffet breakfast with various choices', 15.00, 1),
(2, 'Spa Access', 'Access to the hotel spa and sauna', 50.00, 1),
(3, 'Airport Pickup', 'Pickup service from the airport', 30.00, 1),
(4, 'Room Service', '24/7 in-room dining service', 20.00, 1),
(5, 'Gym Access', 'Access to the gym facilities', 10.00, 1);

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
(1, 'admin', 'bIk2uLKRCVByfck9ZgcqmA==$dntJjU4VovPdeZLMfvMKPF63DCLiqWTk2wepKnmew+I=', 'admin@admin.admin', '', '', '', '2025-03-05 07:38:52', 0, 1),
(2, 'rec', '6+gKnWNIw2uiInsg7OtHmQ==$ooSFSJuS4NxlmBT3h3pUhUmeEPbLvgEwnZwcR7TyF6U=', 'rec@rec.rec', '', '', '', '2025-03-05 07:39:02', 0, 1),
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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT a táblához `reservationservices`
--
ALTER TABLE `reservationservices`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `tokens`
--
ALTER TABLE `tokens`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

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
