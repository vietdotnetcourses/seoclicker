using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Affilitest.Models
{
    public class DataResult
    {
        public Status Status { get; set; }
        public String Message { get; set; }

        public String LinkRedirect { get; set; }
        public List<Object> Data { get; set; }

        public DataResult()
        {
            Data = new List<object>();
        }

    }

    public enum Status
    {
        Success,
        Error,
        Redirect
    }
}