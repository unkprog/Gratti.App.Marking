using System;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using System.Reflection;
using Gratti.App.Marking.Core.Extensions;

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


        private string textBusy = string.Empty;
        public string TextBusy
        {
            get { return textBusy; }
            set { this.RaiseAndSetIfChanged(ref textBusy, value); }
        }

        public string VersionApp
        {
            get
            {
                return "Версия: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private int countBusy = 0;
        public void Busy(bool aIsShow, bool aIsReset = false)
        {
            countBusy = (!aIsShow && aIsReset ? 0 : countBusy + (aIsShow ? 1 : -1));
            countBusy = (countBusy < 0 ? 0 : countBusy);
            VisibilityBusy = (countBusy > 0 ? Visibility.Visible : Visibility.Collapsed);
            if (VisibilityBusy == Visibility.Collapsed)
                TextBusy = string.Empty;
        }
        public void Error(string errorMessage, string aTitle = "Гратти.Маркировка")
        {
            Log(errorMessage);
            MessageBox.Show(errorMessage, aTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        public void Info(string infoMessage, string aTitle = "Гратти.Маркировка")
        {
            Log(infoMessage);
            MessageBox.Show(infoMessage, aTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        public void RunAsync(Action action, Action<Exception> errorAction = null)
        {
            var runCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(action));
            runCommand.ThrownExceptions.Subscribe((ex) =>
             {
                 Error(ex.GetMessages());
                 Busy(false, true);
             });
            runCommand.IsExecuting.Subscribe(isExecuting => Busy(isExecuting));
            runCommand.Execute().Subscribe();
        }
    }
}
