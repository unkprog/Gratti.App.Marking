using System;

namespace Gratti.App.Marking.Core.Sql
{
    public static class Constants
    {
        public static readonly DateTime minReportDate = new DateTime(1899, 12, 30);
        public static readonly string dateFormat = "dd.MM.yyyy";
        public static readonly string dateFormatWitTime = "dd.MM.yyyy HH:mm:ss";
    }
}
