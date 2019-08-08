using Newtonsoft.Json;
using SeoClicker.Models;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System;
using SeoClicker.Utils;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

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
            File.AppendAllLines(path, new List<string> { data });
        }

        public static void DeleteResultsFolder()
        {
            var path = Path.Combine(Application.StartupPath, $"Results");
            DirectoryInfo di = new DirectoryInfo(path);
            if(di.GetFiles().Length >= 500)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            

        }

        public static async Task<IEnumerable<SequenceUrl>> FetchDataFromApi(string apiurl, int take)
        {

            var url = $"{apiurl}?take={take}&token=O2ECaKWYM5Q1goceJDI9gNMQ2O8tKskZ";
            try
            {

                var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();     
                return JsonConvert.DeserializeObject<IEnumerable<SequenceUrl>>(responseString);
            }
            catch (Exception ex)
            {
                
                return null;
            }


        }
    }

}
