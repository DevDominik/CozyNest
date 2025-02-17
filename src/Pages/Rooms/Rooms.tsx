import styles from "./Rooms.module.css"





export const Rooms = () => {
    return (
        <div className={styles.roomPage}>
            <div className={styles.Container}>
                <h1>AVAILABLE ROOMS</h1>
                <div className={styles.SearchDiv}>
                    <select name="roomType" id="roomType" className={styles.Select}>
                        <option value="">Normal</option>
                        <option value="">Deluxe</option>
                        <option value="">Suite</option>
                    </select>

                    <select name="" id="roomAvailability" className={styles.Select}>
                        <option value="">Show Available</option>
                        <option value="">Show Unavailable</option>
                        <option value="">Show Both</option>
                    </select>
                    <input type="text" className={styles.SearchBar} />
                    <input type="range" id="volume" name="volume" min="0" max="11" />

                </div>
                <div className={styles.Wrapper}>
                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>


                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>

                    <div className={styles.Card}>
                        <h2 className={styles.RoomName}>ROOM #1</h2>
                        <div className={styles.RoomInfo}>
                            <p>Price: 3000HUF / Night</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Price:</p>
                            <p>Availability: Yes/No</p>
                        </div>
                        <button className={styles.RoomBtn}>Foglalás</button>
                    </div>
                </div>



            </div>
        </div>
    )
}
