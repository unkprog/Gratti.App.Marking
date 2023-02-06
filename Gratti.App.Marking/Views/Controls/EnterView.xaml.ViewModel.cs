using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Utils;
using Gratti.App.Marking.Views.Models;
using ReactiveUI;

namespace Gratti.App.Marking.Views.Controls.Models
{
    public class EnterViewModel : LogViewModel
    {
        public EnterViewModel()
        {
            LoadProfiles();
        }

        public ObservableCollection<ProfileInfoModel> Profiles { get; private set; } = new ObservableCollection<ProfileInfoModel>();


        private ProfileInfoModel currentProfile;
        public ProfileInfoModel CurrentProfile
        {
            get => currentProfile;
            set
            {
                this.RaiseAndSetIfChanged(ref currentProfile, value);
                CurrentCertificate = Certificates.FirstOrDefault(f => f.SerialNumber == currentProfile.SerialNumber);
            }
        }

        public ObservableCollection<CertificateInfoModel> Certificates { get; private set; } = Utils.Certificate.GetCertificatesList();

        private CertificateInfoModel currentCertificate;
        public CertificateInfoModel CurrentCertificate
        {
            get => currentCertificate;
            set
            {
                this.RaiseAndSetIfChanged(ref currentCertificate, value);
                CurrentProfile.SerialNumber = currentCertificate?.SerialNumber;
            }
        }

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

        public void LoadProfiles()
        {
            string fileSettingsPath = IO.GetFileSettingsPath();

            SettingModel setting = null;
            if (File.Exists(fileSettingsPath))
            {
                string json = File.ReadAllText(fileSettingsPath);
                setting = JsonSerializer.Deserialize<SettingModel>(json);
            }
            if (setting == null)
            {

                setting = new SettingModel()
                {
                    Dev = new ProfileInfoModel
                    {
                        Name = "Тестовый сервер (https://markirovka.sandbox.crptech.ru)",
                        GisUri = "https://markirovka.sandbox.crptech.ru",
                        OmsUri = "https://suz.sandbox.crptech.ru",
                    },
                    Prod = new ProfileInfoModel
                    {
                        Name = "Основной сервер (https://markirovka.crpt.ru)",
                        GisUri = "https://markirovka.crpt.ru",
                        OmsUri = "https://suzgrid.crpt.ru",
                    },
                    Current = "Dev"
                };
            }

            Profiles.Add(setting.Dev);
            Profiles.Add(setting.Prod);

            CurrentProfile = (setting.Current == "Dev" ? setting.Dev: setting.Prod);
        }

        public string SaveProfiles()
        {
            string result = string.Empty;

            string fileSettingsPath = IO.GetFileSettingsPath();

            SettingModel setting = new SettingModel()
            {
                Dev = Profiles[0],
                Prod = Profiles[1],
                Current = (CurrentProfile == Profiles[0] ? "Dev" : "Prod")
            };

            var appendResult = new Action<string>((msg) =>
            {
                result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : Environment.NewLine, msg);
            });

            if (string.IsNullOrEmpty(CurrentProfile.SerialNumber))
                appendResult("Выберите сертификат");
            if (string.IsNullOrEmpty(CurrentProfile.OmsId))
                appendResult("Укажите идентификатор СУЗ (Oms Id)");
            if (string.IsNullOrEmpty(CurrentProfile.ConnectionId))
                appendResult("Укажите идентификатор подключения (Connection Id)");

            JavaScriptEncoder encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
            string json = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true, Encoder = encoder });
            File.WriteAllText(fileSettingsPath, json);
            return result;
        }
    }
}
