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

namespace Gratti.App.Marking.Services
{
    public class AuthService
    {
        public AuthService(ProfileInfoModel profile)
        {
            this.profile = profile;
        }

        ProfileInfoModel profile;
        private TokenAuthModel omsToken = null;
        public string OmsToken { get => getOmsToken(); }


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

            using (HttpClient client = GetHttpClient())
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
            using (HttpClient client = GetHttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(profile.GisUri));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                result = client.PostJson<TokenModel, TokenAuthModel>("/api/v3/auth/cert", tokenRequest);
            }

            return result;
        }

        public void Connect(Action<string> trace)
        {
            TokenModel tokenResponse = GetTokenResponse();

            trace?.Invoke("Выбор сертификата для авторизации...");
            X509Certificate2 cert = Utils.Certificate. App.GetCertificate(trace);

            //trace?.Invoke("Подписывание токена для авторизации...");
            //// данные для подписи
            //var content = new ContentInfo(Encoding.UTF8.GetBytes(tokenResponse.Data));
            //var signedCms = new SignedCms(content, false);

            //// настраиваем сертификат для подлиси, добавляем дату
            //var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert);
            //signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));

            //// формируем подпись
            //signedCms.ComputeSignature(signer, false);
            //byte[] sign = signedCms.Encode();

            trace?.Invoke("Авторизация...");
            // TokenModel tokenRequest = new TokenModel() { UUID = tokenResponse.UUID, Data = Convert.ToBase64String(sign) };

            State.TokenAuth = Connect(cert, tokenResponse);

            trace?.Invoke("Token: " + State.TokenAuth?.Token + "...");
        }

    }
}
