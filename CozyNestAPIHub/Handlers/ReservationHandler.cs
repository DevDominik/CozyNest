using CozyNestAPIHub.Models;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Data;

namespace CozyNestAPIHub.Handlers
{
    public class ReservationHandler
    {
        private static string _connectionString;
        private static readonly SemaphoreSlim _reservationReadLock = new(1, 1);
        private static readonly SemaphoreSlim _reservationWriteLock = new(1, 1);
        private static readonly SemaphoreSlim _reservationServiceReadLock = new(1, 1);
        private static readonly SemaphoreSlim _reservationServiceWriteLock = new(1, 1);
        private static readonly SemaphoreSlim _serviceReadLock = new(1, 1);
        private static readonly SemaphoreSlim _serviceWriteLock = new(1, 1);

        private static readonly ConcurrentDictionary<int, Reservation> _reservationCache = new();
        private static readonly ConcurrentDictionary<int, Service> _serviceCache = new();
        private static readonly ConcurrentDictionary<int, ReservationService> _reservationServiceCache = new();

        public static void Initialize(string username, string password)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("ReservationHandler is already initialized.");

            _connectionString = $"Server=localhost;Database=cozynest;User ID={username};Password={password};";
        }

        private static MySqlConnection CreateConnection() => new(_connectionString);

        public static async Task<List<Reservation>> GetReservations()
        {
            await _reservationReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                var reservations = new List<Reservation>();
                var query = "SELECT * FROM reservations";
                using var cmd = new MySqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var reservation = new Reservation
                    {
                        Id = reader.GetInt32("id"),
                        GuestId = reader.GetInt32("guest_id"),
                        RoomId = reader.GetInt32("room_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.GetDateTime("check_out_date"),
                        Status = reader.GetInt32("status"),
                        Notes = reader.GetString("notes")
                    };

                    reservations.Add(reservation);
                    _reservationCache[reservation.Id] = reservation;
                }

                return reservations;
            }
            finally
            {
                _reservationReadLock.Release();
            }
        }

        public static async Task<Reservation?> GetReservationById(int id)
        {
            if (_reservationCache.TryGetValue(id, out var cachedReservation))
            {
                return cachedReservation;
            }

            await _reservationReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT * FROM reservations WHERE id = @id";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var reservation = new Reservation
                    {
                        Id = reader.GetInt32("id"),
                        GuestId = reader.GetInt32("guest_id"),
                        RoomId = reader.GetInt32("room_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.GetDateTime("check_out_date"),
                        Status = reader.GetInt32("status"),
                        Notes = reader.GetString("notes")
                    };

                    _reservationCache[id] = reservation;
                    return reservation;
                }

                return null;
            }
            finally
            {
                _reservationReadLock.Release();
            }
        }

        public static async Task<Reservation?> CreateReservation(Reservation reservation)
        {
            await _reservationWriteLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO reservations (guest_id, room_id, check_in_date, check_out_date, status, notes) 
                              VALUES (@guestId, @roomId, @checkInDate, @checkOutDate, @status, @notes); 
                              SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@guestId", reservation.GuestId);
                cmd.Parameters.AddWithValue("@roomId", reservation.RoomId);
                cmd.Parameters.AddWithValue("@checkInDate", reservation.CheckInDate);
                cmd.Parameters.AddWithValue("@checkOutDate", reservation.CheckOutDate);
                cmd.Parameters.AddWithValue("@status", reservation.Status);
                cmd.Parameters.AddWithValue("@notes", reservation.Notes);

                var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                reservation.Id = newId;
                _reservationCache[newId] = reservation;

                return await GetReservationById(reservation.Id);
            }
            finally
            {
                _reservationWriteLock.Release();
            }
        }

        public static async Task<Reservation?> ModifyReservation(Reservation reservation)
        {
            await _reservationWriteLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                var query = @"UPDATE reservations 
                              SET guest_id = @guestId, room_id = @roomId, check_in_date = @checkInDate, 
                                  check_out_date = @checkOutDate, status = @status, notes = @notes 
                              WHERE id = @id";

                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", reservation.Id);
                cmd.Parameters.AddWithValue("@guestId", reservation.GuestId);
                cmd.Parameters.AddWithValue("@roomId", reservation.RoomId);
                cmd.Parameters.AddWithValue("@checkInDate", reservation.CheckInDate);
                cmd.Parameters.AddWithValue("@checkOutDate", reservation.CheckOutDate);
                cmd.Parameters.AddWithValue("@status", reservation.Status);
                cmd.Parameters.AddWithValue("@notes", reservation.Notes);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _reservationCache[reservation.Id] = reservation;
                    return await GetReservationById(reservation.Id);
                }
                return null;
            }
            finally
            {
                _reservationWriteLock.Release();
            }
        }
        public static async Task<List<Reservation>> GetUserReservations(User user)
        {
            await _reservationReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                var reservations = new List<Reservation>();
                var query = "SELECT * FROM reservations WHERE guest_id = @guestId";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@guestId", user.Id);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var reservation = new Reservation
                    {
                        Id = reader.GetInt32("id"),
                        GuestId = reader.GetInt32("guest_id"),
                        RoomId = reader.GetInt32("room_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.GetDateTime("check_out_date"),
                        Status = reader.GetInt32("status"),
                        Notes = reader.GetString("notes")
                    };
                    reservations.Add(reservation);
                    _reservationCache[reservation.Id] = reservation;
                }
                return reservations;
            }
            finally
            {
                _reservationReadLock.Release();
            }
        }

    }
}
