using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Affilitest.Models;
using System.Web.Security;
using System.Globalization;
using System.Data.Entity.Validation;

namespace Affilitest.Controllers
{
    public class HomeController : Controller
    {
        private AffilitestdbEntities db = new AffilitestdbEntities();
        [Authorize]
        [CheckUserExistAttribute]
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
            var numberOfUrl = user.NumberOfUrl;
            ViewBag.numberOfUrl = numberOfUrl;
            ViewBag.Title = "Home Page";
            List<SavingUrlModel> lsts; lsts = db.URLSavings.Where(s => s.UserID.Value.Equals(user.UserID)).Select(s => new SavingUrlModel()
            {
                click = s.Click,
                url = s.URL,
                thread = s.Speed,
                device = s.Device,
                country = s.GEO,
                textSelected = s.TextSelected
            }).ToList();

            return View(lsts);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveHistory(List<URLModel> lstu, string status)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            DataResult dtr = new DataResult();
            var um = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
            dtr.Status = Status.Success;

            List<string> listUrl = new List<string>();
            List<string> listClick = new List<string>();
            List<string> listSpeed = new List<string>();
            List<string> listGEO = new List<string>();
            List<string> listDevice = new List<string>();
            foreach (URLModel u in lstu)
            {
                listUrl.Add(u.url);
                listGEO.Add(u.country);
                listDevice.Add(u.device);
                listClick.Add(u.click);
                listSpeed.Add(u.thread);
            }
            History historyEntity = new History()
            {
                UserID = um.UserID,
                Click = string.Join("|", listClick.ToArray()),
                Speed = string.Join("|", listSpeed.ToArray()),
                URL = string.Join("|", listUrl.ToArray()),
                GEO = string.Join("|", listGEO.ToArray()),
                Device = string.Join("|", listDevice.ToArray()),
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                DateDefault = DateTime.Now,
                Status = status
            };
            try
            {
                db.Histories.Add(historyEntity);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                dtr.Status = Status.Error;
                dtr.Message = "Không lưu được lịch sử";
            }
            return Json(dtr, JsonRequestBehavior.AllowGet);
        }

        private bool isRemainClick(string userName, int click)
        {
            var userEntity = db.Users.Where(u => u.UserName.Equals(userName)).First();
            var isRemain = userEntity.TotalOfClick - userEntity.CurrentOfClick - click;
            return isRemain >= 0;
        }

