﻿using SeoClicker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeoClicker.Utils
{
    public class SeoClickerClient : WebClient
    {
        public string username = "lum-customer-thidoan-zone-fitechtimes2-route_err-pass_dyn";
        public static string password = "06g71bkbudiv";
        public int port = 22225;
        public static string user_agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
        public static int max_failures = 3;
        public static Random rng = new Random();
        public string session_id;
        public string login;
        public string country;
        public int fail_count;
        public int n_req_for_exit_node;
        public HashSet<ServicePoint> service_points;
        public string proxy_ip;

        public SeoClickerClient(string proxy_ip, ClientSettings settings, string country = null)
        {
            username = $"lum-customer-{settings.UserName}-zone-{settings.Zone}-route_err-{settings.Route}";
            password = settings.Password;
            port = settings.Port;

            this.country = country;
            this.proxy_ip = proxy_ip;
            service_points = new HashSet<ServicePoint>();
            switch_session_id();
        }

        public void switch_session_id()
        {
            clean_connection_pool();
            session_id = rng.Next().ToString();
            n_req_for_exit_node = 0;
            update_super_proxy();
        }

        public void update_super_proxy()
        {
            Proxy = new WebProxy("session-" + session_id + ".zproxy.lum-superproxy.io", port);
            login = username + (country != null ? "-country-" + country : "")
                + "-session-" + session_id;
            Proxy.Credentials = new NetworkCredential(login, password);
        }

        public void clean_connection_pool()
        {
            foreach (ServicePoint sp in service_points)
                sp.CloseConnectionGroup(login);
            service_points.Clear();
        }

        public bool have_good_super_proxy()
        {
            return fail_count < max_failures;
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

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.AllowAutoRedirect = false;

            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            request.UserAgent = user_agent;
            request.Method = "HEAD";
            request.ConnectionGroupName = login;
            request.PreAuthenticate = true;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            var uriString = response.Headers["Location"];
            ServicePoint sp = ((HttpWebRequest)request).ServicePoint;
            service_points.Add(sp);
            return response;
        }
    }
}