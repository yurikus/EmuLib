using Comfort.Common;
using EFT;
using UnityEngine;

namespace EmuLib.Utils.Reflection
{
    internal static class ClientAppUtils
    {
        public static ClientApplication GetClientApp()
        {
            ClientApplication clientApp = Singleton<ClientApplication>.Instance;
            if (clientApp != null) return clientApp;

            Debug.LogError("ClientAppUtils GetClientApp() method. clientApp is null");
            return null;
        }

        public static GInterface24 GetBackendSession()
        {
            GInterface24 session = GetClientApp()?.GetClientBackEndSession();
            if (session != null) return session;

            Debug.LogError("ClientAppUtils GetBackendSession() method. BackEndSession is null");
            return null;
        }

        public static string GetSessionId()
        {
            GInterface24 backend = GetBackendSession();
            return backend?.Profile == null ? "-1" : backend.GetPhpSessionId();
        }
    }
}