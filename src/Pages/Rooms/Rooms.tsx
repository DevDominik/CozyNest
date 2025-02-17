import { useEffect, useState } from "react";
import styles from "./Rooms.module.css";

// Define Room Type and Status
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
    status: string;
};

// Helper function to map the database values to human-readable types
const mapRoomType = (type: number): string => {
    switch (type) {
        case 1: return RoomType.NORMAL;
        case 2: return RoomType.DELUXE;
        case 3: return RoomType.SUITE;
        default: return "Unknown";
    }
};

// Helper function to map the status values to human-readable statuses
const mapAvailabilityStatus = (status: number): string => {
    switch (status) {
        case 1: return RoomType.AVAILABLE;
        case 2: return RoomType.MAINTENANCE;
        case 3: return RoomType.OCCUPIED;
        default: return "Unknown";
    }
};

export const Rooms = () => {
    const [rooms, setRooms] = useState<Room[]>([]);
    const [filteredRooms, setFilteredRooms] = useState<Room[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Track filter states
    const [roomTypeFilter, setRoomTypeFilter] = useState<string>("");
    const [availabilityFilter, setAvailabilityFilter] = useState<string>("");
    const [searchText, setSearchText] = useState<string>("");
    const [priceRange, setPriceRange] = useState<number>(50); // 50 is the default value for the slider

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
                    // Map type and status values to human-readable values and filter out deleted rooms
                    const mappedRooms = data.rooms
                        .filter((room: any) => !room.deleted) // Exclude deleted rooms
                        .map((room: any) => ({
                            ...room,
                            type: mapRoomType(room.type),
                            status: mapAvailabilityStatus(room.status)
                        }));
                    setRooms(mappedRooms);
                }
            } catch (err) {
                setError("Failed to fetch rooms.");
            } finally {
                setLoading(false);
            }
        };

        fetchRooms();
    }, []);

    useEffect(() => {
        const applyFilters = () => {
            let filtered = rooms;
    
            // Filter by room type
            if (roomTypeFilter) {
                filtered = filtered.filter(room => room.type === roomTypeFilter);
            }
    
            // Filter by availability
            if (availabilityFilter === "Show Available") {
                filtered = filtered.filter(room => room.status === RoomType.AVAILABLE);
            } else if (availabilityFilter === "Show Unavailable") {
                filtered = filtered.filter(room => room.status !== RoomType.AVAILABLE);
            }
    
            // Filter by search text (room number or description)
            if (searchText) {
                filtered = filtered.filter(
                    room =>
                        room.roomNumber.toLowerCase().includes(searchText.toLowerCase()) ||
                        room.description.toLowerCase().includes(searchText.toLowerCase())
                );
            }
    
            // Filter by price range
            filtered = filtered.filter(room => room.pricePerNight <= priceRange);
    
            setFilteredRooms(filtered);
        };
    
        applyFilters();
    }, [roomTypeFilter, availabilityFilter, searchText, priceRange, rooms]);
    
    return (
        <div className={styles.roomPage}>
            <div className={styles.Container}>
                <h1>AVAILABLE ROOMS</h1>
                <div className={styles.SearchDiv}>
                    <select
                        name="roomType"
                        id="roomType"
                        className={styles.Select}
                        onChange={(e) => setRoomTypeFilter(e.target.value)}
                    >
                        <option value="">All</option>
                        <option value={RoomType.NORMAL}>{RoomType.NORMAL}</option>
                        <option value={RoomType.DELUXE}>{RoomType.DELUXE}</option>
                        <option value={RoomType.SUITE}>{RoomType.SUITE}</option>
                    </select>

                    <select
                        name="roomAvailability"
                        id="roomAvailability"
                        className={styles.Select}
                        onChange={(e) => setAvailabilityFilter(e.target.value)}
                    >
                        <option value="">Show All</option>
                        <option value="Show Available">Show Available</option>
                        <option value="Show Unavailable">Show Unavailable</option>
                        <option value="Show Both">Show Both</option>
                    </select>

                    <input
                        type="text"
                        className={styles.SearchBar}
                        placeholder="Search..."
                        onChange={(e) => setSearchText(e.target.value)}
                    />

                    <input
                        type="range"
                        className={styles.Slider}
                        min="0"
                        max="200"
                        defaultValue="200"
                        onChange={(e) => setPriceRange(Number(e.target.value))}
                    />
                </div>

                {loading ? (
                    <p>Loading rooms...</p>
                ) : error ? (
                    <p className={styles.Error}>{error}</p>
                ) : (
                    <div className={styles.Wrapper}>
                        {filteredRooms.map(room => (
                            <div key={room.id} className={styles.Card}>
                                <h2 className={styles.RoomName}>ROOM #{room.roomNumber}</h2>
                                <div className={styles.RoomInfo}>
                                    <p>Type: {room.type}</p>
                                    <p>Price: {room.pricePerNight} HUF / Night</p>
                                    <p>{room.description}</p>
                                    <p>Availability: {room.status}</p>
                                </div>
                                <button
                                    className={styles.RoomBtn}
                                    disabled={room.status !== RoomType.AVAILABLE}
                                >
                                    {room.status === RoomType.AVAILABLE ? "Foglalás" : "Not Available"}
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};
