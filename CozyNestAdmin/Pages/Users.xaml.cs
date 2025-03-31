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
        private UserDataResponse? SelectedUser { get; set; }
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
            UsersListGrid.Visibility = Visibility.Hidden;
            UserProfileGrid.Visibility = Visibility.Visible;

        }
        private void AddUserListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddUserListViewItem_Asyncronomous();
        }
        private async void ProfileListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            UsersListGrid.Visibility = Visibility.Hidden;
            UserProfileGrid.Visibility = Visibility.Visible;
            UserEditGrid.Visibility = Visibility.Visible;
            UserAddGrid.Visibility = Visibility.Hidden;
            EditUserUserIdDisplayTextBlock.Text = SelectedUser.Id.ToString();
            EditUserTextBox.Text = SelectedUser.Username;
            EditUserCloseAccountDisplayTextBlock.Text = SelectedUser.Closed ? "Feloldás" : "Kitiltás";
            EditUserCloseAccountIcon.Kind = SelectedUser.Closed ? MaterialDesignThemes.Wpf.PackIconKind.AccountTick : MaterialDesignThemes.Wpf.PackIconKind.AccountCancel;
            EditUserUsernameDisplayTextBlock.Visibility = Visibility.Visible;
            EditUserTextBox.Visibility = Visibility.Hidden;
        }

        private void ProfileListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                SelectedUser = baseElement.DataContext as UserDataResponse;
            }
            ProfileListViewItem_Asyncronomous();
        }
        private async void BanUnbanListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
            var c = new { Id = SelectedUser.Id, Username = SelectedUser.Username, Closed = !SelectedUser.Closed, RoleName = SelectedUser.RoleName, PasswordReset = false };

            var response = await client.PostAsync(GetEndpoint(AdminEndpoints.ModifyUser), new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                SearchUserListViewItem_Asyncronomous();
                ReturnToUsersListViewItem_Asyncronomous();
            }
            SelectedUser = null;
        }
        private void BanUnbanListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                SelectedUser = baseElement.DataContext as UserDataResponse;
            }
            BanUnbanListViewItem_Asyncronomous();
        }
        private async void ReturnToUsersListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            UsersListGrid.Visibility = Visibility.Visible;
            UserProfileGrid.Visibility = Visibility.Hidden;
            UserAddGrid.Visibility = Visibility.Hidden;
            UserEditGrid.Visibility = Visibility.Hidden;
        }
        private void ReturnToUsersListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedUser = null;
            ReturnToUsersListViewItem_Asyncronomous();
        }
        private async void AddUserConfirmListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            string role = (AddUserRoleComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
            var c = new { Username = AddUserUsernameTextBox.Text, Role = role, Password = AddUserPasswordTextBox.Text, FirstName = AddUserFirstNameTextBox.Text, LastName = AddUserLastNameTextBox.Text, Email = AddUserEmailTextBox.Text, Address = AddUserAddressTextBox.Text };

            var response = await client.PostAsync(GetEndpoint(AdminEndpoints.AddUser), new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                SearchUserListViewItem_Asyncronomous();
                ReturnToUsersListViewItem_Asyncronomous();
            }
            SelectedUser = null;
        }

        private void AddUserConfirmListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddUserConfirmListViewItem_Asyncronomous();
        }
        private async void EditUserConfirmListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
            var c = new { Id = SelectedUser.Id, Username = SelectedUser.Username, Closed = SelectedUser.Closed, RoleName = SelectedUser.RoleName, PasswordReset = false };

            var response = await client.PostAsync(GetEndpoint(AdminEndpoints.ModifyUser), new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                SearchUserListViewItem_Asyncronomous();
                ReturnToUsersListViewItem_Asyncronomous();
            }
            SelectedUser = null;
        }
        private void EditUserConfirmListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            EditUserConfirmListViewItem_Asyncronomous();
        }
        private async void EditUserCloseAccountListViewItem_Asyncronomous()
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            SelectedUser.Closed = !SelectedUser.Closed;
        }
        private void EditUserCloseAccountListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            EditUserConfirmListViewItem_Asyncronomous();
        }

        private void EditUserUsernameListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditUserTextBox.Visibility == Visibility.Visible)
            {
                EditUserUsernameDisplayTextBlock.Visibility = Visibility.Visible;
                EditUserTextBox.Visibility = Visibility.Hidden;
            }
            else
            {
                EditUserUsernameDisplayTextBlock.Visibility = Visibility.Hidden;
                EditUserTextBox.Visibility = Visibility.Visible;
            }
        }
    }
}
