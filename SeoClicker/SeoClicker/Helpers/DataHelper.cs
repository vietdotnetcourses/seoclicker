﻿using Newtonsoft.Json;
using SeoClicker.Models;
using System.Collections.Generic;
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

        public static string SpinnerImagePath
        {
            get { return Path.Combine(Application.StartupPath, "Images\\spinner.gif"); }
        }
        public static string GetSpinnerImagePath()
        {

            return SpinnerImagePath;
        }

        public static void SaveResult(string data, string fileName)
        {
            var path = Path.Combine(Application.StartupPath, $"Results\\{fileName}.txt");
            File.AppendAllLines(path, new List<string> { data});
        }
      
        public static void DeleteResultsFolder()
        {
            var path = Path.Combine(Application.StartupPath, $"Results");
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

        }
    }

}
