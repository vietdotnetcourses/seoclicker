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
using System.Threading.Tasks;

namespace SeoClicker.Utils
{
    public class RequestTaskRunner : INotifyPropertyChanged
    {
        AsyncObservableCollection<ClickerThreadInfo> _threadInfos;
        string _logs;
        string _resultMessage;
        string _spinnerVisibility;
        bool _isEnabled;

        private int n_parallel_exit_nodes = 0;
        private int n_total_req = 0;
        private int switch_ip_every_n_req = 1;
        private int at_req = 0;
        private string targetUrl;
        private int successCount = 0;
        private int failCount = 0;
        private Random rdm = new Random();
        private int _loadTime = 0;

        private System.Diagnostics.Stopwatch timer = new Stopwatch();
        public ClientSettings ClientSettings { get; set; }
        private Queue<SequenceUrl> Data { get; set; }
        private bool IsRunning { get; set; }
        public static string super_proxy_ip;
        private Dictionary<int, int> TimeLoadList { get; set; }
        private SimpleTaskScheduler taskScheduler { get; set; }

        private List<Task> TaskList { get; set; }
        private MyTaskScheduler DataFetcher { get; set; }
        public RequestTaskRunner()
        {
            ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
            Logs = "";
            ResultMessage = "";
            SpinnerVisibility = "Hidden";
            IsEnabled = true;
            IsRunning = false;
            Data = new Queue<SequenceUrl>();
            TimeLoadList = new Dictionary<int, int>();
            DataFetcher = new MyTaskScheduler(0, 50);
            TaskList = new List<Task>();
          
 
        }
        private void FetchData()
        {
            if (!Data.Any())
            {
                var data = DataHelper.FetchDataFromApi(ClientSettings.ApiDataUri, ClientSettings.Take);
                foreach (var item in data)
                {
                    Data.Enqueue(item);
                }
                foreach(var task in TaskList)
                {
                    //if(task.)
                    
                }
                n_total_req = Data.Count;
            }
        }

        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken[] CancellationTokens { get; set; }
        public void Start()
        {
            IsEnabled = false;
            DataFetcher.Start();
            DataFetcher.DoWork = () =>
            {
                FetchData();
            };

          

            //// create multiple tasks to run paralelly

            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {

                var task = Task.Factory.StartNew(async() =>
                {
                   
                    while (true)
                    {
                        
                       Run();
                        
                    }
                }, CancellationTokens[i], TaskCreationOptions.LongRunning, TaskScheduler.Default);
                ThreadInfos.Add(new ClickerThreadInfo { Geo = "", Info = "Started.", Id = task.Id, Order = i, Url = "" });
                TaskList.Add(task);

            }

        }
        public void ConfigureTask()
        {
            n_parallel_exit_nodes = ClientSettings.NumberOfThread;
            switch_ip_every_n_req = ClientSettings.IpChangeRequestNumber;
            targetUrl = ClientSettings.TargetUrl;
            _loadTime = ClientSettings.LoadTime * 1000;
            CancellationTokens = new CancellationToken[n_parallel_exit_nodes];
            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationTokens);
        }


