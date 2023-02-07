using Gratti.Marking.Extensions;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Gratti.App.Marking.Views.Models
{
    public class MainWindowViewModel : ContentViewModel
    {
        public MainWindowViewModel()
        {
            App.Self.MainVM = this;
            Content = new Controls.EnterView();
        }

        private Visibility visibilityBusy = Visibility.Collapsed;
        public Visibility VisibilityBusy
        {
            get { return visibilityBusy; }
            set { this.RaiseAndSetIfChanged(ref visibilityBusy, value); }
        }
        
        private int countBusy = 0;
        public void Busy(bool aIsShow)
        {
            countBusy += (aIsShow ? 1 : -1);
            VisibilityBusy = (countBusy > 0 ? Visibility.Visible : Visibility.Collapsed);
        }
        public void Error(string errorMessage, string aTitle = "Гратти.Маркировка")
        {
            MessageBox.Show(errorMessage, aTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        public void Run(Action action)
        {
            if (action != null)
            {
                Busy(true);
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TryCatch.Invoke(action, (error) => this.Log(error));
                        Busy(false);
                    });
                });

            }
        }
    }
}
