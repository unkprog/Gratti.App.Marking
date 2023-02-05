using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Model
{
    public class SettingModel
    {
        public ProfileInfoModel Prod { get; set; }
        public ProfileInfoModel Dev { get; set; }

        public string Current { get; set; }
    }
}