        public void Stop()
        {
            DataFetcher.Stop();

            IsEnabled = true;
            SpinnerVisibility = "Hidden";
            ThreadInfos.Clear();

            IsRunning = false;
            CancellationTokenSource.Cancel();
        }
        public bool Run()
        {

            bool check = false;
            SequenceUrl dataItem = new SequenceUrl();
            var info = new ClickerThreadInfo();
            lock (ThreadInfos)
            {
                info = ThreadInfos.FirstOrDefault(x => x.Id == Task.CurrentId);
            }
            lock (Data)
            {
                if (Data.Any())
                {

                    check = true;
                    dataItem = Data.FirstOrDefault();
                }
            }


            if (!check & info == null)
            {
                return false;
            }
            if (dataItem == null)
            {
                return false;
            }




            if (!string.IsNullOrWhiteSpace(dataItem.URL) && !string.IsNullOrWhiteSpace(dataItem.Country) && !string.IsNullOrWhiteSpace(dataItem.Device))
            {
                Data.Dequeue();
                var sessionId = rdm.Next().ToString();
                var proxy = new WebProxy($"session-{sessionId}.zproxy.lum-superproxy.io:{ClientSettings.Port}");
                var proxyCredential = new NetworkCredential($"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-route_err-{ClientSettings.Route}-country-{dataItem.Country}-session-{sessionId}", ClientSettings.Password);
                var uriString = dataItem.URL;

                var resultStr = "";
                info.Id = Task.CurrentId.Value;
                info.Info = "Running...";
                info.Geo = dataItem.Country;

                while (!string.IsNullOrEmpty(uriString))

                {
                    if (uriString.StartsWith("https") || uriString.StartsWith("http") || uriString.StartsWith("/"))
                    {
                        if (uriString.StartsWith("/"))
                        {
                            Uri myUri = new Uri(info.Url);
                            var protocal = "";
                            if (info.Url.StartsWith("https"))
                            {
                                protocal = "https";
                            }
                            else
                            {
                                protocal = "http";
                            }
                            var host = myUri.Host;

                            uriString = $"{protocal}://{host}{uriString}";
                        }
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriString);
                        webRequest.Proxy = proxy;
                        webRequest.Proxy.Credentials = proxyCredential;
                        webRequest.AllowAutoRedirect = false;  // IMPORTANT
                        webRequest.Timeout = ClientSettings.Timeout;
                        webRequest.KeepAlive = true;
                        webRequest.Method = "GET";
                        webRequest.ContentType = "text/html; charset=UTF8";
                        webRequest.UserAgent = UserAgentHelper.GetUserAgentByDevice(dataItem.Device);
                        webRequest.Headers.Add("Accept-Language", $"{dataItem.Country}");


                        HttpWebResponse webResponse = null;
                        try
                        {
                            info.Url = uriString;

                            timer.Start();

                            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
                            {


                                timer.Stop();
                                var loadtime = timer.Elapsed.Milliseconds;

                                Logs += $"Sent request to {uriString} successfully.{Environment.NewLine}";
                                resultStr += $"Url: {info.Url} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
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
                                else if (webResponse.StatusCode == HttpStatusCode.Moved || webResponse.StatusCode == HttpStatusCode.MovedPermanently)
                                {
                                    uriString = "";
                                }

                                webResponse.Close();
                            }

                            if (string.IsNullOrWhiteSpace(uriString))
                            {
                                Interlocked.Increment(ref successCount);
                                DataHelper.SaveResult(resultStr, sessionId);

                                ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";

                            }

                        }
                        catch (Exception ex)
                        {
                            if (webResponse == null)
                            {
                                Interlocked.Increment(ref successCount);
                            }
                            else
                            {
                                Interlocked.Increment(ref failCount);
                            }


                            uriString = "";
                            Logs += $"Sent request to {uriString} failed. reason: {ex.Message}{Environment.NewLine}";
                            ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";


                        }

                    }
                    else
                    {
                        Interlocked.Increment(ref successCount);
                        ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
                        uriString = "";

                    }

                }

            }
            return true;



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


        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
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

            //get redirect url from meta tag

            var metaRegex = new Regex(@"<meta http-equiv='refresh'[\s\S]*?>");
            var metaMatches = metaRegex.Matches(responseString);
            if (metaMatches.Count > 0)
            {
                for (var j = 0; j < metaMatches.Count; j++)
                {
                    if (metaMatches[j].Value.Contains("https://"))
                    {
                        var httpsmetachildRegex = new Regex(@"https://[\s\S]*?>");
                        var httpsMatches = httpsmetachildRegex.Matches(metaMatches[j].Value);
                        if (httpsMatches.Count > 0)
                        {
                            result = httpsMatches[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "");
                        }

                    }
                    if (metaMatches[j].Value.Contains("http://"))
                    {
                        var httpmetachildRegex = new Regex(@"http://[\s\S]*?>");
                        var httpMatches = httpmetachildRegex.Matches(metaMatches[j].Value);
                        if (httpMatches.Count > 0)
                        {
                            result = httpMatches[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "");
                        }

                    }
                }
            }
            return result;
        }
    }


}