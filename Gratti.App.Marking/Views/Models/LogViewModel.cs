using System;
using System.Collections.ObjectModel;
using Gratti.App.Marking.Core.Interfaces;

namespace Gratti.App.Marking.Views.Models
{
    public class LogViewModel : BaseViewModel, ILoggerOutput
    {
        public static readonly ObservableCollection<string> LogItems = new ObservableCollection<string>();

        public ObservableCollection<string> Items => LogItems;

        public void Log(string logRecord)
        {
            SyncThread(() => LogItems.Insert(0, string.Concat(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), ": ", logRecord)));
        }
    }
}
