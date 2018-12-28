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
        public string Zone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Country { get; set; }
        public string Device { get; set; }
        public string Route { get; set; }
        public string SuperProxy { get; set; }
        public string DNSResolution { get; set; }
        public string TargetUrl { get; set; }
        public int Timeout { get; set; }
        public NetworkCredential Credential { get; set; }
        public int NumberOfThread { get; set; }
        public int RequestNumber { get; set; }
        public int IpChangeRequestNumber { get; set; }

    }
}