        private bool checkValidateListUrl(List<URLModel> lstu)
        {
            var result = true;
            try
            {
                foreach (var url in lstu)
                {
                    var click = int.Parse(url.click);
                    var thread = int.Parse(url.thread);
                    if (click < 0 && thread < 0)
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProxyRequestPostData(List<URLModel> lstu)
        {
            DataResult dtr = new DataResult();
            dtr.Status = Status.Success;
            var um = db.Users.Where(u => u.UserName.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            um.DayClick = DateTime.Now.Day;
            if (!User.Identity.IsAuthenticated && um == null)
            {
                dtr.Status = Status.Redirect;
                dtr.Message = "";
                dtr.LinkRedirect = Url.Action("Login", "Home");
                return Json(dtr, JsonRequestBehavior.AllowGet);
            }


            if (!checkValidateListUrl(lstu))
            {
                dtr.Status = Status.Error;
                dtr.Message = "Thông số không hợp lệ, vui lòng kiểm tra lại";
                return Json(dtr, JsonRequestBehavior.AllowGet);
            }
            var checkQuickTotalClick = 0;
            foreach (var urlTab in lstu)
            {
                var threadQuick = (int.Parse(urlTab.click) - int.Parse(urlTab.thread)) >= 0 ? int.Parse(urlTab.thread) : int.Parse(urlTab.click);
                checkQuickTotalClick += threadQuick;
            }
            if (!isRemainClick(User.Identity.Name, checkQuickTotalClick))
            {
                dtr.Status = Status.Error;
                dtr.Message = "Bạn đã hết lượt click, vui lòng liên hệ Admin";
                return Json(dtr, JsonRequestBehavior.AllowGet);
            }
            var threadcheck = lstu.Where(t => int.Parse(t.thread) > 20).ToList().Count;
            if (threadcheck > 0)
            {
                dtr.Status = Status.Error;
                dtr.Message = "không được để speed lớn hơn 20";
                return Json(dtr, JsonRequestBehavior.AllowGet);
            }
            var totalClick = 0;
            foreach (URLModel u in lstu)
            {
                int click = Int32.Parse(u.click);
                int thread = Int32.Parse(u.thread);
                int times = 0;
                u.isStop = true;
                if (click > 0)
                {
                    u.isStop = false;
                    if ((click - thread) > 0)
                    {
                        times = thread;
                        u.click = (click - thread).ToString();
                    }
                    else
                    {
                        times = click;
                        u.thread = click.ToString();
                        u.click = "0";
                    }
                    for (int i = 0; i < times; i++)
                    {

                        db.SequenceUrls.Add(new SequenceUrl()
                        {
                            URL = u.url,
                            Device = u.device,
                            Country = u.country,
                            Date = DateTime.Now,
                            UserID = um.UserID
                        });
                    }
                    totalClick += times;
                }

                dtr.Data.Add(u);
            }
            if (um.NumberClick >= 0)
            {
                if (DateTime.Now.Day == um.DayClick)
                {
                    um.NumberClick += totalClick;
                }
                else
                {
                    um.NumberClick = totalClick;
                }
            }
            SaveHistory(lstu, um.UserID);

            try
            {
                um.CurrentOfClick += totalClick;
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var stringbuilder = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    stringbuilder.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        stringbuilder.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                dtr.Status = Status.Error;
                dtr.Message = stringbuilder.ToString();
            }
            catch (Exception ex)
            {
                dtr.Status = Status.Error;
                dtr.Message = ex.Message;
            }
            return Json(dtr, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteCustom(string dateDelete)
        {
            var dateDeleteDT = DateTime.ParseExact(dateDelete, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (dateDeleteDT.Year > 2000)
            {
                var histories = db.Histories.Where(h => h.DateDefault.Value.Month == dateDeleteDT.Month && h.DateDefault.Value.Year == dateDeleteDT.Year).ToList();
                foreach (var hi in histories)
                {
                    db.Histories.Remove(hi);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "History");
        }

        private void SaveHistory(List<URLModel> lstu, Guid userID)
        {
            DataResult dtr = new DataResult();
            dtr.Status = Status.Success;

            List<string> listUrl = new List<string>();
            List<string> listClick = new List<string>();
            List<string> listSpeed = new List<string>();
            List<string> listGEO = new List<string>();
            List<string> listDevice = new List<string>();
            foreach (URLModel u in lstu)
            {
                listUrl.Add(u.url);
                listGEO.Add(u.country);
                listDevice.Add(u.device);
                listClick.Add(u.click);
                listSpeed.Add(u.thread);
            }
            History historyEntity = new History()
            {
                UserID = userID,
                Click = string.Join("|", listClick.ToArray()),
                Speed = string.Join("|", listSpeed.ToArray()),
                URL = string.Join("|", listUrl.ToArray()),
                GEO = string.Join("|", listGEO.ToArray()),
                Device = string.Join("|", listDevice.ToArray()),
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                DateDefault = DateTime.Now,
            };
            try
            {
                db.Histories.Add(historyEntity);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                dtr.Status = Status.Error;
                dtr.Message = "Không lưu được lịch sử";
            }
        }
        [HttpPost]
        [Authorize]
        public ActionResult getCurrentClick()
        {
            var um = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).Select(u => new { u.UserName, u.CurrentOfClick, u.TotalOfClick }).First();
            DataResult dtr = new DataResult();
            dtr.Data.Add(um);
            dtr.Status = Status.Success;
            return Json(dtr);

        }

        private void AddOneClickToDB()
        {
            var um = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
            try
            {
                um.CurrentOfClick += 1;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
        public bool CheckSessionLogined()
        {
            if (Session["user"] == null)
            {
                return false;
            }
            return true;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginToHome(string username, string password)
        {

            User user = db.Users.Where(u => u.UserName.Equals(username) && u.Password.Equals(password)).FirstOrDefault();
            var r = "normal";
            if (user != null)
            {
                UserModel model = new UserModel()
                {
                    UserName = username,
                    Password = password,
                    Role = user.Role
                };
                if (user.Role)
                {
                    r = "Admin";
                }
                Response.Cookies.Add(UserModel.GetAuthenticationCookie(model, new UserCookieData(DateTime.Now) { Roles = new[] { r } }));

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveUrl(List<SavingUrlModel> lstSaving)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            var user = db.Users.Where(u => u.UserName.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (user != null)
            {
                var urlSavingOld = db.URLSavings.Where(s => s.UserID.Value.Equals(user.UserID)).ToList();
                foreach (var u in urlSavingOld)
                {
                    db.URLSavings.Remove(u);
                }
                foreach (var s in lstSaving)
                {
                    URLSaving urlSaving = new URLSaving()
                    {
                        Click = s.click,
                        Device = s.device,
                        GEO = s.country,
                        Speed = s.thread,
                        URL = s.url,
                        UserID = user.UserID,
                        TextSelected = s.textSelected
                    };
                    db.URLSavings.Add(urlSaving);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
                return Json("success");
            }
            else
            {
                return Json("fail");
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(string curentPassword, string newPassword, string passwordConfirm)
        {
            DataResult dtr = new DataResult();
            if (!User.Identity.IsAuthenticated)
            {
                dtr.Status = Status.Error;
                dtr.Message = "không có quyền truy cập";
                return Json(dtr);
            }
            if (string.IsNullOrEmpty(curentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(passwordConfirm))
            {
                dtr.Status = Status.Error;
                dtr.Message = "không được để trống trường nào";
                return Json(dtr);
            }
            var user = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
            if (!user.Password.Equals(curentPassword))
            {
                dtr.Status = Status.Error;
                dtr.Message = "password hiện tại không đúng";
                return Json(dtr);
            }
            if (!newPassword.Equals(passwordConfirm))
            {
                dtr.Status = Status.Error;
                dtr.Message = "password nhập lại không đúng cho lắm.";
                return Json(dtr);
            }
            user.Password = newPassword;
            try
            {
                db.SaveChanges();
                dtr.Message = "Đổi Thành Công";
                dtr.Status = Status.Success;
                FormsAuthentication.SignOut();
            }

            catch (Exception ex)
            {
                dtr.Status = Status.Error;
                dtr.Message = ex.Message;
            }

            return Json(dtr);
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
}
