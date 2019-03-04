using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeoClicker.Utils
{
    public static class RedirectLinkExtractor
    {
        public static string GetRidirectUriFromContent(string responseString)
        {
            responseString = responseString.Replace("\"", "'");
            responseString = responseString.Replace("<meta http-equiv=\"Refresh\"", "<meta http-equiv='refresh'").Replace("<meta http-equiv= 'Refresh'", "<meta http-equiv='refresh'").Replace("<meta http-equiv='Refresh'", "<meta http-equiv='refresh'").Replace("<meta http-equiv ='Refresh'", "<meta http-equiv='refresh'").Replace("<meta http-equiv=\"refresh\"", "<meta http-equiv='refresh'");
            responseString = responseString.Replace("document.location.href", "window.location").Replace("window.location.href", "window.location").Replace("window.top.location", "window.location");
            responseString = responseString.Replace("window.location=\"", "window.location='").Replace("window.location= \"", "window.location='").Replace("window.location= '", "window.location='").Replace("window.location= ' ", "window.location='").Replace("window.location = '", "window.location='");
            string url = "";

            MatchCollection allUrls = new Regex("\\b(?:https?://|www\\.)\\S+\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled).Matches(responseString);
            if (allUrls.Count > 0)
            {
                for (int index = 0; index < allUrls.Count; ++index)
                {
                    if (RedirectLinkExtractor.IsEndingDomain(allUrls[index].Value))
                    {
                        url = allUrls[index].Value;
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(url))
            {

                MatchCollection matchCollection1 = new Regex("<meta http-equiv='refresh'[\\s\\S]*?>").Matches(responseString);
                if (matchCollection1.Count > 0)
                {
                    for (int index = 0; index < matchCollection1.Count; ++index)
                    {
                        if (matchCollection1[index].Value.Contains("https://"))
                        {
                            MatchCollection matchCollection2 = new Regex("https://[\\s\\S]*?>").Matches(matchCollection1[index].Value);
                            if (matchCollection2.Count > 0)
                                url = matchCollection2[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "").Replace("/>", "").Replace("\"", "").Replace("'", "").Trim();
                        }
                        if (matchCollection1[index].Value.Contains("http://"))
                        {
                            MatchCollection matchCollection2 = new Regex("http://[\\s\\S]*?>").Matches(matchCollection1[index].Value);
                            if (matchCollection2.Count > 0)
                                url = matchCollection2[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "").Replace("/>", "").Replace("\"", "").Replace("'", "").Trim();
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(url) || RedirectLinkExtractor.IsExcludedDomain(url))
                {
                    Regex regex1 = new Regex("<script[^>]*>[\\s\\S]*?</script>");
                    Regex regex2 = new Regex("window.location='(.*?)'");
                    Regex regex3 = new Regex("(.*?)");
                    string input1 = responseString;
                    MatchCollection matchCollection2 = regex1.Matches(input1);
                    for (int index = 0; index < matchCollection2.Count; ++index)
                    {
                        string input2 = matchCollection2[index].Value;
                        if (input2.Contains("window.location='"))
                            url = regex2.Matches(input2)[0].Value.Replace("window.location='", "").Replace("'", "").Replace("\"", "").Trim();
                    }
                }
                if (string.IsNullOrWhiteSpace(url) || RedirectLinkExtractor.IsExcludedDomain(url))
                {
                    MatchCollection matchCollection2 = new Regex("<meta content[\\s\\S]*?http-equiv='refresh'>").Matches(responseString);
                    if (matchCollection2.Count > 0)
                    {
                        for (int index = 0; index < matchCollection2.Count; ++index)
                        {
                            if (matchCollection2[index].Value.Contains("https://"))
                            {
                                MatchCollection matchCollection3 = new Regex("https://[\\s\\S]*?>").Matches(matchCollection2[index].Value);
                                if (matchCollection3.Count > 0)
                                    url = matchCollection3[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "").Replace("/>", "").Replace("\"", "").Replace("'", "").Trim();
                            }
                            if (matchCollection2[index].Value.Contains("http://"))
                            {
                                MatchCollection matchCollection3 = new Regex("http://[\\s\\S]*?>").Matches(matchCollection2[index].Value);
                                if (matchCollection3.Count > 0)
                                    url = matchCollection3[0].Value.Replace("'>", "").Replace("\">", "").Replace("\" >", "").Replace("'> ", "").Replace("/>", "").Replace("\"", "").Replace("'", "").Trim();
                            }
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(url) || RedirectLinkExtractor.IsExcludedDomain(url))
                {
                    MatchCollection matchCollection2 = new Regex("<a\\shref=.*?>.*?</a>").Matches(responseString);
                    if (matchCollection2.Count > 0)
                    {
                        for (int index = 0; index < matchCollection2.Count; ++index)
                        {
                            Match match = Regex.Match(matchCollection2[index].Value, "href=\\\"(.*?)\\\"", RegexOptions.Singleline);
                            if (match.Success && !RedirectLinkExtractor.IsExcludedDomain(match.Value))
                                url = match.Value.Replace("href=", "").Replace("\"", "").Replace("'", "").Trim();
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(url) || RedirectLinkExtractor.IsExcludedDomain(url))
                {
                    MatchCollection matchCollection2 = new Regex("\\b(?:https?://|www\\.)\\S+\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled).Matches(responseString);
                    if (matchCollection2.Count > 0)
                    {
                        for (int index = 0; index < matchCollection2.Count; ++index)
                        {
                            if (!RedirectLinkExtractor.IsExcludedDomain(matchCollection2[index].Value))
                            {
                                url = matchCollection2[index].Value;
                                break;
                            }
                        }
                    }
                }


            }





            return url;
        }

        public static bool IsEndingDomain(string url)
        {
            return File.ReadLines(Path.Combine(Application.StartupPath, "endingDomain.txt")).ToList().Any(x => url.Contains(x));
        }

        private static bool IsExcludedDomain(string url)
        {
            return File.ReadLines(Path.Combine(Application.StartupPath, "excludedHost.txt")).Any<string>((Func<string, bool>)(x => url.Contains(x)));
        }

        private static bool IsValidDomain(string url)
        {
            Uri result;
            if (!Uri.TryCreate(url, UriKind.Absolute, out result))
                return false;
            if (!(result.Scheme == Uri.UriSchemeHttp))
                return result.Scheme == Uri.UriSchemeHttps;
            return true;
        }
    }
}
