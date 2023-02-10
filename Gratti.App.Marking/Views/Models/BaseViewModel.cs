using System;
using System.Windows;
using ReactiveUI;

namespace Gratti.App.Marking.Views.Models
{
    public class BaseViewModel : ReactiveObject
    {
        public void SyncThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                action?.Invoke();
            });
        }
    }
}
