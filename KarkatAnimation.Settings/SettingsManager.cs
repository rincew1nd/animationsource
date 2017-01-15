using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace KarkatAnimation.Settings
{
    /// <summary>
    /// Settings manager for loading\saving settings to external json file
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// Settings object
        /// </summary>
        public static SettingsObj Settings;

        /// <summary>
        /// Settings json file location
        /// </summary>
        private static readonly string ConfigLocation =
            AppDomain.CurrentDomain.BaseDirectory + "\\settings.json";

        /// <summary>
        /// Load or Create default settings json file
        /// </summary>
        /// <returns></returns>
        public static SettingsObj Load()
        {
            if (File.Exists(ConfigLocation))
            {
                Settings = JsonConvert
                    .DeserializeObject<SettingsObj>(
                        File.ReadAllText(ConfigLocation)
                    );
            }
            else
            {
                Settings = new SettingsObj
                {
                    LastUsedDevice = 0,
                    Silence = 0,
                    Speaking = 0,
                    Shouting = 0,
                    UpdateTime = 500,
                    SampleDelta = 1,
                    Images = new Dictionary<VolumeType, List<AnimationImage>>()
                };
                Save();
            }
            return Settings;
        }

        /// <summary>
        /// Save settings to json file
        /// </summary>
        public static void Save()
        {
            var settingsJson = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(ConfigLocation, settingsJson);
        }
    }
}
