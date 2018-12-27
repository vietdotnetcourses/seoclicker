using System;
using System.Net;
using System.Threading;
using SeoClicker.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {
                DashBoardInfo.ThreadInfos.Add(new ClickerThreadInfo
                {
                    Order = i + 1,
                    Info = $"Thread {i + 1} created."
                });
            }
         
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
                var t = new Thread(new ThreadStart(Run));
                t.Name = "Thread " + i;
                t.Start();
                ThreadList.Add(t);
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

            var client = new ClickerClient(super_proxy_ip, ClientSettings);

            while (Interlocked.Increment(ref at_req) <= n_total_req)
            {
                Clients.Add(client);
                var info = DashBoardInfo.ThreadInfos.FirstOrDefault(x => x.Order == at_req + 1);
                if (info == null) return;
                info.Info = $"Thread {at_req + 1} running.";
                info.Geo = client.country;
                info.Url = targetUrl;
                if (!client.have_good_super_proxy())
                    client.switch_session_id();
                if (client.n_req_for_exit_node == switch_ip_every_n_req)
                    client.switch_session_id();
                try
                {

                    client.DownloadString(targetUrl);
                    client.handle_response();
                    info.Info = $"Thread {at_req + 1} succeeded.";
                }
                catch (WebException e)
                {
                   
                    info.Info = $"Thread {at_req + 1} failed.";
                    DashBoardInfo.Logs.Add(e.Message);
                    ExceptionLogger.LogExceptionToFile(e);
                    client.handle_response(e);
                }
            }

            client.clean_connection_pool();
            client.Dispose();


        }

    }
}
