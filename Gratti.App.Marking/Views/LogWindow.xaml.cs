using Gratti.App.Marking.Views.Models;
using System.Windows;

namespace Gratti.App.Marking.Views
{
    /// <summary>
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
            DataContext = new LogWindowViewModel();
        }

        static LogWindow _self;
        public static LogWindow Self
        {
            get
            {
                if (_self == null)
                    _self = new LogWindow();
                return _self;
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _self = null;
        }
    }
}
