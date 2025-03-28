using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CozyNestAdmin.Models;
using CozyNestAdmin.ResponseTypes;
using Newtonsoft.Json;

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

            var loginData = new
            {
                Username = username,
                Password = password
            };

            var responseMessage = await LoginAsync(loginData);

            // Update the label with the response message
            lbResponse.Content = responseMessage;

            // If login is successful, check the role
            if (responseMessage == "Login successful!")
            {
                bool isManager = await CheckUserRoleAsync(_accessToken);
                if (isManager)
                {
                    // Open MainWindow.xaml
                    UserInfo.userName = tbUsername.Text;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); // Close the Auth window
                }
                else
                {
                    lbResponse.Content = "Hozzáférés megtagadva.";
                }

                // If Save Login is checked, save login information to a file
                if (cbSaveLogin.IsChecked == true)
                {
                    SaveLoginData(username, password);
                }
            }
        }

        private async Task<string> LoginAsync(object loginData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(BASE_URL + "/api/account/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    // Assume the response body contains the tokens in a JSON format like:
                    // { "accessToken": "abc", "refreshToken": "xyz" }
                    var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                    // Set the static _accessToken
                    _accessToken = tokenData.AccessToken;

                    // If needed, also set the refresh token
                    _refreshToken = tokenData.RefreshToken;

                    return "Login successful!";
                }
                else
                {
                    return $"Login failed: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private async Task<bool> CheckUserRoleAsync(string accessToken)
        {
            try
            {
                // Set up the Authorization header with the access token
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.GetAsync(BASE_URL + "/api/account/introspect");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<UserRoleData>(responseBody);

                    // Check if the user has the "Manager" or "Receptionist" role
                    return userInfo.UserData.RoleName.Equals("Manager", StringComparison.OrdinalIgnoreCase) ||
                           userInfo.UserData.RoleName.Equals("Receptionist", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    lbResponse.Content = $"Failed to introspect: {response.ReasonPhrase}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                lbResponse.Content = $"Error: {ex.Message}";
                return false;
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
                    // Read the saved JSON from the file
                    string json = System.IO.File.ReadAllText(filePath);

                    // Deserialize the JSON into an object
                    var savedData = JsonConvert.DeserializeObject<dynamic>(json);

                    // Populate the username and password fields
                    tbUsername.Text = savedData?.Username;
                    tbPassword.Password = savedData?.Password;

                    // Check if the saved access token is still active
                    if (!string.IsNullOrEmpty(savedData?.AccessToken))
                    {
                        bool isTokenActive = await CheckTokenActiveAsync(savedData?.AccessToken);

                        if (isTokenActive)
                        {
                            // Token is active, log the user in automatically
                            _accessToken = savedData?.AccessToken;
                            _refreshToken = savedData?.RefreshToken;

                            bool isManager = await CheckUserRoleAsync(_accessToken);
                            if (isManager)
                            {
                                // Open MainWindow.xaml
                                MainWindow mainWindow = new MainWindow();
                                mainWindow.Show();
                                this.Close(); // Close the Auth window
                            }
                            else
                            {
                                lbResponse.Content = "Access denied: You do not have Manager role.";
                            }
                        }
                        else
                        {
                            lbResponse.Content = "Token is no longer active. Please log in again.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lbResponse.Content = $"Error loading saved login data: {ex.Message}";
                }
            }
        }

        private async Task<bool> CheckTokenActiveAsync(string accessToken)
        {
            try
            {
                // Set up the Authorization header with the access token
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.GetAsync(BASE_URL + "/api/account/introspect");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var introspectData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    // Check if the token is active
                    return introspectData?.active == true;
                }
                else
                {
                    lbResponse.Content = $"Failed to introspect: {response.ReasonPhrase}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                lbResponse.Content = $"Error checking token status: {ex.Message}";
                return false;
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
