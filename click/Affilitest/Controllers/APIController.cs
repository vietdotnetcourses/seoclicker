using Affilitest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Affilitest.Controllers
{
    public class APIController : Controller
    {
        [HttpPost]
        public ActionResult Click(List<ModelAPIClick> listData, string UserName, string Password)
        {
            DataResult dtr = new DataResult();
            dtr.Status = Status.Success;
            using (var db = new AffilitestdbEntities())
            {
                var um = db.Users.Where(u => u.UserName.Equals(UserName) && u.Password.Equals(Password)).FirstOrDefault();
                if (um == null)
                {
                    dtr.Status = Status.Error;
                    dtr.Message = "user and password not correctly";
                    dtr.LinkRedirect = Url.Action("Login", "Home");
                    return Json(dtr, JsonRequestBehavior.AllowGet);
                }
                if (!checkValidateListUrl(listData))
                {
                    dtr.Status = Status.Error;
                    dtr.Message = "Thông số không hợp lệ, vui lòng kiểm tra lại";
                    return Json(dtr, JsonRequestBehavior.AllowGet);
                }
                um.DayClick = DateTime.Now.Day;
                var checkQuickTotalClick = 0;
                foreach (var urlTab in listData)
                {
                    var threadQuick = int.Parse(urlTab.Click);
                    checkQuickTotalClick += threadQuick;
                }
                if (!isRemainClick(UserName, checkQuickTotalClick))
                {
                    dtr.Status = Status.Error;
                    dtr.Message = "Bạn đã hết lượt click, vui lòng liên hệ Admin";
                    return Json(dtr, JsonRequestBehavior.AllowGet);
                }
                var threadcheck = listData.Where(t => int.Parse(t.Click) > 40).ToList().Count;
                if (threadcheck > 0)
                {
                    dtr.Status = Status.Error;
                    dtr.Message = "không được để click lớn hơn 40";
                    return Json(dtr, JsonRequestBehavior.AllowGet);
                }
                foreach (var u in listData)
                {
                    int thread = Int32.Parse(u.Click);
                    if (thread > 0)
                    {
                        for (int i = 0; i < thread; i++)
                        {
                            db.SequenceUrls.Add(new SequenceUrl()
                            {
                                URL = u.Url,
                                Device = u.Device,
                                Country = u.Geo,
                                UserID = um.UserID
                            });
                        }
                    }
                    dtr.Data.Add(u);
                }
                if (um.NumberClick >= 0)
                {
                    if (DateTime.Now.Day == um.DayClick)
                    {
                        um.NumberClick += checkQuickTotalClick;
                    }
                    else
                    {
                        um.NumberClick = checkQuickTotalClick;
                    }
                }
                try
                {
                    um.CurrentOfClick += checkQuickTotalClick;
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
                    throw;
                }
                catch (Exception ex)
                {
                    dtr.Status = Status.Error;
                    dtr.Message = ex.Message;
                }
            }
            return Json(dtr, JsonRequestBehavior.AllowGet);

        }

        private bool checkValidateListUrl(List<ModelAPIClick> lstu)
        {
            var result = true;
            try
            {
                foreach (var url in lstu)
                {
                    var click = int.Parse(url.Click);
                    if (click < 0)
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
        [AllowAnonymous]
        public async Task<ActionResult> CheckUserAllowTest(string tokenKey)
        {
            var result = await Task.Run(() =>
            {
                using (var db = new AffilitestdbEntities())
                {
                    var userEntity = db.Users.Where(u => u.Token.Equals(tokenKey)).FirstOrDefault();
                    if (userEntity != null && userEntity.TotalOfClick.HasValue)
                    {
                        var isRemain = userEntity.TotalOfClick - userEntity.CurrentOfClick - 1;
                        if (isRemain >= 0)
                        {
                            userEntity.CurrentOfClick = userEntity.CurrentOfClick + 1;
                            db.SaveChanges();
                            Guid g = Guid.NewGuid();

                            string token = g.ToString();
                            token = token.Substring(0, 25) + "o" + token.Substring(26); return Json(new { AllowTest = true, Token = token });
                        }
                        else
                        {
                            return Json(new { AllowTest = false, Message = "Hết lượt click, liên hệ HuyNN" });
                        }
                    }
                    else
                    {
                        return Json(new { AllowTest = false, Message = "Tài Khoản Không tồn tại" });
                    }
                }
            });
            return result;
        }

        private bool isRemainClick(string userName, int click)
        {
            using (var db = new AffilitestdbEntities())
            {
                var userEntity = db.Users.Where(u => u.UserName.Equals(userName)).First();
                var isRemain = userEntity.TotalOfClick - userEntity.CurrentOfClick - click;
                return isRemain >= 0;
            }
        }

        private bool CheckUserExist(string username, string password)
        {
            using (var db = new AffilitestdbEntities())
            {
                var userEntity = db.Users.Where(u => u.UserName.Equals(username) && u.Password.Equals(password)).FirstOrDefault();
                if (userEntity != null)
                {
                    return true;
                }
                return false;
            }
        }

        [HttpGet]
        public JsonResult GetDataForTool(int take, string token)
        {
           
            if (token== "O2ECaKWYM5Q1goceJDI9gNMQ2O8tKskZ")
            {
                var result = new List<SequenceUrl>();
                using (var dbContenxt = new AffilitestdbEntities())
                {
                    result = dbContenxt.SequenceUrls.Take(take).ToList();



                    foreach (var item in result)
                    {
                        dbContenxt.SequenceUrls.Remove(item);
                    }

                    try
                    {
                        dbContenxt.SaveChanges();
                    }
                    catch
                    {

                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }


         
        }
    }

    public class ModelAPIClick
    {
        public string Url { get; set; }
        public string Device { get; set; }
        public string Geo { get; set; }
        public string Click { get; set; }
    }
}