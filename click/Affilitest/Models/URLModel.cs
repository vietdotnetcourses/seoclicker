using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Affilitest.Models
{
    public class URLModel
    {
        
        public string url { get; set; }
        public string device { get; set; }
        public string country { get; set; }
        public string thread { get; set; }

        public string click { get; set; }
        public string index { get; set; }
        public bool isStop { get; set; }
    }
}