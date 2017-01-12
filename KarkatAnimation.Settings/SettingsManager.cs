using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

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
                Settings = new JavaScriptSerializer()
                    .Deserialize<SettingsObj>(
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
            var settingsJson = new JavaScriptSerializer().Serialize(Settings);
            File.WriteAllText(ConfigLocation, settingsJson);
        }
    }
}
