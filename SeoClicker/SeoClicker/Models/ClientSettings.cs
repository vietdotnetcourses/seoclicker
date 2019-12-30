using SeoClicker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class ClientSettings
    {
        public ClientSettings(string zone, string username, string pwd,
            int timeout, int numberofthread, string apidatauri, int take, int loadcount, int loadtime, bool clearresult = false)
        {
            Zone = zone;
            UserName = username;
            Password = pwd;
            Timeout = timeout;
            NumberOfThread = numberofthread;
            ApiDataUri = apidatauri;
            Take = take;
            LoadCount = loadcount;
            LoadTime = loadtime;
            ClearResult = clearresult;
        }
        public string Zone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int Timeout { get; set; }
        public int NumberOfThread { get; set; }

        public string ApiDataUri { get; set; }

        public int Take { get; set; }
        public int LoadCount { get; set; }
        public int LoadTime { get; set; }
        public bool ClearResult { get; set; }

    }
}
