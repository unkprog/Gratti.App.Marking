using System;
using System.Windows;

namespace Gratti.Marking.Extensions
{
    public static class TryCatch
    {
        public static void Invoke(Action action, Action<string> errorMessage)
        {
            Invoke(action, (ex) =>
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg = string.Concat(msg, Environment.NewLine, ex.InnerException.Message);
                errorMessage?.Invoke(msg);
            });
        }

        public static void Invoke(Action action, Action<Exception> error)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
            }
        }
    }
}
