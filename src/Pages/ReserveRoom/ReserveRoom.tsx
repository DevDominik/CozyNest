import { useState } from "react";
import { useLocation } from "react-router-dom";
import styles from "./ReserveRoom.module.css";

const ReserveRoom = () => {
  const location = useLocation();
  const room = location.state?.room;

  const [checkInDate, setCheckInDate] = useState<string>(new Date().toISOString().split("T")[0]); // Default to today's date
  const [checkOutDate, setCheckOutDate] = useState<string>(new Date().toISOString().split("T")[0]); // Default to today's date
  const [notes, setNotes] = useState<string>("");

  const handleReservation = async () => {
    if (!room || !checkInDate || !checkOutDate) return;

    const reservationData = {
      roomNumber: room.roomNumber,
      checkInDate: checkInDate,
      checkOutDate: checkOutDate,
      notes: notes,
    };

    try {
      const token = localStorage.getItem("accessToken"); // Get token from localStorage
      if (!token) {
        alert("You must be logged in to make a reservation.");
        return;
      }

      const response = await fetch("https://localhost:7290/api/reservation/reserve", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,  // Use the token from localStorage
        },
        body: JSON.stringify(reservationData),
      });

      const data = await response.json();
      if (data.success) {
        alert("Room reserved successfully!");
      } else {
        alert("Reservation failed.");
      }
    } catch (error) {
      console.error(error);
      alert("Error making reservation.");
    }
  };

  return (
    <div className={styles.reservePage}>
      <h1>Reserve Room</h1>
      <div className={styles.roomDetails}>
        <h3>Room #{room?.roomNumber}</h3>
        <p>{room?.description}</p>
        <p>Price: {room?.pricePerNight} HUF per night</p>
      </div>

      <div className={styles.datePicker}>
        <label>Check-in Date</label>
        <input
          type="date"
          value={checkInDate}
          onChange={(e) => setCheckInDate(e.target.value)}
        />
        <label>Check-out Date</label>
        <input
          type="date"
          value={checkOutDate}
          onChange={(e) => setCheckOutDate(e.target.value)}
        />
      </div>

      <div className={styles.notes}>
        <label>Notes</label>
        <textarea value={notes} onChange={(e) => setNotes(e.target.value)} />
      </div>

      <button onClick={handleReservation}>Reserve</button>
    </div>
  );
};

export default ReserveRoom;
