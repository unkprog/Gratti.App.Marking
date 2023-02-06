using Gratti.App.Marking.Views.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Views.Models
{
    public class MainWindowViewModel : ContentViewModel
    {
        public MainWindowViewModel()
        {
            App.State.MainVM = this;
            Content = new Controls.EnterView();
        }
    }
}
