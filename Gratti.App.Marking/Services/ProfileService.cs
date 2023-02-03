using Gratti.App.Marking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Services
{
    public class ProfileService
    {
        public ProfileService()
        {

        }

        public List<ProfileModel>  Profiles { get => LoadProfiles(); } 

        private List<ProfileModel> LoadProfiles()
        {
            List<ProfileModel> result = new List<ProfileModel>();
            ProfileModel profileDev = new ProfileModel
            {
                Name = "Тестовый сервер (https://markirovka.sandbox.crptech.ru)",
                OmsUri = "https://suz.sandbox.crptech.ru",
            }
            , profileProd = new ProfileModel
            {
                Name = "Основной сервер (https://markirovka.crpt.ru)",
                OmsUri = "https://suzgrid.crpt.ru",
            };

            result.Add(profileDev);
            result.Add(profileProd);
            return result;
        }
    }
}
