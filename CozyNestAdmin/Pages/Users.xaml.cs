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
            LoadUsers();
        }
        public async void LoadUsers() 
        {
            if (!await Introspect())
            {
                ReturnToLogin();
                return;
            }
            using HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken);
            var response = await client.GetAsync(GetEndpoint(AdminEndpoints.GetUsers));
            if (response.IsSuccessStatusCode)
            {
                GetUsersResponse getUsersResponse = JsonConvert.DeserializeObject<GetUsersResponse>(await response.Content.ReadAsStringAsync());
                ObservableCollection<UserDataResponse> users = new(getUsersResponse.Users);
                UsersDataGrid.ItemsSource = users;
            }
        }

        private void SearchUserListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddUserListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void NameSearchComboBox_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
