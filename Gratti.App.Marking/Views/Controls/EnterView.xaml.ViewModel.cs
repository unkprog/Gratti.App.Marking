using Gratti.App.Marking.Model;
using Gratti.App.Marking.Utils;
using Gratti.App.Marking.Views.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Views.Controls.Models
{
    public class EnterViewModel : LogViewModel
    {
        public EnterViewModel()
        {
            loadProfiles();
        }

        public ObservableCollection<ProfileInfoModel> Profiles { get; private set; } = new ObservableCollection<ProfileInfoModel>();

        public ProfileInfoModel CurrentProfile { get; set; }


        public ObservableCollection<CertificateInfoModel> Certificates { get; private set; } = Utils.Certificate.GetCertificatesList();

        public CertificateInfoModel CurrentCertificate { get; set; }

        public string OmsId
        {
            get => CurrentProfile.OmsId;
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.OmsId = value;
            }
        }

        public string ConnectionId
        {
            get => CurrentProfile?.ConnectionId; 
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.ConnectionId = value;
            }
        }

        private void loadProfiles()
        {
            ProfileInfoModel profileDev = new ProfileInfoModel
            {
                Name = "Тестовый сервер (https://markirovka.sandbox.crptech.ru)",
                GisUri = "https://markirovka.sandbox.crptech.ru",
                OmsUri = "https://suz.sandbox.crptech.ru",
            };


            ProfileInfoModel profileProd = new ProfileInfoModel
            {
                Name = "Основной сервер (https://markirovka.crpt.ru)",
                GisUri = "https://markirovka.crpt.ru",
                OmsUri = "https://suzgrid.crpt.ru",
            };

            Profiles.Add(profileDev);
            Profiles.Add(profileProd);

            CurrentProfile = profileProd;
        }

        public string saveProfiles()
        {
            string result = string.Empty;

            string fileSettingsPath = IO.GetFileSettingsPath();

            SettingModel setting = new SettingModel()
            {
                Dev = Profiles[0],
                Prod = Profiles[1],
                Current = (CurrentProfile == Profiles[0] ? "Dev" : "Prod")
            };

            if (string.IsNullOrEmpty(CurrentProfile.OmsId))
            {

            }

            File.WriteAllText(fileSettingsPath, setting.ToString());

            return result;
        }
    }
}
