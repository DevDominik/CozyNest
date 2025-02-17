import { useEffect, useState } from "react";
import styles from "./Rooms.module.css";

// Define Room Type
const RoomType = {
    NORMAL: "Normal",
    DELUXE: "Deluxe",
    SUITE: "Suite",
    MAINTENANCE: "Maintenance",
    OCCUPIED: "Occupied",
    AVAILABLE: "Available"
};

// Define Room Data Type
type Room = {
    id: number;
    roomNumber: string;
    type: string;
    pricePerNight: number;
    description: string;
    deleted: boolean;
};

export const Rooms = () => {
    const [rooms, setRooms] = useState<Room[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchRooms = async () => {
            try {
                const response = await fetch("https://localhost:7290/api/room/list", {
                    headers: {
                        "Content-Type": "application/json"
                    }
                });
                const data = await response.json();
                if (data.rooms) {
                    setRooms(data.rooms.filter((room: Room) => !room.deleted)); // Exclude deleted rooms
                }
            } catch (err) {
                setError("Failed to fetch rooms.");
            } finally {
                setLoading(false);
            }
        };

        fetchRooms();
    }, []);

    return (
        <div className={styles.roomPage}>
            <div className={styles.Container}>
                <h1>AVAILABLE ROOMS</h1>
                <div className={styles.SearchDiv}>
                    <select name="roomType" id="roomType" className={styles.Select}>
                        <option value="">All</option>
                        <option value={RoomType.NORMAL}>{RoomType.NORMAL}</option>
                        <option value={RoomType.DELUXE}>{RoomType.DELUXE}</option>
                        <option value={RoomType.SUITE}>{RoomType.SUITE}</option>
                    </select>
                    <select name="" id="roomAvailability" className={styles.Select}>
                        <option value="">Show Available</option>
                        <option value="">Show Unavailable</option>
                        <option value="">Show Both</option>
                    </select>
                    <input type="text" className={styles.SearchBar} placeholder="Search..." />
                    <input type="range" className={styles.Slider} min="0" max="100" defaultValue="50" />
                </div>
                
                {loading ? (
                    <p>Loading rooms...</p>
                ) : error ? (
                    <p className={styles.Error}>{error}</p>
                ) : (
                    <div className={styles.Wrapper}>
                        {rooms.map(room => (
                            <div key={room.id} className={styles.Card}>
                                <h2 className={styles.RoomName}>ROOM #{room.roomNumber}</h2>
                                <div className={styles.RoomInfo}>
                                    <p>Type: {room.type}</p>
                                    <p>Price: {room.pricePerNight} HUF / Night</p>
                                    <p>{room.description}</p>
                                    <p>Availability: {room.type === RoomType.AVAILABLE ? "Yes" : "No"}</p>
                                </div>
                                <button className={styles.RoomBtn} disabled={room.type !== RoomType.AVAILABLE}>
                                    {room.type === RoomType.AVAILABLE ? "Foglalás" : "Not Available"}
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};