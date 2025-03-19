import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import styles from "./ReserveRoom.module.css";

const BASEURL = "https://localhost:7290";

const ReserveRoom = () => {
  const location = useLocation();
  const room = location.state?.room;

  const [checkInDate, setCheckInDate] = useState<string>(new Date().toISOString().split("T")[0]);
  const [checkOutDate, setCheckOutDate] = useState<string>(new Date().toISOString().split("T")[0]);
  const [notes, setNotes] = useState<string>("");
  const [services, setServices] = useState<{ serviceId: number; quantity: number }[]>([]);
  const [serviceOptions, setServiceOptions] = useState<any[]>([]);
  const [guests, setGuests] = useState<number>(1);

  useEffect(() => {
    const fetchServices = async () => {
      try {
        const response = await fetch(`${BASEURL}/api/service/services`);
        const data = await response.json();
        if (data.services) {
          setServiceOptions(data.services);
        }
      } catch (error) {
        console.error("Failed to fetch services:", error);
      }
    };
    fetchServices();
  }, []);

  const calculateTotalPrice = () => {
    const checkIn = new Date(checkInDate);
    const checkOut = new Date(checkOutDate);
    const nights = Math.max((checkOut.getTime() - checkIn.getTime()) / (1000 * 60 * 60 * 24), 1);
    const roomPrice = room?.pricePerNight * nights;
    const servicePrice = services.reduce((total, service) => {
      const option = serviceOptions.find((s) => s.id === service.serviceId);
      return total + (option ? option.price * service.quantity * guests : 0);
    }, 0);
    return roomPrice + servicePrice;
  };

  const handleServiceChange = (serviceId: number, quantity: number) => {
    if (quantity > room.capacity) {
      alert(`Maximum capacity for services is ${room.capacity}`);
      return;
    }

    setServices((prev) => {
      const existingService = prev.find((s) => s.serviceId === serviceId);
      if (existingService) {
        return prev.map((s) => (s.serviceId === serviceId ? { ...s, quantity: Math.max(0, quantity) } : s));
      } else {
        return [...prev, { serviceId, quantity: Math.max(0, quantity) }];
      }
    });
  };

  const handleReservation = async () => {
    if (!room || !checkInDate || !checkOutDate) return;

    const reservationData = {
      roomNumber: room.roomNumber,
      checkInDate,
      checkOutDate,
      services,
      notes,
      guests,
    };

    try {
      const token = localStorage.getItem("accessToken");
      if (!token) {
        alert("You must be logged in to make a reservation.");
        return;
      }

      const response = await fetch(`${BASEURL}/api/reservation/reserve`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(reservationData),
      });

      const data = await response.json();
      if (data.success) {
        alert("Room reserved successfully!");
      } else {
        alert(data.message);
      }
    } catch (error) {
      console.error(error);
      alert("Error making reservation.");
    }
  };

  return (
    <div className={styles.reservePage}>
      <div className={styles.roomInfo}>
        <div className={styles.infoBorder}>
          <h1>Reserve Room</h1>
          <div className={styles.roomDetails}>
            <h3>Room #{room?.roomNumber}</h3>
            <p>{room?.description}</p>
            <p>Price: {room?.pricePerNight} HUF per night</p>
            <p>Capacity: {room?.capacity} guests</p>
          </div>

          <div className={styles.datePicker}>
            <label>Check-in Date</label>
            <input type="date" value={checkInDate} onChange={(e) => setCheckInDate(e.target.value)} />
            <label>Check-out Date</label>
            <input type="date" value={checkOutDate} onChange={(e) => setCheckOutDate(e.target.value)} />
            <label>Number of Guests</label>
            <input type="number" min="1" max={room?.capacity} value={guests} onChange={(e) => setGuests(parseInt(e.target.value) || 1)} />
          </div>

          <div className={styles.services}>
            <h3>Services (per person)</h3>
            <div className={styles.serviceGrid}>
              {serviceOptions.map((service) => (
                <div key={service.id} className={styles.serviceItem}>
                  <label>{service.name} ({service.price} HUF)</label>
                  <input
                    type="number"
                    min="0"
                    max={room?.capacity}
                    value={services.find(s => s.serviceId === service.id)?.quantity || 0}
                    onChange={(e) => handleServiceChange(service.id, parseInt(e.target.value) || 0)}
                  />
                </div>
              ))}
            </div>
          </div>

          <div className={styles.notes}>
            <label>Notes</label>
            <textarea value={notes} onChange={(e) => setNotes(e.target.value)} />
          </div>

          <div className={styles.totalPrice}>
            <h3>Total Price: {calculateTotalPrice()} HUF</h3>
          </div>

          <button className={styles.reserveBtn} onClick={handleReservation}>Reserve</button>
        </div>
      </div>
    </div>
  );
};

export default ReserveRoom;
