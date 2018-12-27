using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Affilitest.Models
{
    public class HistoryModel
    {
        public System.Guid HistoryID { get; set; }
        public System.Guid? UserID { get; set; }
        public string URL { get; set; }
        public string Date { get; set; }
        public string Click { get; set; }
        public string GEO { get; set; }
        public string Device { get; set; }
        public string UserName { get; set; }
        public string TotalClick { get; set; }
        public string Status { get; set; }
    }
}