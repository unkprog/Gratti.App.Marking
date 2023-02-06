using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Model
{
    public class TokenModel
    {
        /// <summary>
        /// уникальный идентификатор сгенерированных случайных данных, тип string
        /// </summary>
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// случайная строка, тип string
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
