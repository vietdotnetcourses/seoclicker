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
using System.Collections.Concurrent;

namespace SeoClicker.Utils
{
    public class RequestTaskRunner : INotifyPropertyChanged
    {
        AsyncObservableCollection<ClickerThreadInfo> _threadInfos;
        string _logs;
        string _resultMessage;
        string _spinnerVisibility;
        bool _isEnabled;
        private CancellationTokenSource TokenSource { get; set; }
        private int n_parallel_exit_nodes = 0;
        private int successCount = 0;
        private int failCount = 0;
        private Random rdm = new Random();
        private int _loadTime = 0;
        private int _loadCount = 0;

        private Stopwatch timer = new Stopwatch();
        public ClientSettings ClientSettings { get; set; }

        public static string super_proxy_ip;
        private static ConcurrentBag<Task> TaskList { get; set; }
        private MyTaskScheduler DataFetcher { get; set; }
        public RequestTaskRunner()
        {
            ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
            Logs = "";
            ResultMessage = "";
            SpinnerVisibility = "Hidden";
            IsEnabled = true;
            DataFetcher = new MyTaskScheduler(0, 30);
            TaskList = new ConcurrentBag<Task>();
            TokenSource = new CancellationTokenSource();
        }

        public void ClearTaskList()
        {
            while (!TaskList.IsEmpty)
            {
                TaskList.TryTake(out _);
            }
        }

        public void Start()
        {
            TokenSource.Dispose();
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            ThreadInfos.Clear();
            ClearTaskList();

            IsEnabled = false;
            DataFetcher.Start();
            DataFetcher.DoWork = async () =>
            {
                await FetchData();
            };


            for (var i = 0; i < n_parallel_exit_nodes; i++)
            {
                var task = Task.Factory.StartNew(() => DoWork(token), token);
                ThreadInfos.Add(new ClickerThreadInfo { Geo = "", Info = Constants.TaskStatus.STARTING, Id = task.Id, Order = i, Url = "" });
            }

        }


        public RequestTaskRunner ConfigureTask()
        {
            n_parallel_exit_nodes = ClientSettings.NumberOfThread;
            _loadTime = ClientSettings.LoadTime * 1000;
            _loadCount = ClientSettings.LoadCount;
            return this;
        }
        public void Stop()
        {
            DataFetcher.Stop();
            IsEnabled = true;
            SpinnerVisibility = "Hidden";
            ThreadInfos.Clear();
            TokenSource.Cancel();
        }

        private void DoWork(CancellationToken ct)
        {
            var currentTaskId = Task.CurrentId;
            if (ct.IsCancellationRequested)
            {
                ct.ThrowIfCancellationRequested();
            }


            try
            {
                while (true)
                {

                    ct.ThrowIfCancellationRequested();

                    var requestDetails = DataHelper.GetDataItem();
                    if (currentTaskId != null && requestDetails != null)
                    {
                        SetTaskInfo(currentTaskId, requestDetails.URL, Constants.TaskStatus.STARTED, requestDetails.Country);
                        MakeRequest(requestDetails, currentTaskId);
                    }


                }

            }
            catch (Exception ex)
            {
            }

        }

        private void SetTaskInfo(int? id, string url = "", string info = "", string geo = "")
        {
            var infoItem = ThreadInfos.FirstOrDefault(x => x.Id == id);
            if (infoItem == null) return;
            if (!string.IsNullOrWhiteSpace(url))
            {
                infoItem.Url = url;
            }
            if (!string.IsNullOrWhiteSpace(info))
            {
                infoItem.Info = info;
            }
            if (!string.IsNullOrWhiteSpace(geo))
            {
                infoItem.Geo = geo;
            }
        }

        private void MakeRequest(SequenceUrl detail, int? currentTaskId)
        {
            SetTaskInfo(currentTaskId, detail.URL, Constants.TaskStatus.RUNNING, detail.Country);
            var preUri = "";
            if (!string.IsNullOrWhiteSpace(detail.URL) && !string.IsNullOrWhiteSpace(detail.Country) && !string.IsNullOrWhiteSpace(detail.Device))
            {
                var sessionId = rdm.Next().ToString();
                //OxyLabs
                //var proxy = new WebProxy($"pr.oxylabs.io:7777");
                //var login = $"customer-{ClientSettings.UserName}-cc-{detail.Country}-sessid-{sessionId}";
                //var proxyCredential = new NetworkCredential(login, ClientSettings.Password);

                //Luminati
                var proxy = new WebProxy($"session-{sessionId}.zproxy.lum-superproxy.io:22225");
                var proxyCredential = new NetworkCredential($"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-country-{detail.Country}-session-{sessionId}", ClientSettings.Password);
                var uriString = detail.URL;

                var resultStr = "";
                var userAgent = UserAgentHelper.GetUserAgentByDevice(detail.Device);
                var resultList = new List<string>();
                var count = 0;
                var totalLoadTime = 0;
                CookieCollection cookies = null;
                while (!string.IsNullOrEmpty(uriString))

                {
                    if (count >= _loadCount)
                    {
                        break;
                    }

                    if (totalLoadTime >= _loadTime)
                    {

                        Interlocked.Increment(ref successCount);
                        DataHelper.SaveResult(resultStr, sessionId);
                        ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
                        break;
                    }

                    if (uriString.StartsWith("https") || uriString.StartsWith("http") || uriString.StartsWith("/"))
                    {
                        if (uriString.StartsWith("/"))
                        {

                            Uri myUri = new Uri(preUri);
                            string protocal;
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
                            Interlocked.Increment(ref successCount);
                            DataHelper.SaveResult(resultStr, sessionId);
                            ResultMessage = $"Succeeded: {successCount}  Failed: {failCount}";
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

                        //add cookies if any
                        if (cookies != null)
                        {
                            if (webRequest.CookieContainer == null)
                            {
                                webRequest.CookieContainer = new CookieContainer();
                            }

                            webRequest.CookieContainer.Add(cookies);
                        }


                        try
                        {
                            SetTaskInfo(currentTaskId, url: uriString);
                            timer.Start();
                            HttpWebResponse webResponse;
                            using (webResponse = (HttpWebResponse)webRequest.GetResponseAsync().Result)
                            {

                                count++;
                                timer.Stop();
                                totalLoadTime += timer.Elapsed.Milliseconds;
                                Logs += $"Sent request to {uriString} successfully.{Environment.NewLine}";
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

                                    cookies = webResponse.Cookies;

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
                                    Interlocked.Increment(ref successCount);
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
                                break;
                            }

                        }
                        catch (Exception ex)
                        {

                            Interlocked.Increment(ref failCount);
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
                        break;
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
        private async Task FetchData()
        {

            var data = await DataHelper.FetchDataFromApi(ClientSettings.ApiDataUri, ClientSettings.Take);
            if (data != null)
            {
                foreach (var item in data)
                {
                    try
                    {
                        DataHelper.WriteDataToFile(item);
                    }
                    catch
                    { }

                }

                DataHelper.DeleteResultsFolder();
                Logs = "";
            }
            else
            {
                Logs += $"{Environment.NewLine}{Constants.TaskStatus.NODATA}";

            }


        }
    }


}