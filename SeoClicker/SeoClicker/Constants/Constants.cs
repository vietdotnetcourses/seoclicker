using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Constants
{
    public static class DashBoard
    {

        public const string REQUEST_FAIL = "Request failed.";
        public const string REQUEST_SUCCEEDED = "Request succeeded.";
        public const string THREAD_STARTED = "Thread {0} started.";
        public const string ALL_REQUEST_COMPLETED = "All requests completed.";
    }

    public static class Commands
    {
        public const string REQUEST_STRING = "curl --proxy zproxy.lum-superproxy.io:{0} --proxy-user lum-customer-{1}-zone-static-country-{2}:{3} \"{4}\"";
    }

    public static class TaskStatus
    {
        public const string STARTING = "starting";
        public const string STARTED = "started";
        public const string RUNNING = "running";
        public const string NODATA = "no data";
        public const string CANCELLED = "cancelled";
    }
}
