using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using static CozyNestAdmin.GlobalMethods;

namespace CozyNestAdmin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Username.Content = Session.Username;
        }

        // Minimize the window
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Close the window
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Handle dragging of the window (when clicking on the header)
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void LoadPage(object sender, RoutedEventArgs e)
        {
            var selectedItem = ((ListViewItem)sender).Content as StackPanel;
            var textBlock = (TextBlock)selectedItem.Children[1];
            switch (textBlock.Text)
            {
                case "Main":
                    MainContentFrame.Navigate(new Uri("Pages/Main.xaml", UriKind.Relative));
                    break;
                case "Rooms":
                    MainContentFrame.Navigate(new Uri("Pages/Rooms.xaml", UriKind.Relative));
                    break;
                case "Users":
                    MainContentFrame.Navigate(new Uri("Pages/Users.xaml", UriKind.Relative));
                    break;
                case "Misc":
                    MainContentFrame.Navigate(new Uri("Pages/Misc.xaml", UriKind.Relative));
                    break;
                case "Logout":
                    ReturnToLogin(this);
                    break;
                default:
                    break;
            }
        }

    }
}
