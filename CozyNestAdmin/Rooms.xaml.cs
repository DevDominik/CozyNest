using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CozyNestAdmin
{
    public partial class Rooms : Page
    {
        private const string ApiUrl = "https://localhost:7290/api/room/list";
        private const string ModifyRoomApiUrl = "https://localhost:7290/api/room/modify";
        private Room currentEditingRoom;

        public Rooms()
        {
            InitializeComponent();
            LoadRoomsAsync();
        }

        // Load room data from API
        private async Task LoadRoomsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    RoomResponse roomResponse = JsonConvert.DeserializeObject<RoomResponse>(jsonResponse);
                    DisplayRooms(roomResponse.Rooms);
                }
            }
        }

        // Display the list of rooms as cards
        private void DisplayRooms(List<Room> rooms)
        {
            RoomsWrapPanel.Children.Clear(); // Clear existing cards

            foreach (var room in rooms)
            {
                // Create the room card
                var card = new StackPanel
                {
                    Width = 450,
                    Height = 250,
                    Margin = new System.Windows.Thickness(10),
                    Background = System.Windows.Media.Brushes.LightGray,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                // Room Information Text Blocks
                var roomNumber = new TextBlock
                {
                    Text = $"Room Number: {room.RoomNumber}",
                    FontWeight = System.Windows.FontWeights.Bold,
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(roomNumber);

                var roomType = new TextBlock
                {
                    Text = $"Type: {room.Type}",
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(roomType);

                var price = new TextBlock
                {
                    Text = $"Price per Night: ${room.PricePerNight}",
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(price);

                var description = new TextBlock
                {
                    Text = $"Description: {room.Description}",
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(description);

                var status = new TextBlock
                {
                    Text = $"Status: {room.Status}",
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(status);

                var deleted = new TextBlock
                {
                    Text = $"Deleted: {(room.Deleted ? "Yes" : "No")}",
                    Margin = new System.Windows.Thickness(5)
                };
                card.Children.Add(deleted);

                // Create buttons for Edit and Delete
                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new System.Windows.Thickness(5)
                };

                // Edit Button
                var editButton = new Button
                {
                    Content = "Edit",
                    Width = 75,
                    Margin = new System.Windows.Thickness(5)
                };
                editButton.Click += (sender, e) => EditRoom(room);
                buttonPanel.Children.Add(editButton);

                // Delete Button
                var deleteButton = new Button
                {
                    Content = "Delete",
                    Width = 75,
                    Margin = new System.Windows.Thickness(5)
                };
                deleteButton.Click += (sender, e) => DeleteRoom(room);
                buttonPanel.Children.Add(deleteButton);

                // Add button panel to the card
                card.Children.Add(buttonPanel);

                // Add card to the wrap panel
                RoomsWrapPanel.Children.Add(card);
            }
        }

        // Edit the selected room
        private void EditRoom(Room room)
        {
            // Lock the "Add Room" button and show Save/Cancel
            AddButton.IsEnabled = false;
            SaveButton.IsEnabled = true;
            CancelButton.IsEnabled = true;

            // Populate the sidebar fields with the room data
            currentEditingRoom = room;
            RoomNameTextBox.Text = room.RoomNumber;
            RoomTypeComboBox.SelectedItem = room.Type;
            PriceTextBox.Text = room.PricePerNight.ToString();
            CapacityTextBox.Text = "Not Used"; // Add capacity logic if needed
        }

        // Save the room after editing
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var updatedRoom = new Room
            {
                RoomNumber = RoomNameTextBox.Text,
                Type = RoomTypeComboBox.SelectedItem.ToString(),
                PricePerNight = double.Parse(PriceTextBox.Text),
                Description = "Updated Description", // Update with the real data
                Status = "Available", // Adjust status logic as needed
                Id = currentEditingRoom.Id,
                Deleted = currentEditingRoom.Deleted
            };

            var jsonContent = JsonConvert.SerializeObject(updatedRoom);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsync(ModifyRoomApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Room updated successfully!");
                    LoadRoomsAsync(); // Reload rooms after updating
                }
                else
                {
                    MessageBox.Show("Failed to update the room.");
                }
            }

            // Re-enable "Add Room" button and hide Save/Cancel
            AddButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }

        // Cancel the editing of the room
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Restore sidebar and re-enable "Add Room" button
            RoomNameTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            RoomTypeComboBox.SelectedItem = null;
            CapacityTextBox.Text = string.Empty;
            AddButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }

        // Delete the selected room
        private void DeleteRoom(Room room)
        {
            // Logic for deleting the room (e.g., calling the API)
            MessageBox.Show($"Deleted room: {room.RoomNumber}");
        }

        // Room data model and response
        public class RoomResponse
        {
            public string Message { get; set; }
            public List<Room> Rooms { get; set; }
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
    }
}
