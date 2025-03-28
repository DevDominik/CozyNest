using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CozyNestAdmin.Models;
using CozyNestAdmin.ResponseTypes;
using Newtonsoft.Json;
using static CozyNestAdmin.GlobalMethods;
using static CozyNestAdmin.GlobalEnums;


namespace CozyNestAdmin
{
    public partial class Auth : Window
    {
        private readonly HttpClient _httpClient;
        public static string _accessToken;
        private string _refreshToken;
        const string BASE_URL = "https://localhost:7290";

        public Auth()
        {
            InitializeComponent();
            _httpClient = new HttpClient();

            // Load saved login data if available
            LoadSavedLoginData();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var username = tbUsername.Text;
            var password = tbPassword.Password;

            var (success, message) = await Authenticate(username, password);

            lbResponse.Content = message;

            if (success && (Session.RoleName == "Manager" || Session.RoleName == "Receptionist"))
            {
                // Open main window
                UserInfo.userName = username;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

                // Save login info if checked
                if (cbSaveLogin.IsChecked == true)
                {
                    SaveLoginData(username, password);
                }
            }
            else if (success)
            {
                lbResponse.Content = "Hozzáférés megtagadva.";
            }
        }

        private void SaveLoginData(string username, string password)
        {
            try
            {
                // Define the file path to store the login data
                string filePath = "loginData.json";

                // Create an anonymous object to store the login data
                var loginData = new
                {
                    Username = username,
                    Password = password
                };

                // Serialize the object to JSON
                var json = JsonConvert.SerializeObject(loginData);

                // Write the JSON to a file
                System.IO.File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                lbResponse.Content = $"Error saving login data: {ex.Message}";
            }
        }

        private async void LoadSavedLoginData()
        {
            string filePath = "loginData.json";
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(filePath);
                    var savedData = JsonConvert.DeserializeObject<dynamic>(json);
                    string username = savedData?.Username;
                    string password = savedData?.Password;

                    tbUsername.Text = username;
                    tbPassword.Password = password;

                    var (success, _) = await Authenticate(username, password);

                    if (success && (Session.RoleName == "Manager" || Session.RoleName == "Receptionist"))
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else if (success)
                    {
                        lbResponse.Content = "Hozzáférés megtagadva.";
                    }
                    else
                    {
                        lbResponse.Content = "Sikertelen automatikus bejelentkezés.";
                    }
                }
                catch (Exception ex)
                {
                    lbResponse.Content = $"Hiba a bejelentkezési adatok betöltésekor: {ex.Message}";
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }
    }
}
