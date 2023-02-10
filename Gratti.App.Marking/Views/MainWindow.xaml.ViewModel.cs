using System;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

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
            VisibilityBusy = (aIsShow ? Visibility.Visible : Visibility.Collapsed);
        }
        public void Error(string errorMessage, string aTitle = "Гратти.Маркировка")
        {
            Log(errorMessage);
            MessageBox.Show(errorMessage, aTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        public void Run(Action action)
        {
            var runCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(action));
            runCommand.ThrownExceptions.Subscribe((ex) =>
             {
                 string msg = ex.Message;
                 if (ex.InnerException != null)
                     msg = string.Concat(msg, Environment.NewLine, ex.InnerException.Message);
                 Error(msg);
                 Busy(false);
             });
            runCommand.IsExecuting.Subscribe(isExecuting => Busy(isExecuting));
            runCommand.Execute().Subscribe();
        }
    }
}
