using Newtonsoft.Json;
using SeoClicker.Models;
using System.IO;
using System.Windows.Forms;

namespace SeoClicker.Helpers
{

    public static class DataHelper
    {
        public static string DataFilePath
        {
            get { return Path.Combine(Application.StartupPath, "data.json"); }
        }
        public static void SaveSettings(TargetUrl data)
        {
            File.WriteAllText(DataFilePath, JsonConvert.SerializeObject(data, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        public static TargetUrl LoadData()
        {
            if (!File.Exists(DataFilePath))
            {
                SaveSettings(new TargetUrl());
            }

            return JsonConvert.DeserializeObject<TargetUrl>(File.ReadAllText(DataFilePath));
        }      
       
    }

}
