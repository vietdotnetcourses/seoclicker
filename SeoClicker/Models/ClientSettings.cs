using SeoClicker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class ClientSettings
    {
        public string Zone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Country { get; set; }
        public string UserAgent { get; set; }
        public string Route { get; set; }
        public string SuperProxy { get; set; }
        public string DNSResolution { get; set; }
        public string TargetUrl { get; set; }
    }
}
