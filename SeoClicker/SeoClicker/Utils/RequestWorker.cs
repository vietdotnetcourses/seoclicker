using System;
using System.Net;
using System.Threading;
using SeoClicker.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SeoClicker.Helpers;
using System.Threading.Tasks;

namespace SeoClicker.Utils
{
    public class RequestWorker
    {
        AsyncObservableCollection<ClickerThreadInfo> _threadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
        AsyncObservableCollection<string> _logs = new AsyncObservableCollection<string>();
        private int n_parallel_exit_nodes = 100;
        private int n_total_req = 1000;
        private int switch_ip_every_n_req = 20;
        private int at_req = 0;
        private string super_proxy_ip;
        private string targetUrl;
        public int successCount = 0;
        public int failCount = 0;

        private readonly ClientSettings ClientSettings;
        public DashBoardInfo DashBoardInfo { get; set; }

        private List<Thread> ThreadList = new List<Thread>();
        private List<ClickerClient> Clients = new List<ClickerClient>();
        public RequestWorker(int _n_parallel_exit_nodes, int _n_total_req, int _switch_ip_every_n_req, ClientSettings _ClientSettings, DashBoardInfo _DashBoardInfo)
        {
            ClientSettings = _ClientSettings;
            n_parallel_exit_nodes = _n_parallel_exit_nodes;
            n_total_req = _n_total_req;
            switch_ip_every_n_req = _switch_ip_every_n_req;
            targetUrl = ClientSettings.TargetUrl;
            DashBoardInfo = _DashBoardInfo;

        }
        public void DoWork()
        {

            string proxy_session_id = new Random().Next().ToString();
            IPHostEntry hostInfo = Dns.GetHostEntry("session-" + proxy_session_id + $"{ClientSettings.SuperProxy}");
            IPAddress[] address = hostInfo.AddressList;

            super_proxy_ip = address[0] + "";
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {
                var index = i;
                var thread = new Thread(new ThreadStart(Run));
                ThreadList.Add(thread);
                DashBoardInfo.ThreadInfos.Add(new ClickerThreadInfo
                {
                    Id = thread.ManagedThreadId,
                    Info = "started."
                });
                Thread.Sleep(500);
                thread.Start();

            }

        }
        public void Stop()
        {
            foreach (var thread in ThreadList)
            {
                thread.Abort();

            }
            foreach (var client in Clients)
            {
                client.clean_connection_pool();
                client.Dispose();
            }



        }
        public void Run()
        {
            var threadID = Thread.CurrentThread.ManagedThreadId;
            var info = DashBoardInfo.ThreadInfos.FirstOrDefault(x => x.Id == threadID);
            if (info == null) return;
            while (Interlocked.Increment(ref at_req) <= n_total_req)
            {
                var userNameString = $"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-route_err-{ClientSettings.Route}-country-{ClientSettings.Country}";
                var userAgent = UserAgentHelper.GetUserAgentByDevice(ClientSettings.Device);
                var uriString = targetUrl;
                while (!string.IsNullOrEmpty(uriString))
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriString);
                    webRequest.Proxy = new WebProxy("zproxy.lum-superproxy.io:22225");
                    webRequest.AllowAutoRedirect = false;  // IMPORTANT
                    webRequest.Timeout = ClientSettings.Timeout;
                    webRequest.KeepAlive = true;
                    webRequest.Method = "HEAD";
                    webRequest.UserAgent = userAgent;
                    webRequest.Proxy.Credentials = ClientSettings.Credential;
                    HttpWebResponse webResponse = null;
                    try
                    {
                        using (webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                           // if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                           // {
                                uriString = webResponse.Headers["Location"];
                            if (!string.IsNullOrWhiteSpace(uriString))
                            {
                                DashBoardInfo.Logs.Add($"Sent request to {uriString} sucessfully.");
                            }
                                
                            // }
                            webResponse.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        DashBoardInfo.Logs.Add($"Sent request to {uriString} failed. reason: {ex.Message}");
                        if (successCount + failCount == n_total_req)
                        {
                            DashBoardInfo.IsEnabled = true;
                            DashBoardInfo.SpinnerVisibility = "Hidden";
                        }
                    }

                    
                }
                successCount++;
                if (successCount + failCount == n_total_req)
                {
                    DashBoardInfo.IsEnabled = true;
                    DashBoardInfo.SpinnerVisibility = "Hidden";
                }
            }

            info.Info = "Ended.";
            DashBoardInfo.ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
        }
        //public void Run()
        //{

        //   // var client = new ClickerClient(super_proxy_ip, ClientSettings);
        //   // client.user_agent = UserAgentHelper.GetUserAgentByDevice(ClientSettings.Device);
        //    var threadID = Thread.CurrentThread.ManagedThreadId;
        //    var info = DashBoardInfo.ThreadInfos.FirstOrDefault(x => x.Id == threadID);
        //    if (info == null) return;
        //    while (Interlocked.Increment(ref at_req) <= n_total_req)
        //    {
        //        //Clients.Add(client);                       
        //        //info.Id = threadID;
        //        //info.Info = $"running";
        //        //info.Geo = client.country;
        //        //info.Url = targetUrl;
        //        //if (!client.have_good_super_proxy())
        //        //    client.switch_session_id();
        //        //if (client.n_req_for_exit_node == switch_ip_every_n_req)
        //        //    client.switch_session_id();
        //        try
        //        {
        //           // while()

        //            //var responseString = client.DownloadString(targetUrl);
        //            ////DashBoardInfo.Logs.Add(responseString);

        //            ////var response = RequestData(client,targetUrl);
        //            //client.handle_response();
        //            //DashBoardInfo.Logs.Add($"Sent request to {targetUrl} sucessfully.");
        //            //info.Info = $"succeeded.";
        //            //successCount++;
        //            //if(successCount + failCount == n_total_req)
        //            //{
        //            //    DashBoardInfo.IsEnabled = true;
        //            //    DashBoardInfo.SpinnerVisibility = "Hidden";
        //            //}
        //        }
        //        catch (WebException e)
        //        {

        //            info.Info = $"failed.";
        //            DashBoardInfo.Logs.Add($"Sent request to {targetUrl} failed. reason: {e.Message}");
        //            ExceptionLogger.LogExceptionToFile(e);
        //           // client.handle_response(e);
        //            failCount++;
        //            if (successCount + failCount == n_total_req)
        //            {
        //                DashBoardInfo.IsEnabled = true;
        //                DashBoardInfo.SpinnerVisibility = "Hidden";
        //            }
        //        }
        //    }
        //    info.Info = "Ended.";
        //   // client.clean_connection_pool();
        //   // client.Dispose();
        //    DashBoardInfo.ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";

        //}

        private async Task<string> RequestData(ClickerClient client, string uri)
        {
            string data = await client.DownloadStringTaskAsync(uri);
            return data;
        }

    }

}
