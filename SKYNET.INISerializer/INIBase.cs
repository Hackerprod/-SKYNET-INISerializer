using System;

namespace SKYNET.INI
{
    public class INIBase
    {
        public static event EventHandler<string> OnErrorMessage;

        internal static void InvokeErrorMessage(object msg)
        {
            OnErrorMessage?.Invoke(null, msg.ToString());
        }
    }
}