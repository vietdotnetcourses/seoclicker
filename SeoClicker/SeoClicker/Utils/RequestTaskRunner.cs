using System;
using System.Net;
using System.Threading;
using SeoClicker.Models;
using System.Collections.Generic;
using System.Linq;
using SeoClicker.Helpers;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
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
        private int successCount = 0;
        private int failCount = 0;
        private Random rdm = new Random();
        private int _loadTime = 0;
        private int _loadCount = 0;

        private Stopwatch timer = new Stopwatch();
        public ClientSettings ClientSettings { get; set; }

        public static string super_proxy_ip;
        private Dictionary<int, int> TimeLoadList { get; set; }
        private List<MyTaskScheduler> TaskList { get; set; }
        private MyTaskScheduler DataFetcher { get; set; }
        public RequestTaskRunner()
        {
            ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
            Logs = "";
            ResultMessage = "";
            SpinnerVisibility = "Hidden";
            IsEnabled = true;
            TimeLoadList = new Dictionary<int, int>();
            DataFetcher = new MyTaskScheduler(0, 90);
            TaskList = new List<MyTaskScheduler>();
        }
        private async void FetchData()
        {

            var data = await DataHelper.FetchDataFromApi(ClientSettings.ApiDataUri, ClientSettings.Take);
            foreach (var item in data)
            {
                try
                {
                    DataHelper.WriteDataToFile(item);
                }
                catch { }

            }
            DataHelper.DeleteResultsFolder();
            Logs = "";

        }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken[] CancellationTokens { get; set; }
        public void Start()
        {
            ThreadInfos.Clear();
            TaskList.Clear();
            IsEnabled = false;
            DataFetcher.Start();
            DataFetcher.DoWork = () =>
            {
                FetchData();
            };


         
            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {

                var task = new MyTaskScheduler(0, 3000);
                task.DoWork = async () =>
                {
                    await Run();
                };
                task.Start();
                ThreadInfos.Add(new ClickerThreadInfo { Geo = "", Info = "running...", Id = task.Id, Order = i, Url = "" });
                TaskList.Add(task);

            }

        }
        public void ConfigureTask()
        {
            n_parallel_exit_nodes = ClientSettings.NumberOfThread;
            _loadTime = ClientSettings.LoadTime * 1000;
            _loadCount = ClientSettings.LoadCount;

        }
        public void Stop()
        {
            DataFetcher.Stop();
            IsEnabled = true;
            SpinnerVisibility = "Hidden";
            ThreadInfos.Clear();
        }
        public async Task Run()
        {
            var dataItem = DataHelper.GetDataItem();
            var info = ThreadInfos.FirstOrDefault(x => x.Id == Task.CurrentId);
            if (dataItem == null || (dataItem.SequenceID == Guid.Empty && dataItem.UserID == null))
            {
                if (info != null)
                {
                    info.Info = "No data...";
                    info.Geo = "";
                    info.Url = "No data";
                }

                Thread.Sleep(2000);
                return;
            }
            var preUri = "";
            if (info != null)
            {
                info.Id = Task.CurrentId.Value;
                info.Info = "Running...";
                info.Geo = dataItem.Country;
            }


            if (!string.IsNullOrWhiteSpace(dataItem.URL) && !string.IsNullOrWhiteSpace(dataItem.Country) && !string.IsNullOrWhiteSpace(dataItem.Device))
            {

                var sessionId = rdm.Next().ToString();
                var proxy = new WebProxy($"session-{sessionId}.zproxy.lum-superproxy.io:22225");

                var proxyCredential = new NetworkCredential($"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-country-{dataItem.Country}-session-{sessionId}", ClientSettings.Password);
                var uriString = dataItem.URL;

                var resultStr = "";


                var userAgent = UserAgentHelper.GetUserAgentByDevice(dataItem.Device);
                var resultList = new List<string>();
                var count = 0;
                var totalLoadTime = 0;
                while (!string.IsNullOrEmpty(uriString))

                {
                    if (count >= _loadCount)
                    {
                        break;
                    }

                    if (totalLoadTime >= _loadTime)
                    {
                        break;
                    }

                    if (uriString.StartsWith("https") || uriString.StartsWith("http") || uriString.StartsWith("/"))
                    {
                        if (uriString.StartsWith("/"))
                        {

                            Uri myUri = new Uri(preUri);
                            var protocal = "";
                            if (preUri.StartsWith("https"))
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
                        preUri = uriString;
                        resultList.Add(preUri);
                        HttpWebRequest webRequest;
                        try
                        {
                            webRequest = (HttpWebRequest)WebRequest.Create(uriString);
                        }
                        catch
                        {
                            resultStr += $"Url: {preUri} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
                            break;
                        }
                        webRequest.Proxy = proxy;
                        webRequest.Proxy.Credentials = proxyCredential;
                        webRequest.AllowAutoRedirect = false;  // IMPORTANT
                        webRequest.Timeout = ClientSettings.Timeout;
                        webRequest.KeepAlive = true;
                        webRequest.Method = "GET";
                        webRequest.ContentType = "text/html; charset=UTF8";
                        webRequest.UserAgent = userAgent;
                        webRequest.ConnectionGroupName = sessionId;

                        HttpWebResponse webResponse = null;
                        try
                        {
                            info.Url = uriString;

                            timer.Start();
                            using (webResponse = (HttpWebResponse)webRequest.GetResponseAsync().Result)
                            {

                                count++;
                                timer.Stop();
                                totalLoadTime += timer.Elapsed.Milliseconds;

                                Logs += $"Sent request to {uriString} successfully.{Environment.NewLine}";
                                resultStr += $"Url: {info.Url} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";

                                var statusCode = (int)webResponse.StatusCode;
                                if (statusCode > 300 && statusCode < 399)
                                {
                                    //redirecting
                                    uriString = webResponse.Headers["Location"];
                                }
                                else if (!string.IsNullOrWhiteSpace(webResponse.Headers["Refresh"]))
                                {
                                    //redirecting
                                    var refreshHeader = webResponse.Headers["Refresh"];
                                    var startUrl = refreshHeader.IndexOf("http");
                                    uriString = refreshHeader.Substring(startUrl, refreshHeader.Length - startUrl);
                                }

                                else
                                {
                                    Stream receiveStream = webResponse.GetResponseStream();
                                    StreamReader readStream = null;

                                    readStream = new StreamReader(receiveStream);

                                    var redirectUrl = "";

                                    string data = readStream.ReadToEnd();
                                    if (!string.IsNullOrWhiteSpace(data))
                                    {
                                        redirectUrl = RedirectLinkExtractor.GetRidirectUriFromContent(data);
                                    }

                                    if (!string.IsNullOrEmpty(redirectUrl))
                                    {
                                        uriString = redirectUrl;

                                    }

                                    if (uriString == preUri)
                                    {
                                        uriString = "";
                                    }

                                    readStream.Close();

                                }

                                if (RedirectLinkExtractor.IsEndingDomain(uriString) || IsRepeatedDomain(uriString, resultList, preUri))
                                {
                                    Logs += $"Sent request to {uriString} successfully.{Environment.NewLine}";
                                    resultStr += $"Url: {uriString} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
                                    uriString = "";
                                }
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
                            break;

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

        private bool IsRepeatedDomain(string url, List<string> resultList, string preUrl)
        {
            if (url.StartsWith("/"))
            {
                var preUri = new Uri(preUrl);
                if (preUri != null)
                {
                    var protocal = "";
                    if (preUrl.StartsWith("https"))
                    {
                        protocal = "https";
                    }
                    else
                    {
                        protocal = "http";
                    }
                    url = $"{protocal}://{preUri.Host}{url}";
                }

            }
            var result = false;

            for (var i = 0; i < resultList.Count - 1; i++)
            {
                if (resultList[i].Trim() == url.Trim()) return true;
            }
            return result;

        }

    }


}