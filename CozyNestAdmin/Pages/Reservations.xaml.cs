using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using static CozyNestAdmin.GlobalMethods;
using static CozyNestAdmin.GlobalEnums;

namespace CozyNestAdmin
{
    public partial class Misc : Page
    {
        private const string GetReservationsUrl = "http://localhost:5232/api/admin/getreservations";
        private const string AddServiceUrl = "http://localhost:5232/api/admin/addservice";
        private const string CancelReservationUrl = "http://localhost:5232/api/admin/cancelreservation";

        private List<Reservation> currentReservations = new();

        public Misc()
        {
            InitializeComponent();
        }

        private async void LoadReservations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
                var response = await client.GetAsync(GetReservationsUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var reservations = JsonSerializer.Deserialize<ReservationResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    currentReservations = reservations?.Reservations ?? new();
                    ReservationsListBox.Items.Clear();

                    foreach (var res in currentReservations)
                    {
                        ReservationsListBox.Items.Add(
                            $"#{res.Id} - Szoba {res.RoomNumber} ({res.CheckInDate:yyyy-MM-dd} → {res.CheckOutDate:yyyy-MM-dd}) - {res.RoomType}"
                        );
                    }
                }
                else
                {
                    MessageBox.Show($"Hiba a foglalások betöltésekor: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kivétel: {ex.Message}");
            }
        }

        private async void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationsListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Kérlek válassz ki egy foglalást a törléshez.");
                return;
            }

            var selectedReservation = currentReservations[ReservationsListBox.SelectedIndex];

            var confirm = MessageBox.Show($"Biztosan törlöd a(z) #{selectedReservation.Id} azonosítójú foglalást?",
                                          "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var payload = new
                {
                    reservationId = selectedReservation.Id
                };

                var json = JsonSerializer.Serialize(payload);
                using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(CancelReservationUrl),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Foglalás sikeresen törölve!");
                    LoadReservations_Click(null, null); // frissítés
                }
                else
                {
                    MessageBox.Show($"Hiba történt a törlés során: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kivétel: {ex.Message}");
            }
        }

        private async void SaveService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ServiceNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ServiceDescriptionTextBox.Text) ||
                    !double.TryParse(ServicePriceTextBox.Text, out double price))
                {
                    MessageBox.Show("Kérlek töltsd ki az összes mezőt helyesen.");
                    return;
                }

                var service = new
                {
                    name = ServiceNameTextBox.Text.Trim(),
                    description = ServiceDescriptionTextBox.Text.Trim(),
                    price = price
                };

                var json = JsonSerializer.Serialize(service);
                using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);

                var response = await client.PostAsync(AddServiceUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Szolgáltatás sikeresen hozzáadva!");
                    ServiceNameTextBox.Text = "";
                    ServiceDescriptionTextBox.Text = "";
                    ServicePriceTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show($"Hiba a szolgáltatás mentésekor: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kivétel: {ex.Message}");
            }
        }
    }

    public class ReservationResponse
    {
        public string Message { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    public class Reservation
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomDescription { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
        public string Notes { get; set; }
    }
}
