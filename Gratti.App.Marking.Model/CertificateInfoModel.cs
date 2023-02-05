using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Model
{
    public class CertificateInfoModel
    {
        public string SerialNumber { get; set; } = null;
        public string Name { get; set; } = null;
        public string RussianINN { get; set; } = null;

        public DateTime NotAfter { get; set; }

    }
}
