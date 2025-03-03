using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CozyNestAdmin
{
    public partial class Rooms : Page
    {
        private const string BaseUrl = "https://localhost:7290/api/room";
        private string authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjY0OGVjMGUyLWQ2MWMtNGM3MS04Njk5LTllYjlhNDI0ZTdhZiIsInVzZXJJZCI6MSwiZXhwIjoxNzQxMDA4NzgwfQ.jr9vx7jD3tvLwoJ1jZCh6hjnrifYQjPhm2zXViGyuqs\t";
        private List<Room> roomList = new();
        private Room selectedRoom = null;

        public Rooms()
        {
            InitializeComponent();
            LoadRooms();
        }

        private async void LoadRooms()
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var response = await client.GetAsync($"{BaseUrl}/list");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Response: {responseBody}"); // Debugging output

                    var roomData = JsonSerializer.Deserialize<RoomResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (roomData == null || roomData.Rooms == null)
                    {
                        MessageBox.Show("No rooms found in response.");
                        return;
                    }

                    roomList = roomData.Rooms;
                    Application.Current.Dispatcher.Invoke(DisplayRooms);
                }
                else
                {
                    MessageBox.Show($"Error loading rooms: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void DisplayRooms()
        {
            RoomsWrapPanel.Children.Clear();
            foreach (var room in roomList)
            {
                var card = CreateRoomCard(room);
                RoomsWrapPanel.Children.Add(card);
            }
        }

        private UIElement CreateRoomCard(Room room)
        {
            var card = new StackPanel { Margin = new Thickness(10), Width = 200, Height = 150, Background = System.Windows.Media.Brushes.LightGray };
            card.Children.Add(new TextBlock { Text = $"Room {room.RoomNumber}", FontWeight = FontWeights.Bold });
            card.Children.Add(new TextBlock { Text = $"Type: {room.Type}" });
            card.Children.Add(new TextBlock { Text = $"Price: {room.PricePerNight}$" });

            var editButton = new Button { Content = "Edit", Margin = new Thickness(5) };
            var deleteButton = new Button { Content = "Delete", Margin = new Thickness(5) };

            editButton.Click += (s, e) => EditRoom(room);
            deleteButton.Click += (s, e) => DeleteRoom(room.Id);

            card.Children.Add(editButton);
            card.Children.Add(deleteButton);

            return card;
        }

        private async void EditRoom(Room room)
        {
            selectedRoom = room;
            RoomNameTextBox.Text = room.RoomNumber;
            RoomTypeComboBox.Text = room.Type;
            PriceTextBox.Text = room.PricePerNight.ToString();
            CapacityTextBox.Text = room.Description;

            AddButton.IsEnabled = false;
            ConfirmButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        private async void ConfirmEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoom == null) return;

            selectedRoom.RoomNumber = RoomNameTextBox.Text;
            selectedRoom.Type = RoomTypeComboBox.Text;
            selectedRoom.PricePerNight = double.Parse(PriceTextBox.Text);
            selectedRoom.Description = CapacityTextBox.Text;

            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/modify")
                {
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        roomId = selectedRoom.Id,
                        roomNumber = selectedRoom.RoomNumber,
                        typeDescription = selectedRoom.Type,
                        pricePerNight = selectedRoom.PricePerNight,
                        description = selectedRoom.Description,
                        statusDescription = selectedRoom.Status
                    }), Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    LoadRooms();
                    RoomNameTextBox.Text = "";
                    RoomTypeComboBox.Text = "";
                    PriceTextBox.Text = RoomTypeComboBox.Text = "";
                    CapacityTextBox.Text = RoomTypeComboBox.Text = "";
                    AddButton.IsEnabled = true;
                    ConfirmButton.IsEnabled = false;
                    CancelButton.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show($"Error updating room: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void CreateRoom()
        {
            try
            {
                var newRoom = new Room
                {
                    RoomNumber = RoomNameTextBox.Text,
                    Type = RoomTypeComboBox.Text,
                    PricePerNight = double.Parse(PriceTextBox.Text),
                    Description = CapacityTextBox.Text,
                    Status = "Available"
                };

                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/create")
                {
                    Content = new StringContent(JsonSerializer.Serialize(newRoom), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    LoadRooms();
                }
                else
                {
                    MessageBox.Show($"Error creating room: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void DeleteRoom(int roomId)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/delete")
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { roomId }), Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    LoadRooms();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting room: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RoomNameTextBox.Text = "";
            RoomTypeComboBox.Text = "";
            PriceTextBox.Text = RoomTypeComboBox.Text = "";
            CapacityTextBox.Text = RoomTypeComboBox.Text = "";
            AddButton.IsEnabled = true;
            ConfirmButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }
    }
}

public class RoomResponse
    {
        public string Message { get; set; }
        public List<Room> Rooms { get; set; } = new();
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public double PricePerNight { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public string Status { get; set; }
    }

