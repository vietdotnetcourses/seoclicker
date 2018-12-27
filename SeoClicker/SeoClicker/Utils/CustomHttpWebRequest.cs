using SeoClicker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Utils
{
    public class CustomHttpWebRequest : WebRequest
    {
        private string userName = "";
        private string passWord = "";
        private string zone = "";
        private int port = 0;
        private string country = "";
        private string device = "";
        private string route = "";
        private string superproxy = "";
        private string dnsResolution = "";
        private string targetUrl = "";
        private int fail_count;
        public int n_req_for_exit_node;
        public string session_id;
        public static Random rng = new Random();
        public string login;
        public CustomHttpWebRequest(ClientSettings settings)
        {
            userName = settings.UserName;
            passWord = settings.Password;
            zone = settings.Zone;
            port = settings.Port;
            country = settings.Country;
            device = settings.Device;
            route = settings.Route;
            superproxy = settings.SuperProxy;
            dnsResolution = settings.DNSResolution;
            targetUrl = settings.TargetUrl;
            Timeout = settings.Timeout;
            Method = settings.Method;
            Headers["User-Agent"] = settings.UserAgent;
            Proxy = settings.Proxy;
            Proxy.Credentials = settings.Credential;
        }

        protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }
        public void handle_response(WebException e = null)
        {
            if (e != null && should_switch_exit_node((HttpWebResponse)e.Response))
            {


                switch_session_id();
                fail_count++;


                return;
            }
            // success or other client/website error like 404...
            n_req_for_exit_node++;
            fail_count = 0;
        }
        public bool should_switch_exit_node(HttpWebResponse response)
        {
            return response == null ||
                status_code_requires_exit_node_switch((int)response.StatusCode);
        }
        public bool status_code_requires_exit_node_switch(int code)
        {
            return code == 403 || code == 429 || code == 502 || code == 503;
        }
        public void switch_session_id()
        {

            session_id = rng.Next().ToString();
            n_req_for_exit_node = 0;
            update_super_proxy();
        }
        public void update_super_proxy()
        {
            Proxy = new WebProxy("session-" + session_id + $"{superproxy}", port);
            login = userName + (country != null ? "-country-" + country : "")
                + $"{dnsResolution}" + session_id;
            Proxy.Credentials = new NetworkCredential(login, passWord);
        }

    }
}
