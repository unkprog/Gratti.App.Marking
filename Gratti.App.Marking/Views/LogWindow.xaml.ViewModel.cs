using Gratti.App.Marking.Views.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Views.Models
{
    public interface ILoggerOutput
    {
        void Write(string logRecord);
    }

    public class LogViewModel : ViewModelBase, ILoggerOutput
    {
        public static readonly ObservableCollection<string> LogItems = new ObservableCollection<string>();

        public void Write(string logRecord)
        {
            LogItems.Insert(0, logRecord);
        }

        public static void Log(string logRecord)
        {
            LogItems.Insert(0, logRecord);
        }
    }

    public class LogWindowViewModel : LogViewModel
    {

        public LogWindowViewModel()
        {
        }

        public ObservableCollection<string> Items => LogItems;
    }
}
