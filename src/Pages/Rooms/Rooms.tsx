import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"; // Import useNavigate from react-router-dom
import styles from "./Rooms.module.css";

// Define Room Type and Status
const RoomType = {
  NORMAL: "Standard", // Fix to match API response
  DELUXE: "Deluxe",
  SUITE: "Suite",
  MAINTENANCE: "Maintenance",
  OCCUPIED: "Occupied",
  AVAILABLE: "Available",
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

export const Rooms = () => {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [filteredRooms, setFilteredRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Track filter states
  const [roomTypeFilter, setRoomTypeFilter] = useState<string>("");
  const [availabilityFilter, setAvailabilityFilter] = useState<string>("");
  const [searchText, setSearchText] = useState<string>("");
  const [priceRange, setPriceRange] = useState<number>(50); // Default value for the slider

  // Date pickers
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");

  const navigate = useNavigate(); // Create navigate function from useNavigate hook

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch("https://localhost:7290/api/reservation/getrooms", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            start: startDate,
            end: endDate,
          }),
        });
        const data = await response.json();
        if (data.rooms) {
          // Ensure deleted rooms are excluded
          const mappedRooms = data.rooms.filter((room: any) => !room.deleted);
          setRooms(mappedRooms);
        }
      } catch (err) {
        setError("Failed to fetch rooms.");
      } finally {
        setLoading(false);
      }
    };

    fetchRooms();
  }, [startDate, endDate]); // Fetch rooms when dates change

  useEffect(() => {
    const applyFilters = () => {
      let filtered = rooms;

      // Filter by room type
      if (roomTypeFilter) {
        filtered = filtered.filter((room) => room.type === roomTypeFilter);
      }

      // Filter by availability
      if (availabilityFilter === "Show Available") {
        filtered = filtered.filter((room) => room.status === RoomType.AVAILABLE);
      } else if (availabilityFilter === "Show Unavailable") {
        filtered = filtered.filter((room) => room.status !== RoomType.AVAILABLE);
      }

      // Filter by search text (room number or description)
      if (searchText) {
        filtered = filtered.filter(
          (room) =>
            room.roomNumber.toLowerCase().includes(searchText.toLowerCase()) ||
            room.description.toLowerCase().includes(searchText.toLowerCase())
        );
      }

      // Filter by price range
      filtered = filtered.filter((room) => room.pricePerNight <= priceRange);

      setFilteredRooms(filtered);
    };

    applyFilters();
  }, [roomTypeFilter, availabilityFilter, searchText, priceRange, rooms, startDate, endDate]);

  const handleReservationClick = (room: Room) => {
    // Navigate to the ReserveRoom page and pass the room data as state
    navigate("/reserve-room", { state: { room } });
  };

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
          <p>MAX: {priceRange}HUF</p>

          {/* Date Pickers */}
          <div className={styles.DatePickers}>
            <label htmlFor="startDate">Start Date:</label>
            <input
              type="date"
              id="startDate"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
            />

            <label htmlFor="endDate">End Date:</label>
            <input
              type="date"
              id="endDate"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
            />
          </div>
        </div>

        {loading ? (
          <p>Loading rooms...</p>
        ) : error ? (
          <p className={styles.Error}>{error}</p>
        ) : (
          <div className={styles.Wrapper}>
            {filteredRooms.map((room) => (
              <div key={room.id} className={styles.Card}>
                <div
                  className={`${styles.cardImage} ${
                    room.type === "Standard"
                      ? styles.cardImageBasic
                      : room.type === "Deluxe"
                      ? styles.cardImageDeluxe
                      : styles.cardImageSuite
                  }`}
                ></div>
                <div className={styles.CardPadding}>
                  <div className={styles.roomHeader}>
                    <h3 className={styles.RoomName}>#{room.roomNumber}</h3>
                    <h3>{room.type}</h3>
                  </div>
                  <div className={styles.RoomInfo}>
                    <p>{room.description}</p>
                    <p>{room.pricePerNight} HUF</p>
                    <p>Availability: {room.status}</p>
                  </div>
                  <button
                    className={styles.btn}
                    disabled={room.status !== RoomType.AVAILABLE}
                    onClick={() => handleReservationClick(room)}
                  >
                    {room.status === RoomType.AVAILABLE
                      ? "Foglalás"
                      : "Not Available"}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
