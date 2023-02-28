using Gratti.App.Marking.Model;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Gratti.App.Marking.Utils
{
    public static class IO
    {
        public static string GetUserLocalFolderPath()
        {
            string result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Marking");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }

        public static string GetFileSettingsPath()
        {
            string result = Path.Combine(GetUserLocalFolderPath(), "settings.json");
           
            //if (!File.Exists(result))
            //{
            //    File.WriteAllText(result, "{}");
            //}
            return result;
        }

        public static SettingModel GetSetting()
        {

            string fileSettingsPath = GetFileSettingsPath();

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

            return setting;
        }

        public static void SetSetting(SettingModel setting)
        {
            string fileSettingsPath = IO.GetFileSettingsPath();

            JavaScriptEncoder encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
            string json = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true, Encoder = encoder });
            File.WriteAllText(fileSettingsPath, json);
        }

        public static ProfileInfoModel GetCurrentProfile(SettingModel setting)
        {
            return (setting == null ? null : setting.Current == "Dev" ? setting.Dev : setting.Prod);
        }

        public static string VerifytProfile(ProfileInfoModel currentProfile)
        {
            string result = string.Empty;

            var appendResult = new Action<string>((msg) =>
            {
                result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : Environment.NewLine, msg);
            });

            if (string.IsNullOrEmpty(currentProfile.ThumbPrint))
                appendResult("Выберите сертификат");
            if (string.IsNullOrEmpty(currentProfile.OmsId))
                appendResult("Укажите идентификатор СУЗ (Oms Id)");
            if (string.IsNullOrEmpty(currentProfile.ConnectionId))
                appendResult("Укажите идентификатор подключения (Connection Id)");
            if (string.IsNullOrEmpty(currentProfile.ApiKey))
                appendResult("Укажите ключ API ГИСМТ (ApiKey)");
            if (string.IsNullOrEmpty(currentProfile.SqlConnectionString))
                appendResult("Укажите строку подключения SQL");

            return result;
        }
    }
}
