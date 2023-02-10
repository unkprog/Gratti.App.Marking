using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows;
using Gratti.App.Marking.Extensions;
using System.Collections.Generic;
using DynamicData.Kernel;

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
            //VisibilityBusy = (countBusy > 0 ? Visibility.Visible : Visibility.Collapsed);
            VisibilityBusy = (aIsShow ? Visibility.Visible : Visibility.Collapsed);
        }
        public void Error(string errorMessage, string aTitle = "Гратти.Маркировка")
        {
            MessageBox.Show(errorMessage, aTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        //private bool _isBusy;

        //public bool IsBusy
        //{
        //    get { return _isBusy; }
        //    set { 
        //        this.RaiseAndSetIfChanged(ref _isBusy, value);
        //        Busy(value);
        //    }
        //}

        public void Run(Action action)
        {
            Busy(true);
            var runCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(action));
            runCommand.ThrownExceptions.Subscribe((ex) =>
             {
                 string msg = ex.Message;
                 if (ex.InnerException != null)
                     msg = string.Concat(msg, Environment.NewLine, ex.InnerException.Message);
                 this.Log(msg);
                 Busy(false);
             });
            runCommand.IsExecuting.Subscribe(isExecuting => Busy(isExecuting));
            runCommand.Execute().Subscribe();

            //this.WhenAnyObservable(w => runCommand.IsExecuting).Subscribe(pb => pb.IsIndeterminate);

            //if (action != null)
            //{
            //    Busy(true);
            //    Task.Run(() =>
            //    {
            //        Application.Current.Dispatcher.Invoke(() =>
            //        {
            //            TryCatch.Invoke(action, (error) => this.Log(error));
            //            Busy(false);
            //        });
            //    });

            //}
        }
    }
}
