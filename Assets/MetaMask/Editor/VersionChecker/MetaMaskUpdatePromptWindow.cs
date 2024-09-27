using UnityEditor;
using UnityEngine;

namespace MetaMask
{
    public class MetaMaskUpdatePromptWindow : EditorWindow
    {
        private static Texture2D metaMaskLogo;

        [MenuItem("Window/MetaMask Update Prompt")]
        public static void ShowWindow()
        {
            // Load the MetaMask logo
            metaMaskLogo = Resources.Load<Texture2D>("MetaMask/EditorImages/Metamask_Logo");
            var window = GetWindow(typeof(MetaMaskUpdatePromptWindow), true, "MetaMask SDK Update");
            window.minSize = new Vector2(400, 200);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            HeaderSection();
            BodySection();
            GUILayout.EndVertical();
        }

        private void HeaderSection()
        {
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            int logoWidth = 128;
            int logoHeight = 128;

            if (metaMaskLogo != null)
            {
                GUILayout.Label(metaMaskLogo, GUILayout.Width(logoWidth), GUILayout.Height(logoHeight));
            }

            GUILayout.BeginVertical();
            GUILayout.Space(60);
            GUILayout.Label("MetaMask Unity SDK", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16
            });
            GUILayout.Label("A new update is ready", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void BodySection()
        {
            GUILayout.Space(40);
            GUILayout.Label("A new version of the MetaMask Unity SDK is ready to be downloaded and installed. Click the button to download and install the latest version. You will be prompted with the install window once you download the latest version from the package manager", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            if (GUILayout.Button("Update Now"))
            {
                MetaMaskInstallerWindow.ResetStartupBool();
                MetaMaskGettingStartedWindow.UpdateQueued = true;
                Application.OpenURL("com.unity3d.kharma:content/246786");
            }
        }
    }
}