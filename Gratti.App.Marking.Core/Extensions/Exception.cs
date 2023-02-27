using System;

namespace Gratti.App.Marking.Core.Extensions
{
    public static partial class ExceptionExtension
    {
        public static string GetMessages(this Exception ex)
        {
            string result = string.Empty;
            Exception locEx = ex;
            while (locEx != null)
            {
                result = string.Concat(result, (string.IsNullOrEmpty(result) ? string.Empty : Environment.NewLine), locEx.Message);
                locEx = locEx.InnerException;
            }
            return result;
        }
    }
}
