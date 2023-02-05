using Gratti.App.Marking.Model;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Gratti.App.Marking.Utils
{
    internal static class Certificate
    {
        internal static ObservableCollection<CertificateInfoModel> GetCertificatesList()
        {
            ObservableCollection<CertificateInfoModel> result = new ObservableCollection<CertificateInfoModel>();
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    result.Add(new CertificateInfoModel()
                    {
                        SerialNumber = certificate.SerialNumber,
                        Name = certificate.SubjectName.Name,
                        RussianINN = GetRussianINN(certificate),
                        NotAfter = certificate.NotAfter,

                    });
                    //if (certificate.SerialNumber != serialNumber)
                    //{
                    //    continue;
                    //}
                    //return certificate;
                }
            }
            return result;
        }

        internal static string GetRussianINN(X509Certificate2 x509Certificate)
        {
            string result = null;
            if (x509Certificate != null)
            {
                //result = GetNameFromSubjectByOid(x509Certificate, "1.2.643.100.4");
                if (string.IsNullOrEmpty(result))
                {
                    //result = GetNameFromSubjectByOid(x509Certificate, "1.2.643.3.131.1.1");
                }
            }
            return result;
        }
    }
}
