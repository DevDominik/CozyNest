import React, { useEffect, useState } from 'react';
import styles from './Reservations.module.css';

// Define the Reservation type
type Reservation = {
  id: number;
  roomId: number;
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
        console.log('Reservations data:', data);

        // Optionally log the success message
        console.log('Message from server:', data.message);

        // Set the reservations data
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
    console.log('Loading...'); // Debugging loading state
    return <p className={styles.loading}>Loading reservations...</p>;
  }
  if (error) {
    console.error('Error:', error); // Debugging error state
    return <p className={styles.error}>Error: {error}</p>;
  }

  console.log('Rendering reservations:', reservations);

  return (
    <div className={styles.container}>
      <h2 className={styles.heading}>Your Reservations</h2>
      {reservations.length === 0 ? (
        <p className={styles.noReservations}>No reservations found.</p>
      ) : (
        <ul className={styles.reservationList}>
          {reservations.map((reservation, index) => (
            <li key={reservation.id} className={styles.reservationItem}>
              <span className={styles.roomId}>Room ID: {reservation.roomId}</span> | 
              <span className={styles.dates}>Check-In: {new Date(reservation.checkInDate).toLocaleString()}</span> | 
              <span className={styles.dates}>Check-Out: {new Date(reservation.checkOutDate).toLocaleString()}</span> | 
              <span className={styles.status}>Status: {reservation.status}</span>
              {reservation.notes && <p className={styles.notes}>Notes: {reservation.notes}</p>}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default Reservations;
