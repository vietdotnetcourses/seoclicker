using System;
using System.IO;
using System.Windows.Forms;

namespace SeoClicker.Helpers
{
    public static class UserAgentHelper
    {

        public static string GetUserAgentByDevice(string device)
        {
            var result = "";
            var path = Path.Combine(Application.StartupPath, $"user-agent/{device}.txt");
            if (File.Exists(path))
            {
                var agentArray = File.ReadAllLines(path);
                if (agentArray.Length > 0)
                {
                    //get random user-agent
                    var random = new Random();
                    var index = random.Next(0, agentArray.Length - 1);
                    return agentArray[index];
                }
            }
            return result ;
        }
    }
}
