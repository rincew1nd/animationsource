using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace KarkatAnimation.Settings
{
    public static class SettingsManager
    {
        public static SettingsObj Settings;
        private static readonly string ConfigLocation =
            AppDomain.CurrentDomain.BaseDirectory + "\\settings.json";

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
                    PeakDelta = 1,
                    Images = null
                };
                Save();
            }
            return Settings;
        }

        public static void Save()
        {
            var settingsJson = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(ConfigLocation, settingsJson);
        }
    }
}
