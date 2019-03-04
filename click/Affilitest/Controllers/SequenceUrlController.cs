using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Affilitest.Models;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Hosting;
using System.Configuration;

namespace Affilitest.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SequenceUrlController : Controller
    {
        private AffilitestdbEntities db = new AffilitestdbEntities();

        public static string tokenLogin { get; set; }

        public ActionResult Index(int size = 0)
        {
            var pageSize = 20;
            var hasLoadMore = true;
            var listSequenceUrl = db.SequenceUrls.OrderByDescending(s => s.Date).ToList();
            var numberOfUrl = listSequenceUrl.Count;
            ViewBag.numberOfUrl = numberOfUrl;
            var result = new List<SequenceUrl>();
            if (size == 0)
            {
                if (numberOfUrl <= pageSize)
                {
                    result = listSequenceUrl;
                    hasLoadMore = false;
                }
                else
                {
                    result = listSequenceUrl.Take(pageSize).ToList();
                }
            }
            else if (size > 0)
            {

                if (numberOfUrl > size)
                {
                    var remainningUrl = numberOfUrl - size;
                    if (remainningUrl > pageSize)
                    {
                        int total = (pageSize + size);
                        result = listSequenceUrl.Take(total).ToList();
                    }
                    else
                    {
                        result = listSequenceUrl;
                        hasLoadMore = false;
                    }
                }
                else
                {
                    result = listSequenceUrl;
                    hasLoadMore = false;
                }
            }

            ViewBag.hasLoadMore = hasLoadMore;
            return View(result);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SequenceUrl sequenceurl = db.SequenceUrls.Find(id);
            if (sequenceurl == null)
            {
                return HttpNotFound();
            }
            return View(sequenceurl);
        }


        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SequenceUrl sequenceurl = db.SequenceUrls.Find(id);
            if (sequenceurl == null)
            {
                return HttpNotFound();
            }
            return View(sequenceurl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SequenceID,URL,Device,Country,UserID")] SequenceUrl sequenceurl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sequenceurl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sequenceurl);
        }


        public ActionResult Delete(Guid id)
        {
            SequenceUrl sequenceurl = db.SequenceUrls.Find(id);
            db.SequenceUrls.Remove(sequenceurl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            List<SequenceUrl> sequenceurls = db.SequenceUrls.ToList();
            foreach (var s in sequenceurls)
            {
                db.SequenceUrls.Remove(s);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendRequestToAPI()
        {
            DataResult dtr = new DataResult();
            dtr.Status = Status.Success;
            var listAccount = GetAffilitestAccount();
            var recordsNeeding = 500;
            var records = db.SequenceUrls.OrderByDescending(s => s.Date).Take(recordsNeeding).ToList();
            ProxyRequestPostData(records, listAccount);
            return Json(dtr);

        }



        public void ProxyRequestPostData(List<SequenceUrl> lstu, List<AffilitestAccount> lsta)
        {
            if (lstu != null && lstu.Count > 0)
            {

                var _35Records = lstu; if (_35Records.Count > 0)
                {
                    foreach (var sequenceUrl in _35Records)
                    {
                        try
                        {

                            new Thread(new ThreadStart(() =>
                            {
                                PostUrlAddRegularCheck(sequenceUrl.URL, sequenceUrl.Country, sequenceUrl.Device, tokenLogin);
                            })).Start();

                            var sequenceUrlNeedRemove = db.SequenceUrls.Where(s => s.SequenceID.Equals(sequenceUrl.SequenceID)).FirstOrDefault();
                            if (sequenceUrlNeedRemove != null)
                            {
                                db.SequenceUrls.Remove(sequenceUrlNeedRemove);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("out=======" + ex.Message);
                            break;
                        }
                    }
                }
                try
                {
                }
                catch
                {

                }

                try
                {

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                }
            }

        }

        private List<AffilitestAccount> GetAffilitestAccount()
        {
            var listAccount = db.AffilitestAccounts.Where(a => a.Active.HasValue && a.Active.Value == true && a.Number.HasValue).OrderBy(a => a.Number).ToList();
            return listAccount;
        }

        public void PostUrlAddRegularCheck(string url, string country, string device, string token = "")
        {
            Uri target = new Uri("https://api.offertest.net/offertest?async=true");

            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(target);
            wr.Method = "POST";

            var authToken = System.Configuration.ConfigurationManager.AppSettings["offertestAuthToken"];
            var userId = ConfigurationManager.AppSettings["offerTestUserid"];
            wr.ContentType = "application/json";
            wr.Headers["Authorization"] = authToken;
            wr.Timeout = Timeout.Infinite;         
            var deviceArray = device.Split('-');
            if (string.IsNullOrWhiteSpace(deviceArray[0]))deviceArray[0] = "ios";
            if (string.IsNullOrWhiteSpace(deviceArray[1])) deviceArray[0] = "11";
            var callback = "http://api.offertest.net/offertest/8a85dc00-c7e7-4129-85d1-dae905f3da8b/result";
            try
            {
                using (var streamWriter = new StreamWriter(wr.GetRequestStream()))
                {
                    string json = "{\"url\":\"" + url + "\"," +
                                    "\"country\":\"" + country.ToLower() + "\"," +
                                    "\"platform\":\"" + deviceArray[0] + "\"," +
                                    "\"platformVersion\":\"" + deviceArray[1] + "\"," +
                                     "\"callback\":\"" + callback + "\"," +
                                    "\"userid\":\"" + userId +  "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)wr.GetResponse();
            }
            catch (Exception ex)
            {

                var additionInfo = string.Format(" Url: {0} --- country: {1}", url, country);
                Log.Error(ex.Message + additionInfo);
                tokenLogin = "";
            }

        }



        public ResponseLoginModel ProxyRequestLoginOrangear(string email = "", string pass = "")
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create("https://checker.orangear.com/api/v1/signin/");
            wr.Method = "POST";
            wr.ContentType = "application/json;charset=utf-8";
            email = System.Configuration.ConfigurationManager.AppSettings["offertestEmail"];
            pass = System.Configuration.ConfigurationManager.AppSettings["offertestPassword"];
            pass = "Woohyuk91@@";
            using (var streamWriter = new StreamWriter(wr.GetRequestStream()))
            {

                string json = "{\"email\":\"" + email + "\"," +
                               "\"password\":\"" + pass + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            wr.CookieContainer = new CookieContainer();

            ResponseLoginModel result = new ResponseLoginModel();
            var httpResponse = (HttpWebResponse)wr.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                JsonResult jr = new JsonResult();
                jr.Data = streamReader.ReadToEnd();
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseLoginModel>(jr.Data.ToString());
            }

            return result;
        }

        [AllowAnonymous]
        public ActionResult Click()
        {
            return View(new List<SequenceUrl>());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
    public class OfferTestApiData
    {
        public string userid { get; set; }
        public string country { get; set; }
        public string url { get; set; }
        public string platform { get; set; }
    }
}
