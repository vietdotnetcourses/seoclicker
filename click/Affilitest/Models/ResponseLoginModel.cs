using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Affilitest.Models
{
    public class ResponseLoginModel
    {

        public ResponseLoginModel() { }

        public string info { get; set; }
        public string token { get; set; }
    }

}