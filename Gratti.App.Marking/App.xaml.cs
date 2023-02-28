using System.Windows;
using Gratti.App.Marking.Api;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Services;
using Gratti.App.Marking.Views.Models;

namespace Gratti.App.Marking
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Self = this;
        }

        public static App Self;
        public MainWindowViewModel MainVM { get; set; }

        public AuthService Auth => appState.Auth;
        public Oms OmsApi => appState.OmsApi;
        public Cmg CmgApi => appState.CmgApi;

        private AppState appState = new AppState();

        public void SetProfile(ProfileInfoModel profile)
        {
            appState.SetProfile(profile, MainVM);
        }

    }
}
