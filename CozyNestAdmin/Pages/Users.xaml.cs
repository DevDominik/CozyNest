using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CozyNestAdmin.GlobalMethods;
using static CozyNestAdmin.GlobalEnums;
using System.Collections.ObjectModel;
using CozyNestAdmin.ResponseTypes;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.VisualBasic.ApplicationServices;

namespace CozyNestAdmin
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
        {
            InitializeComponent();
            LoadAllUsers();
        }
        public async void LoadAllUsers() 
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            List<UserDataResponse>? data = await GetAllUsers();
            if (data == null) { return; }
            ObservableCollection<UserDataResponse> users = new(data);
            UsersDataGrid.ItemsSource = users;
        }
        public async Task<List<UserDataResponse>?> GetAllUsers()
        {
            using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
            var response = await client.GetAsync(GetEndpoint(AdminEndpoints.GetUsers));
            if (response.IsSuccessStatusCode)
            {
                GetUsersResponse getUsersResponse = JsonConvert.DeserializeObject<GetUsersResponse>(await response.Content.ReadAsStringAsync());
                return getUsersResponse.Users;
            }
            return null;
        }
        private async void SearchUserListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            if (NameSearchTextBox.Text.Trim().Length == 0) { LoadAllUsers(); return; }
            List<UserDataResponse>? users = await GetAllUsers();
            if (users == null) { return; }
            ObservableCollection<UserDataResponse> final = new();
            string searchType = (NameSearchComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            switch (searchType)
            {
                case "Felhasználónév":
                    final = new ObservableCollection<UserDataResponse>(users.Where(x => x.Username.ToLower().StartsWith(NameSearchTextBox.Text)).ToList());
                    break;
                case "Id":
                    final = new ObservableCollection<UserDataResponse>(users.Where(x => x.Id.ToString() == NameSearchTextBox.Text).ToList());
                    break;
                case "Email":
                    final = new ObservableCollection<UserDataResponse>(users.Where(x => x.Email.ToLower().StartsWith(NameSearchTextBox.Text)).ToList());
                    break;
                default:
                    break;
            }
            UsersDataGrid.ItemsSource = final;
        }
        
        private void SearchUserListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchUserListViewItem_Asyncronomous();
        }
        private async void AddUserListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            AddUser addUser = new();
            addUser.ShowDialog();
            LoadAllUsers();
        }
        private void AddUserListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddUserListViewItem_Asyncronomous();
        }

        private void ProfileListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BanUnbanListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
