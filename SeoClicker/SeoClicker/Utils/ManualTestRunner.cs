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
    public class ManualTestRunner
    {
        public ClientSettings ClientSettings { get; set; }
        private int _loadCount;
        private int _loadTime;
        private Stopwatch timer = new Stopwatch();
        private Random rdm = new Random();
 
        public ManualTestRunner Setup(ClientSettings clientSetting)
        {
            ClientSettings = clientSetting;
            _loadCount = ClientSettings.LoadCount;
            _loadTime = ClientSettings.LoadTime;
            return this;
        }
        public void MakeRequest(ref ManualTestModel testmodel)
        {
            testmodel.Log = "";
            var preUri = "";

            if (!string.IsNullOrWhiteSpace(testmodel.Url) && !string.IsNullOrWhiteSpace(testmodel.Geo) && !string.IsNullOrWhiteSpace(testmodel.Device))
            {
                var sessionId = rdm.Next().ToString();
                //OxyLabs
                //var proxy = new WebProxy($"pr.oxylabs.io:7777");
                //var login = $"customer-{ClientSettings.UserName}-cc-{detail.Country}-sessid-{sessionId}";
                //var proxyCredential = new NetworkCredential(login, ClientSettings.Password);

                //Luminati
                var proxy = new WebProxy($"session-{sessionId}.zproxy.lum-superproxy.io:22225");
                var proxyCredential = new NetworkCredential($"lum-customer-{ClientSettings.UserName}-zone-{ClientSettings.Zone}-country-{testmodel.Geo}-session-{sessionId}", ClientSettings.Password);
                var uriString = testmodel.Url;

                var resultStr = "";
                var userAgent = UserAgentHelper.GetUserAgentByDevice(testmodel.Device);
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
                        catch (Exception ex)
                        {

                            testmodel.Log += $"Error: {ex.Message}";
                            break;

                        }
                        
                        webRequest.Proxy = proxy;
                        webRequest.Proxy.Credentials = proxyCredential;
                        webRequest.AllowAutoRedirect = false;  // IMPORTANT
                        webRequest.Timeout = ClientSettings.LoadTime > 0? ClientSettings.LoadTime : 15;
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

                            HttpWebResponse webResponse;
                            using (webResponse = (HttpWebResponse)webRequest.GetResponseAsync().Result)
                            {

                                count++;
                                timer.Stop();
                                totalLoadTime += timer.Elapsed.Milliseconds;
                                testmodel.Log += $"Sent request to {uriString} successfully.{Environment.NewLine}";
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
                                    resultStr += $"Url: {uriString} -- Load time : {timer.Elapsed.Milliseconds} miliseconds{Environment.NewLine}";
                                    uriString = "";
                                }
                            }

                            if (string.IsNullOrWhiteSpace(uriString))
                            {
                                testmodel.Log += "Stopped";
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            uriString = "";
                            testmodel.Log += $"Sent request to {uriString} failed. reason: {ex.Message}{Environment.NewLine}";
                        }

                    }
                    else
                    {
                        testmodel.Log += "Stopped";
                        uriString = "";
                    }

                }

            }
            else
            {
                testmodel.Log += $"Input data is invalid{Environment.NewLine}";
            }

        }
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


