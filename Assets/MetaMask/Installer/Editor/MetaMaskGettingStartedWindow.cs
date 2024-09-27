using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MetaMask
{
    public class MetaMaskGettingStartedWindow : EditorWindow
    {
        private static Texture2D metaMaskLogo;

        public const string update_pref_key = "metamask.updatepending";
        public static bool UpdateQueued
        {
            get
            {
                return EditorPrefs.GetBool(update_pref_key);
            }
            set
            {
                EditorPrefs.SetBool(update_pref_key, value);
            }
        }
        
        public const string SetupPath = "Assets/MetaMask/Installer/setup";
        public static bool SetupCompleted
        {
            get
            {
                return !File.Exists(SetupPath);
            }
            set
            {
                switch (value)
                {
                    case true when File.Exists(SetupPath):
                        File.Delete(SetupPath);
                        AssetDatabase.Refresh();
                        break;
                    case false when !File.Exists(SetupPath):
                        File.WriteAllText(SetupPath, "");
                        AssetDatabase.Refresh();
                        break;
                }
            }
        }

        [DidReloadScripts]
        private static void CheckShouldShowWindow()
        {
            if (!SetupCompleted)
            {
                ShowWindow();
            }
        }

        [MenuItem("MetaMask/Documentation")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://docs.metamask.io/wallet/how-to/use-sdk/gaming/unity/");
        }

        [MenuItem("Window/GettingStarted")]
        public static void ShowWindow()
        {
            // Load the MetaMask logo
            metaMaskLogo = Resources.Load<Texture2D>("MetaMask/EditorImages/Metamask_Logo");
            var window = GetWindow(typeof(MetaMaskGettingStartedWindow), true, "MetaMask SDK Update");
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
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void BodySection()
        {
            GUILayout.Space(40);
            GUILayout.Label("Thank You for downloading the MetaMask Unity SDK. You will need to install the SDK components, use the button below to open the Install window. You will be given the option to install additional examples and dependencies in the Install window.", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            var buttonText = "Complete Setup";
            if (UpdateQueued)
                buttonText = "Complete Update";
            
            if (GUILayout.Button(buttonText))
            {
                MetaMaskInstallerWindow.Initialize();
                Close();
                File.Delete("Assets/MetaMask/Installer/setup");
            }
            
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://docs.metamask.io/wallet/how-to/use-sdk/gaming/unity/");
            }
        }
    }
}