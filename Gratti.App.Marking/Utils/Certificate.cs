using Gratti.App.Marking.Model;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
                    if (ExistsOid(certificate, "1.2.643.100.111"))
                    {
                        result.Add(new CertificateInfoModel()
                        {
                            ThumbPrint = certificate.Thumbprint,
                            Name = GetName(certificate),
                            INN = GetINN(certificate),
                            NotAfter = certificate.NotAfter,

                        });
                    }
                }
            }
            return result;
        }

        internal static CertificateInfoModel GetCertificateInfo(string thumbPrint)
        {
            CertificateInfoModel result = new CertificateInfoModel()
            {
                Name = "Сертификат не выбран",
                INN = "000000000000"
            };
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    if (certificate.Thumbprint == thumbPrint)
                    {
                        result = new CertificateInfoModel()
                        {
                            ThumbPrint = certificate.Thumbprint,
                            Name = GetName(certificate),
                            INN = GetINN(certificate),
                            NotAfter = certificate.NotAfter,

                        };
                        break;
                    }
                }
            }
            return result;
        }
        internal static X509Certificate2 GetCertificate(string thumbPrint)
        {
            X509Certificate2 result = null;
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    if (certificate.Thumbprint == thumbPrint)
                    {
                        result = certificate;
                        break;
                    }
                }
            }
            return result;
        }

        internal static bool ExistsOid(X509Certificate2 certificate, string oid)
        {
            if (certificate != null)
            {
                X509ExtensionCollection ext = certificate.Extensions;
                foreach(var o in ext)
                {
                    if (o.Oid?.Value == oid)
                        return true;
                }
            }
            return false;
        }
 

        internal static string GetName(X509Certificate2 certificate)
        {
            string result = string.Empty;
            if (certificate != null)
            {
                string val = GetNameFromSubjectByOid(certificate, X509Name.CN);
                if (!string.IsNullOrEmpty(val))
                    result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : " ", val);

                val = GetNameFromSubjectByOid(certificate, X509Name.Surname);
                if (!string.IsNullOrEmpty(val))
                    result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : ", ", val);

                val = GetNameFromSubjectByOid(certificate, X509Name.GivenName);
                if (!string.IsNullOrEmpty(val))
                    result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : " ", val);
                val = GetNameFromSubjectByOid(certificate, X509Name.T);
                if (!string.IsNullOrEmpty(val))
                    result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : " ", "(", val, ")");

            }
            return result;
        }

        internal static string GetINN(X509Certificate2 certificate)
        {
            string result = null;
            if (certificate != null)
            {
                result = GetNameFromSubjectByOid(certificate, new DerObjectIdentifier("1.2.643.100.4"));
                if (string.IsNullOrEmpty(result))
                {
                    result = GetNameFromSubjectByOid(certificate, new DerObjectIdentifier("1.2.643.3.131.1.1"));
                }
            }
            return result;
        }

        internal static string GetNameFromSubjectByOid(X509Certificate2 x509Certificate, DerObjectIdentifier oid)
        {
            DerObjectIdentifier oid2 = oid;
            Org.BouncyCastle.X509.X509Certificate certificate = DotNetUtilities.FromX509Certificate(x509Certificate);
            if (certificate.SubjectDN.GetOidList().OfType<DerObjectIdentifier>().FirstOrDefault((DerObjectIdentifier der) => der.Equals(oid2)) != null)
            {
                return certificate.SubjectDN.GetValueList(oid2).OfType<string>().FirstOrDefault();
            }
            return null;
        }


        internal static string SignByCertificate(X509Certificate2 cert, string data)
        {
            // данные для подписи
            var content = new System.Security.Cryptography.Pkcs.ContentInfo(Encoding.UTF8.GetBytes(data));
            var signedCms = new System.Security.Cryptography.Pkcs.SignedCms(content, false);


            // настраиваем сертификат для подписи, добавляем дату
            var signer = new System.Security.Cryptography.Pkcs.CmsSigner(System.Security.Cryptography.Pkcs.SubjectIdentifierType.IssuerAndSerialNumber, cert);
            signer.SignedAttributes.Add(new System.Security.Cryptography.Pkcs.Pkcs9SigningTime(DateTime.Now));

            // формируем подпись
            signedCms.ComputeSignature(signer, false);
            byte[] sign = signedCms.Encode();

            return Convert.ToBase64String(sign);
        }

    }
}
