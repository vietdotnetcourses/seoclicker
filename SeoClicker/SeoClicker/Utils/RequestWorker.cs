using System;
using System.Net;
using System.Threading;
using SeoClicker.Models;
using System.Collections.Generic;
using System.Linq;
using SeoClicker.Helpers;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SeoClicker.Utils
{
    public class RequestWorker : INotifyPropertyChanged
    {
        ObservableCollection<ClickerThreadInfo> _threadInfos;
        string _logs;
        string _resultMessage;
        string _spinnerVisibility;
        bool _isEnabled;

        private int n_parallel_exit_nodes = 5;
        private int n_total_req = 1000;
        private int switch_ip_every_n_req = 1;
        private int at_req = 0;
        private string targetUrl;
        private int successCount = 0;
        private int failCount = 0;
        private Random rdm = new Random();
        private System.Diagnostics.Stopwatch timer = new Stopwatch();
        public ClientSettings ClientSettings { get; set; }
        private List<Thread> ThreadList = new List<Thread>();
        private List<Guid> ProcessedUrls { get; set; }
        private List<SequenceUrl> Data { get; set; }
        private bool IsRunning { get; set; }
        public static string super_proxy_ip;
       
        public RequestWorker()
        {
            ThreadInfos = new ObservableCollection<ClickerThreadInfo>();
            Logs = "";
            ResultMessage = "";
            SpinnerVisibility = "Hidden";
            IsEnabled = true;
            IsRunning = false;
            Data = new List<SequenceUrl>();
        }
        public void ConfigureTask()
        {
            n_parallel_exit_nodes = ClientSettings.NumberOfThread;
            n_total_req = ClientSettings.RequestNumber;
            switch_ip_every_n_req = ClientSettings.IpChangeRequestNumber;
            targetUrl = ClientSettings.TargetUrl;
            //ProcessedUrls = new List<Guid>();
        }
     
        public void DoWork()
        {

           
            Data = DataHelper.FetchDataFromApi(ClientSettings.ApiDataUri, ClientSettings.Take);
            if(Data != null)
            {
                if (Data.Any())
                {
                    IsRunning = true;
                    IsEnabled = false;
                    Logs = "";
                    ProcessedUrls = new List<Guid>();
                    at_req = 0;
                    n_total_req = Data.Count();
                    successCount = 0;
                    ThreadList.Clear();
                    failCount = 0;
                    DispatcherHelper.DispatchAction(() =>
                    {
                        ThreadInfos.Clear();

                    });


                    // start do the job
                    ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                    for (var i = 0; i < n_parallel_exit_nodes; i++)
                    {
                        var index = i;
                        var thread = new Thread(new ThreadStart(Run));
                        thread.Name = $"Thread {thread.ManagedThreadId}";
                        ThreadList.Add(thread);
                        DispatcherHelper.DispatchAction(() =>
                        {
                            ThreadInfos.Add(new ClickerThreadInfo
                            {
                                Id = thread.ManagedThreadId,
                                Info = "started."
                            });

                        });



                        thread.Start();
                        Thread.Sleep(1000);
                    }
                }
            }
          
            else
            {
                Thread.Sleep(3000);
                DoWork();
            }

        }

        public void FetchData()
        {
            Data = DataHelper.FetchDataFromApi(ClientSettings.ApiDataUri, ClientSettings.Take);
            if (Data != null)
            {
                if (Data.Any())
                {
                    IsRunning = true;
                    IsEnabled = false;
                    Logs = "";
                    ProcessedUrls = new List<Guid>();
                    at_req = 0;
                    n_total_req = Data.Count();
                    successCount = 0;
                    ThreadList.Clear();
                    failCount = 0;
                 


                }
            }

            else
            {
                Thread.Sleep(3000);
                FetchData();
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
            successCount = 0;
            failCount = 0;
            IsRunning = false;
            ThreadList.Clear();
        }
        public void Run()
        {

            var threadID = Thread.CurrentThread.ManagedThreadId;
            var info = new ClickerThreadInfo();
            DispatcherHelper.DispatchAction(() =>
            {
                info = ThreadInfos.FirstOrDefault(x => x.Id == threadID);
            });
            
            if (info == null) return;

            while (Interlocked.Increment(ref at_req) <= n_total_req)
            {
                if (!Data.Any()) return;
                Thread.Sleep(500);
                if (at_req >= n_total_req)
                {

                    break ;
                }
                var dataItem = new SequenceUrl();

                lock (Data)
                {
                    dataItem = Data[at_req];
                }

              


                var sessionId = rdm.Next().ToString();
                var proxy = new WebProxy($"session-{sessionId}{ClientSettings.SuperProxy}.zproxy.lum-superproxy.io:{ClientSettings.Port}");
                var proxyCredential = new NetworkCredential($"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-route_err-{ClientSettings.Route}-country-{dataItem.Country}-session-{sessionId}{ClientSettings.DNSResolution}", ClientSettings.Password);
                var uriString = dataItem.URL;

                // var uriString = targetUrl;
                var resultStr = "";
                info.Id = threadID;
                info.Info = "Running...";
                // info.Geo = ClientSettings.Country;
                info.Geo = dataItem.Country;

                while (!string.IsNullOrEmpty(uriString))

                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriString);
                    webRequest.Proxy = proxy;
                    webRequest.Proxy.Credentials = proxyCredential;
                    webRequest.AllowAutoRedirect = false;  // IMPORTANT
                    webRequest.Timeout = ClientSettings.Timeout;
                    webRequest.KeepAlive = false;
                    webRequest.Method = "GET";
                    webRequest.ContentType = "text/html; charset=UTF8";
                    webRequest.UserAgent = UserAgentHelper.GetUserAgentByDevice(dataItem.Device);
                    webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
                    HttpWebResponse webResponse = null;
                    try
                    {
                        info.Url = uriString;

                        timer.Start();

                        using (webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {


                            timer.Stop();
                            if (!string.IsNullOrEmpty(uriString))
                            {
                                Logs += $"Sent request to {uriString} successfully.{Environment.NewLine}";
                                resultStr += $"Url: {uriString} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
                            }

                            uriString = webResponse.Headers["Location"];


                            if (string.IsNullOrEmpty(uriString))
                            {

                                if (webResponse.StatusCode == HttpStatusCode.OK)
                                {


                                    Stream receiveStream = webResponse.GetResponseStream();
                                    StreamReader readStream = null;

                                    if (webResponse.CharacterSet == null)
                                    {
                                        readStream = new StreamReader(receiveStream);
                                    }
                                    else
                                    {
                                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(webResponse.CharacterSet));
                                    }

                                    string data = readStream.ReadToEnd();
                                    var redirectUrl = GetRidirectUriFromContent(data);


                                    if (!string.IsNullOrEmpty(redirectUrl))
                                    {
                                        uriString = redirectUrl;
                                    }
                                    webResponse.Close();
                                    readStream.Close();
                                }

                            }
                            webResponse.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Logs += $"Sent request to {uriString} failed. reason: {ex.Message}{Environment.NewLine}";
                        ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
                        if (successCount + failCount >= n_total_req)
                        {
                            IsEnabled = true;
                            SpinnerVisibility = "Hidden";
                            info.Info = "Ended.";
                            //Thread.CurrentThread.Abort();
                        }

                    }

                }
               // ProcessedUrls.Add(dataItem.SequenceID);

                DataHelper.SaveResult(resultStr, sessionId);
                successCount++;
              
                ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
                if (successCount + failCount >= n_total_req)
                {
                    FetchData();

                }


               // if (ThreadList.Where(x => x.IsAlive).Count() == 1)
               // {
               //this means completed all thread
               //   DoWork();
               // }
               // if (successCount + failCount >= n_total_req)
               // {
               //   IsEnabled = true;
               //   SpinnerVisibility = "Hidden";
               //   info.Info = "Ended.";
               // Thread.CurrentThread.Abort();
               // }


            }

            ResultMessage = $"Succeeded: {successCount}  Failed: {n_total_req -successCount}";
        }

        #region observable part

        public ObservableCollection<ClickerThreadInfo> ThreadInfos
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

        public string Logs
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
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private string GetRidirectUriFromContent(string responseString)
        {
            var result = "";
            var regex = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            var childRegex1 = new Regex(@"'(.*?)'");
            var childRegex2 = new Regex(@"(.*?)");
            var matches = regex.Matches(responseString);
            for (var i = 0; i < matches.Count; i++)
            {
                var childString = matches[i].Value;
                if (childString.Contains("document.location.href") || childString.Contains("window.location.href") || childString.Contains("window.location"))
                {
                    var childMatches = childRegex1.Matches(childString);
                    for (var j = 0; j < childMatches.Count; j++)
                    {
                        if (childMatches[j].Value.Contains("http"))
                        {
                            result = childMatches[j].Value.Replace("'", "");
                        }
                    }
                }

            }

            return result;
        }
    }

}
