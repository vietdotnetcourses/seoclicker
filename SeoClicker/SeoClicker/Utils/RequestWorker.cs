using System;
using System.Net;
using System.Threading;
using SeoClicker.Models;
using System.Collections.Generic;
using System.Linq;
using SeoClicker.Helpers;
using System.Diagnostics;
using System.ComponentModel;

namespace SeoClicker.Utils
{
    public class RequestWorker : INotifyPropertyChanged
    {
        AsyncObservableCollection<ClickerThreadInfo> _threadInfos;
        AsyncObservableCollection<string> _logs;
        string _resultMessage;
        string _spinnerVisibility;
        bool _isEnabled;

        private int n_parallel_exit_nodes = 100;
        private int n_total_req = 1000;
        private int switch_ip_every_n_req = 1;
        private int at_req = 0;
        private string targetUrl;
        private int successCount = 0;
        private int failCount = 0;
        private Random rdm = new Random();
        private System.Diagnostics.Stopwatch timer = new Stopwatch();
        public ClientSettings ClientSettings { get; set; }
       // public DashBoardInfo DashBoardInfo { get; set; }

        private List<Thread> ThreadList = new List<Thread>();

        public RequestWorker()
        {
            ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
            Logs = new AsyncObservableCollection<string>();
            ResultMessage = "";
            SpinnerVisibility = "Hidden";
            IsEnabled = true;
        }
        public void ConfigureTask()
        {
            n_parallel_exit_nodes = ClientSettings.NumberOfThread;
            n_total_req = ClientSettings.RequestNumber;
            switch_ip_every_n_req = ClientSettings.IpChangeRequestNumber;
            targetUrl = ClientSettings.TargetUrl;

        }
        public void DoWork()
        {


            // start do the job
            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {
                var index = i;
                var thread = new Thread(new ThreadStart(Run));
                ThreadList.Add(thread);
                ThreadInfos.Add(new ClickerThreadInfo
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
            IsEnabled = true;
            SpinnerVisibility = "Hidden";
            foreach (var thread in ThreadList)
            {
                thread.Abort();

            }
            ThreadInfos.Clear();
        }
        public void Run()
        {
            var threadID = Thread.CurrentThread.ManagedThreadId;
            var info = ThreadInfos.FirstOrDefault(x => x.Id == threadID);
            if (info == null) return;

            while (Interlocked.Increment(ref at_req) <= n_total_req)
            {
                var sessionId = rdm.Next().ToString();
                var proxy = new WebProxy($"session-{sessionId}.{ClientSettings.SuperProxy}:{ClientSettings.Port}");
                var uriString = targetUrl;
                var resultStr = "";
                while (!string.IsNullOrEmpty(uriString))
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriString);
                    webRequest.Proxy = proxy;
                    webRequest.AllowAutoRedirect = false;  // IMPORTANT
                    webRequest.Timeout = ClientSettings.Timeout;
                    webRequest.KeepAlive = true;
                    webRequest.Method = "HEAD";
                    webRequest.UserAgent = UserAgentHelper.GetUserAgentByDevice(ClientSettings.Device);
                    webRequest.Proxy.Credentials = ClientSettings.Credential;
                    HttpWebResponse webResponse = null;
                    try
                    {
                        info.Id = threadID;
                        info.Url = uriString;
                        info.Info = "Running...";
                        info.Geo = ClientSettings.Country;
                        timer.Start();
                        using (webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                           
                            uriString = webResponse.Headers["Location"];
                          
                            webResponse.Close();
                            timer.Stop();

                            if (!string.IsNullOrEmpty(uriString))
                            {
                                Logs.Add($"Sent request to {uriString} successfully.");
                                resultStr += $"Url: {uriString} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Logs.Add($"Sent request to {uriString} failed. reason: {ex.Message}");
                      
                    }

                    if (Logs.Count >= 500) Logs.Clear();
                    if (successCount + failCount == n_total_req)
                    {
                        IsEnabled = true;
                        SpinnerVisibility = "Hidden";
                    }
                }
                DataHelper.SaveResult(resultStr, sessionId);
                successCount++;
                if (successCount + failCount == n_total_req)
                {
                    IsEnabled = true;
                    SpinnerVisibility = "Hidden";
                }
            }

            info.Info = "Ended.";
            ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
        }

        #region observable part

        public AsyncObservableCollection<ClickerThreadInfo> ThreadInfos
        {
            get
            {
                return _threadInfos;
            }
            set
            {
                _threadInfos = value;
                notifyPropertyChanged("ThreadInfos");
            }
        }

        public AsyncObservableCollection<string> Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
                notifyPropertyChanged("Logs");
            }
        }

        public string ResultMessage
        {
            get { return _resultMessage; }
            set
            {
                _resultMessage = value;
                notifyPropertyChanged("ResultMessage");
            }
        }
        public string SpinnerVisibility
        {
            get { return _spinnerVisibility; }
            set
            {
                _spinnerVisibility = value;
                notifyPropertyChanged("SpinnerVisibility");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                notifyPropertyChanged("IsEnabled");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
