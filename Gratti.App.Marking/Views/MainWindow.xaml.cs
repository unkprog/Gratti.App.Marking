using System;
using System.Reflection;
using System.Windows;
using Gratti.App.Marking.Views.Models;

namespace Gratti.App.Marking.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void logButton_Click(object sender, RoutedEventArgs e)
        {
            LogWindow.Self.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
