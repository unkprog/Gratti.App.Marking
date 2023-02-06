﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Model
{
    public class CertificateInfoModel
    {
        public string SerialNumber { get; set; } = null;
        public string Name { get; set; } = null;
        public string INN { get; set; } = null;

        [JsonIgnore]
        public string INNStr => string.IsNullOrEmpty(INN) ? string.Empty : string.Concat("ИНН: ", INN);

        public DateTime NotAfter { get; set; }

        [JsonIgnore]
        public string NotAfterStr => new DateTime(1899, 12, 30) >= NotAfter ? string.Empty : string.Concat("Действителен до: ", NotAfter.ToString("dd.MM.yyyy HH:mm:ss"));
    }
}
