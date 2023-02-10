using Gratti.App.Marking.Core.Interfaces;
using System.Collections.ObjectModel;

namespace Gratti.App.Marking.Views.Models
{
    public class LogViewModel : BaseViewModel, ILoggerOutput
    {
        public static readonly ObservableCollection<string> LogItems = new ObservableCollection<string>();

        public ObservableCollection<string> Items => LogItems;

        public void Log(string logRecord)
        {
            SyncThread(() => LogItems.Insert(0, logRecord));
        }
    }
}
