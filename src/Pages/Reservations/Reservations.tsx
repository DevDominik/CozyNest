import React, { useEffect, useState } from 'react';
import styles from './Reservations.module.css';

// Define the Reservation type with additional fields
type Reservation = {
  id: number;
  roomId: number;
  roomDescription: string;
  roomNumber: string;
  roomType: string;
  checkInDate: string;
  checkOutDate: string;
  status: number;
  notes?: string;
};

// Define the shape of the API response
type ReservationsResponse = {
  message: string;
  reservations: Reservation[];
};

const Reservations = () => {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchReservations = async () => {
      console.log('Fetching reservations...');
      try {
        const token = localStorage.getItem('accessToken');
        console.log('Token retrieved:', token);

        if (!token) {
          throw new Error('No access token found');
        }

        const response = await fetch('https://localhost:7290/api/reservation/getreservations', {
          method: 'GET',
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });

        console.log('Response status:', response.status);

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data: ReservationsResponse = await response.json();
        setReservations(data.reservations || []);
      } catch (err) {
        console.error('Error fetching reservations:', err);
        setError((err as Error).message);
      } finally {
        setLoading(false);
        console.log('Loading state set to false');
      }
    };

    fetchReservations();
  }, []);

  if (loading) {
    console.log('Loading...');
    return <p className={styles.loading}>Loading reservations...</p>;
  }
  if (error) {
    console.error('Error:', error);
    return <p className={styles.error}>Error: {error}</p>;
  }

  console.log('Rendering reservations:', reservations);

  return (
    <div className={styles.container}>
      <h2 className={styles.heading}>Your Reservations</h2>
      {reservations.length === 0 ? (
        <p className={styles.noReservations}>No reservations found.</p>
      ) : (
        <div className={styles.cardWrapper}>
          {reservations.map((reservation) => (
            <div key={reservation.id} className={styles.card}>
                <div className={`${styles.cardImage} ${reservation.roomType == "Standard" ? styles.cardImageBasic : reservation.roomType == "Deluxe" ? styles.cardImageDeluxe : styles.cardImageSuite}`}></div>
              <div className={styles.cardHeader}>
                <span className={styles.roomId}>Room Number</span>
                <span className={styles.roomNumber}>{reservation.roomNumber}</span>
              </div>
              <div className={styles.cardBody}>
                <div className={styles.roomDescription}>
                  <span>Room Description: {reservation.roomDescription}</span>
                </div>
                <div className={styles.dates}>
                  <span>Check-In: {new Date(reservation.checkInDate).toLocaleString()}</span>
                  <span>Check-Out: {new Date(reservation.checkOutDate).toLocaleString()}</span>
                </div>
                <div className={styles.status}>Status: {reservation.status}</div>
                {reservation.notes && <p className={styles.notes}>Notes: {reservation.notes}</p>}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Reservations;
