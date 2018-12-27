using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Affilitest.Models
{
    public class UserCookieData
    {
        public DateTime? CreatedDate { get; set; }
        public string[] Roles { get; set; }
        public UserCookieData() { }
        public UserCookieData(DateTime createdDate)
        {
            this.CreatedDate = createdDate;
        }
    }
}