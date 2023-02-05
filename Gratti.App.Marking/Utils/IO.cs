using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Utils
{
    internal static class IO
    {
        internal static string GetUserLocalFolderPath()
        {
            string result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Marking");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }

        internal static string GetFileSettingsPath()
        {
            string result = Path.Combine(GetUserLocalFolderPath(), "settings.json");
           
            //if (!File.Exists(result))
            //{
            //    File.WriteAllText(result, "{}");
            //}
            return result;
        }

       

    }
}
