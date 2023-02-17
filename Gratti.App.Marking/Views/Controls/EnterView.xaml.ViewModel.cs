using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ReactiveUI;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Utils;
using Gratti.App.Marking.Views.Models;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using Gratti.App.Marking.Api.Model;

namespace Gratti.App.Marking.Views.Controls.Models
{
    public class EnterViewModel : LogViewModel
    {
        public EnterViewModel()
        {
            LoadProfiles();
            EnterCommand = ReactiveCommand.Create(() => { Task.Run(() => Enter()); });
            TestNKCommand = ReactiveCommand.Create(() => { Task.Run(() => TestNK()); });
        }

        public ReactiveCommand<Unit, Unit> EnterCommand { get; }
        public ReactiveCommand<Unit, Unit> TestNKCommand { get; }


        public ObservableCollection<ProfileInfoModel> Profiles { get; private set; } = new ObservableCollection<ProfileInfoModel>();


        private ProfileInfoModel currentProfile;
        public ProfileInfoModel CurrentProfile
        {
            get { return currentProfile; }
            set
            {
                this.RaiseAndSetIfChanged(ref currentProfile, value);
                CurrentCertificate = Certificates.FirstOrDefault(f => f.ThumbPrint == currentProfile.ThumbPrint);
            }
        }

        public ObservableCollection<CertificateInfoModel> Certificates { get; private set; } = Utils.Certificate.GetCertificatesList();

        private CertificateInfoModel currentCertificate;
        public CertificateInfoModel CurrentCertificate
        {
            get { return currentCertificate; }
            set
            {
                this.RaiseAndSetIfChanged(ref currentCertificate, value);
                CurrentProfile.ThumbPrint = currentCertificate?.ThumbPrint;
            }
        }

        public string OmsId
        {
            get { return CurrentProfile.OmsId; }
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.OmsId = value;
            }
        }

        public string ConnectionId
        {
            get { return CurrentProfile?.ConnectionId; }
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.ConnectionId = value;
            }
        }

        public string ApiKey
        {
            get { return CurrentProfile?.ApiKey; }
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.ApiKey = value;
            }
        }

        public string SqlConnectionString
        {
            get { return CurrentProfile?.SqlConnectionString; }
            set
            {
                if (CurrentProfile != null)
                    CurrentProfile.SqlConnectionString = value;
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
                        CmgUri = "https://api.integrators.nk.crpt.tech"
                    },
                    Prod = new ProfileInfoModel
                    {
                        Name = "Основной сервер (https://markirovka.crpt.ru)",
                        GisUri = "https://markirovka.crpt.ru",
                        OmsUri = "https://suzgrid.crpt.ru",
                        CmgUri = "https://апи.национальный-каталог.рф"
                    },
                    Current = "Dev"
                };
            }
            else
            {
                if (string.IsNullOrEmpty(setting.Dev.CmgUri))
                    setting.Dev.CmgUri = "https://api.integrators.nk.crpt.tech";
                if (string.IsNullOrEmpty(setting.Prod.CmgUri))
                    setting.Prod.CmgUri = "https://апи.национальный-каталог.рф";
            }

            Profiles.Add(setting.Dev);
            Profiles.Add(setting.Prod);

            CurrentProfile = (setting.Current == "Dev" ? setting.Dev : setting.Prod);
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

            if (string.IsNullOrEmpty(CurrentProfile.ThumbPrint))
                appendResult("Выберите сертификат");
            if (string.IsNullOrEmpty(CurrentProfile.OmsId))
                appendResult("Укажите идентификатор СУЗ (Oms Id)");
            if (string.IsNullOrEmpty(CurrentProfile.ConnectionId))
                appendResult("Укажите идентификатор подключения (Connection Id)");
            if (string.IsNullOrEmpty(CurrentProfile.ApiKey))
                appendResult("Укажите ключ API ГИСМТ (ApiKey)");

            if (string.IsNullOrEmpty(CurrentProfile.SqlConnectionString))
                appendResult("Укажите строку подключения SQL");

            JavaScriptEncoder encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
            string json = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true, Encoder = encoder });
            File.WriteAllText(fileSettingsPath, json);
            return result;
        }

        private void Enter()
        {
            App.Self.MainVM.RunAsync(() =>
            {
                SyncThread(() => App.Self.MainVM.TextBusy = "Сохранение профиля");
                string errorMessage = SaveProfiles();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    App.Self.MainVM.Error(errorMessage, "Вход в систему");
                    return;
                }
                App.Self.SetProfile(CurrentProfile);
                bool isConnected = false;
                try
                {
                    SyncThread(() => App.Self.MainVM.TextBusy = "Подключение");
                    App.Self.Auth.Connect();
                    isConnected = true;
                }
                catch (Exception)
                {
                    SyncThread(() => App.Self.MainVM.Error("Не удалось подключиться! " + Environment.NewLine + "Проверьте настройки."));
                }
                if (isConnected)
                    SyncThread(() => App.Self.MainVM.Content = new Oms.OrdersView());

            });
        }

        private void TestNK()
        {
            App.Self.MainVM.RunAsync(() =>
            {
                App.Self.SetProfile(CurrentProfile);
                ProductModel product = App.Self.CmgApi.ProductByGtin("04610166508225");
                Log("Test " + product.Name);

            });
        }

    }
}
