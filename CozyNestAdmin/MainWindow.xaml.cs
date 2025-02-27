using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using CozyNestAdmin.Models; // For Page Navigation

namespace CozyNestAdmin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Username.Content = UserInfo.userName;
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

            if (textBlock.Text == "Main")
            {
                MainContentFrame.Navigate(new Uri("Main.xaml", UriKind.Relative));
            }
            if (textBlock.Text == "Rooms")
            {
                MainContentFrame.Navigate(new Uri("Rooms.xaml", UriKind.Relative));
            }
            if (textBlock.Text == "Users")
            {
                MainContentFrame.Navigate(new Uri("Users.xaml", UriKind.Relative));
            }
            if (textBlock.Text == "Misc")
            {
                MainContentFrame.Navigate(new Uri("Misc.xaml", UriKind.Relative));
            }
        }

    }
}
