using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Gratti.App.Marking.Views.Controls.Oms.Models;

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
            this.DataContext = new OrdersViewModel();
        }

        OrdersViewModel ViewModel => (this.DataContext as OrdersViewModel);
    }
}
