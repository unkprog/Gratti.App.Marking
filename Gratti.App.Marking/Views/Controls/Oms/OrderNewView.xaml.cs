using Gratti.App.Marking.Views.Controls.Oms.Models;
using System.Windows.Controls;

namespace Gratti.App.Marking.Views.Controls.Oms
{
    /// <summary>
    /// Interaction logic for OrderNewView.xaml
    /// </summary>
    public partial class OrderNewView : UserControl
    {
        public OrderNewView()
        {
            InitializeComponent();
            this.DataContext = new OrderNewViewModel();
        }

        OrderNewViewModel ViewModel => (this.DataContext as OrderNewViewModel);
    }
}