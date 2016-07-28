using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace StageDesigner
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SettingsLoader : MonoBehaviour
    {
        SettingsLoader()
        {
            Settings.LoadConfig();
            Settings.ModCompatibilityCheck();
            new AppLauncher();
        }
    }

    public static class Settings
    {
        const string configPath = "GameData/StageDesigner/Plugins/PluginData/settings.cfg";
        static string configAbsolutePath;
        static ConfigNode settings;

        public static int window_x;
        public static int window_y;
        public static void LoadConfig()
        {
            configAbsolutePath = Path.Combine(KSPUtil.ApplicationRootPath, configPath);
            settings = ConfigNode.Load(configAbsolutePath) ?? new ConfigNode();
            window_x = GetValue("window_x", 280);
            window_y = GetValue("window_y", 114);

            Events.ConfigSaving += SaveConfig;
        }

        public static void SaveConfig()
        {
            SetValue("window_x", window_x);
            SetValue("window_y", window_y);

            try
            {
                settings.Save(configAbsolutePath);
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException)
            {
                Debug.LogWarning(
                    string.Format(
                        "Stage Designer failed to save its config, verify that the path '{0}' exists",
                        Path.GetDirectoryName(configPath))
                );
            }
        }

        public static void SetValue(string key, object value)
        {
            if (settings.HasValue(key))
            {
                settings.RemoveValue(key);
            }
            settings.AddValue(key, value);
        }

        public static int GetValue(string key, int defaultValue)
        {
            int value;
            return int.TryParse(settings.GetValue(key), out value) ? value : defaultValue;
        }

    }
}