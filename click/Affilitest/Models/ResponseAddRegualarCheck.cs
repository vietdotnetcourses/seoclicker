using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Affilitest.Models
{
    public class ResponseAddRegualarCheck
    {

        public ResponseAddRegualarCheck() { }

        public ResponseAddRegualarCheckItem item { get; set; }

        public string info { get; set; }

    }


    public class ResponseAddRegualarCheckItem
    {

        public string id { get; set; }
        public string url { get; set; }

        public int type { get; set; }

        public string typeText { get; set; }

        public int status { get; set; }

        public string statusText { get; set; }

        public string countryCode { get; set; }

        public ResponseAddRegualarDevice device { get; set; }

        public List<string> checkResults { get; set; }
    }

    public class ResponseAddRegualarDevice
    {

        public string type { get; set; }

        public string version { get; set; }

    }

}