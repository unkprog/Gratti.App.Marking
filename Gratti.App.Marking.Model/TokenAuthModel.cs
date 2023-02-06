using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Model
{
    public class TokenAuthModel
    {
        /// <summary>
        /// Авторизационный токен в base64-строке
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonIgnore]
        public DateTime Date { get; set; }
    }
}
