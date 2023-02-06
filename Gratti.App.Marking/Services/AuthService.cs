using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Gratti.App.Marking.Model;
using Gratti.Marking.Extensions;
using System.Text.Json.Serialization;
using System.Text;
using System.Security.Cryptography.Pkcs;
using Gratti.App.Marking.Core.Interfaces;

namespace Gratti.App.Marking.Services
{
    public class AuthService
    {
        public AuthService(ProfileInfoModel profile, ILoggerOutput logger)
        {
            this.profile = profile;
            this.logger = logger;
        }

        private ILoggerOutput logger;
        private ProfileInfoModel profile;
        private TokenAuthModel omsToken = null;
        public string OmsToken { get { return getOmsToken(); } }


        public string getOmsToken()
        {
            if (omsToken == null || ((DateTime.Now - omsToken.Date).TotalHours > 2))
            {
               //omsToken = Identity?.GetOmsTokenAsync(Certificate, Current.GisUri, ConnId).Result;
            }

            return omsToken.Token;
        }


        private HttpClient GetHttpClient()
        {
            HttpClient result = new HttpClient();

            result.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Gratti.App.Marking.", typeof(AuthService).Assembly.GetName().Version?.ToString()));
            result.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(" + RuntimeInformation.OSDescription + ")"));
            result.Timeout = TimeSpan.FromSeconds(15.0);

            return result;
        }

        public TokenModel GetTokenResponse()
        {
            TokenModel result = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(profile.GisUri, "/api/v3/auth/cert/key"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                result = client.GetJson<TokenModel>("");
            }
            return result;
        }

        public TokenAuthModel Connect(X509Certificate2 cert, TokenModel tokenResponse)
        {
            // данные для подписи
            var content = new ContentInfo(Encoding.UTF8.GetBytes(tokenResponse.Data));
            var signedCms = new SignedCms(content, false);


            // настраиваем сертификат для подлиси, добавляем дату
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert);
            signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));

            // формируем подпись
            signedCms.ComputeSignature(signer, false);
            byte[] sign = signedCms.Encode();

            TokenModel tokenRequest = new TokenModel() { UUID = tokenResponse.UUID, Data = Convert.ToBase64String(sign) };

            TokenAuthModel result = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(profile.GisUri, "/api/v3/auth/cert/"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                result = client.PostJson<TokenModel, TokenAuthModel>("", tokenRequest);
            }

            return result;
        }

        public void Connect()
        {
            logger?.Log("Получение токена...");
            TokenModel tokenResponse = GetTokenResponse();
            logger?.Log("Токен для авторизации получен (UUID: " + tokenResponse.UUID + ", Data:" + tokenResponse.Data + ")...");

            logger?.Log("Выбор сертификата для авторизации...");
            X509Certificate2 cert = Utils.Certificate.GetCertificate(profile.SerialNumber);

            logger?.Log("Авторизация...");

            omsToken = Connect(cert, tokenResponse);

            logger?.Log("Token: " + omsToken.Token + "...");
        }

    }
}
