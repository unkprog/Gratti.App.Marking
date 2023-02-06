using Gratti.App.Marking.Model;
using Gratti.App.Marking.Views.Controls.Models;
using Gratti.App.Marking.Views.Models;
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

namespace Gratti.App.Marking.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для EnterView.xaml
    /// </summary>
    public partial class EnterView : UserControl
    {
        public EnterView()
        {
            InitializeComponent();
            ViewModel.Log("EnterView");
        }

        EnterViewModel ViewModel => (this.DataContext as EnterViewModel);

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ViewModel.SaveProfiles();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                App.Self.MainVM.Error(errorMessage, "Вход в систему");
                return;
            }

            App.Self.MainVM.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    App.Self.SetProfile(ViewModel.CurrentProfile);
                    App.Self.Auth.Connect();
                    App.Self.MainVM.Content = new Oms.OrdersView();
                });
            });
        }
    }
}
