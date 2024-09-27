using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MetaMask.Unity;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MetaMask
{
    public class MetaMaskVersionChecker
    {
        public class VersionInfo
        {
            [JsonProperty("stable")]
            public string Stable;
        }
        
        internal const string dismissedUpdateTimePrefKey = "MetaMaskUpdateDismissedTime";
        internal const string dismissedUpdatePrefKey = "MetaMaskUpdateDismissed";
        private const string apiVersionCheckUrl = "https://raw.githubusercontent.com/MetaMask/metamask-sdk/main/platforms.json";
        private const double dismissTimeout = 2.592e+6; // 30 days in seconds
        
        // This method will be called on editor startup
        [DidReloadScripts]
        private static async void CheckMetaMaskVersionOnStartup()
        {
            if (EditorPrefs.GetBool(dismissedUpdatePrefKey, false))
            {
                var timestampStr = EditorPrefs.GetString(dismissedUpdateTimePrefKey);
                var timestamp = double.Parse(timestampStr);

                var now = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

                if (now - timestamp >= dismissTimeout)
                {
                    EditorPrefs.DeleteKey(dismissedUpdatePrefKey);
                    EditorPrefs.DeleteKey(dismissedUpdateTimePrefKey);
                }
                else
                {
                    // The user has previously dismissed the update notification, so we won't check again
                    return;
                }
            }

            try
            {
                string latestVersion = await FetchLatestMetaMaskVersion();
                string currentVersion = MetaMaskWallet.Version;

                if (latestVersion != currentVersion)
                {
                    // New version available, reset the update queued flag
                    MetaMaskGettingStartedWindow.UpdateQueued = false;
                    
                    // and prompt the user
                    MetaMaskUpdatePromptWindow.ShowWindow();
                    
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError($"Failed to check MetaMask SDK version: {ex.Message}");
            }
        }

        private static async Task<string> FetchLatestMetaMaskVersion()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiVersionCheckUrl);
                response.EnsureSuccessStatusCode();
                
                string json = await response.Content.ReadAsStringAsync();
                Dictionary<string, VersionInfo> platforms =
                    JsonConvert.DeserializeObject<Dictionary<string, VersionInfo>>(json);

                return platforms.TryGetValue("metamask-unity-sdk", out var platform) ? platform.Stable : MetaMaskWallet.Version; // return self version as latest as fallback
            }
        }
    }
}