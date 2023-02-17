using Gratti.App.Marking.Api;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Services;
using Gratti.App.Marking.Views.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;

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

        public AuthService Auth { get; private set; }
        public Oms OmsApi { get; private set; }
        public Cmg CmgApi { get; private set; }

        public void SetProfile(ProfileInfoModel profile)
        {
            Auth = new AuthService(profile, MainVM);
            OmsApi = new Oms(profile.OmsUri, profile.OmsId);
            CmgApi = new Cmg(profile.CmgUri, profile.ApiKey);
        }

    }
}
