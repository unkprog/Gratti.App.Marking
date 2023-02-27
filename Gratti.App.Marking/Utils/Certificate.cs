using Gratti.App.Marking.Model;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Gratti.App.Marking.Extensions;

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

        //private static CAPICOM.ICertificate GetCertByThumbrint(string thumbprint)
        //{
        //    CAdESCOM.CPStore store = (CAdESCOM.CPStore)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("7CBD72D9-76A3-4939-930C-52C4D6CA206B")));
        //    store.Open();
        //    foreach (CAPICOM.ICertificate certificate in store.Certificates)
        //    {
        //        if (certificate.Thumbprint == thumbprint)
        //        {
        //            return certificate;
        //        }
        //    }
        //    return null;
        //}

        //internal static string SignByCertificateCades(X509Certificate2 x509certificate, string content, bool detached = true)
        //{
        //    CAPICOM.ICertificate certificate = GetCertByThumbrint(x509certificate.Thumbprint);
        //    CAdESCOM.CadesSignedData obj = (CAdESCOM.CadesSignedData)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("1264A46A-FDB8-43A3-AEE3-00E1684C98E9")));
        //    obj.ContentEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY;
        //    obj.Content = Convert.ToBase64String(Encoding.ASCII.GetBytes(content));
        //    CAdESCOM.CadesSignedData signed = obj;
        //    CAdESCOM.CPSigner obj2 = (CAdESCOM.CPSigner)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("94EBF679-1DED-48B9-9FD2-00A3949E0905")));
        //    obj2.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;
        //    obj2.CheckCertificate = true;
        //    obj2.Certificate = certificate;
        //    CAdESCOM.CPSigner signer = obj2;
        //    CAdESCOM.CPAttributes authenticatedAttributes = signer.AuthenticatedAttributes2;
        //    CAdESCOM.CPAttribute obj3 = (CAdESCOM.CPAttribute)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("FE98A77F-7D50-4210-AED4-5B2AE2EDAEF1")));
        //    obj3.Name = CAdESCOM.CADESCOM_ATTRIBUTE.CADESCOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME;
        //    obj3.Value = DateTime.Now;
        //    authenticatedAttributes.Add(obj3);
        //    string resultSign = signed.SignCades(signer, CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES, detached);
        //    string resultNotClRf = resultSign.ReplaceLineEndings(string.Empty);
        //    string result = resultNotClRf.Replace(" ", string.Empty);
        //    return result;
        //}

        internal static string SignByCertificateDetached(X509Certificate2 cert, string data)
        {
            // данные для подписи
            var content = new System.Security.Cryptography.Pkcs.ContentInfo(Encoding.UTF8.GetBytes(data));
            var signedCms = new System.Security.Cryptography.Pkcs.SignedCms(content, true);


            // настраиваем сертификат для подписи, добавляем дату
            var signer = new System.Security.Cryptography.Pkcs.CmsSigner(System.Security.Cryptography.Pkcs.SubjectIdentifierType.IssuerAndSerialNumber, cert);
            signer.IncludeOption = X509IncludeOption.WholeChain;
            signer.SignedAttributes.Add(new System.Security.Cryptography.Pkcs.Pkcs9SigningTime(DateTime.Now));

            // формируем подпись
            signedCms.ComputeSignature(signer, false);
            byte[] sign = signedCms.Encode();

            return Convert.ToBase64String(sign);
        }
    }
}
