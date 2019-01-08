using Newtonsoft.Json;
using SeoClicker.Models;
using System.IO;
using System.Windows.Forms;

namespace SeoClicker.Helpers
{

    public static class SettingsHelper
    {
        public static string SettingsPath
        {
            get { return Path.Combine(Application.StartupPath, "settings.json"); }
        }
        public static void SaveSettings(Settings data)
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(data, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        public static Settings LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                SaveSettings(new Settings());
            }

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
        }      
       
    }

}
