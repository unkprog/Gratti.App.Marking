using Gratti.App.Marking.Model;

namespace Gratti.App.Marking.Views.Models
{
    public class CertViewModel : LogViewModel
    {
        public CertificateInfoModel Certificate
        {
            get
            {
                return Utils.Certificate.GetCertificateInfo(App.Self.Auth.Profile.ThumbPrint);
            }
        }
    }
}
