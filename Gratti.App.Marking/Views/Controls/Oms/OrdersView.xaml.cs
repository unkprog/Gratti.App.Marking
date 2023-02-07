using Gratti.App.Marking.Views.Controls.Oms.Models;
using System.Windows;
using System.Windows.Controls;

namespace Gratti.App.Marking.Views.Controls.Oms
{
    /// <summary>
    /// Логика взаимодействия для OrdersView.xaml
    /// </summary>
    public partial class OrdersView : UserControl
    {
        public OrdersView()
        {
            InitializeComponent();

            App.Self.MainVM.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModel.Refresh();
                });
            });
           
        }

        OrdersViewModel ViewModel => (this.DataContext as OrdersViewModel);
    }
}
