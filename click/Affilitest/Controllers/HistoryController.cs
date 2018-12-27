using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Affilitest.Models;

namespace Affilitest.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private AffilitestdbEntities db = new AffilitestdbEntities();

                [CheckUserExistAttribute]
        public ActionResult Index(string searchString = "", string date = "today", int page = 1)
        {
            var dateOrder = DateTime.Now.AddDays(-1).Date;
            if (date.Equals("yesterday"))
            {
                dateOrder = DateTime.Now.AddDays(-1).Date;
            }
            else if (date.Equals("lastweek"))
            {
                dateOrder = DateTime.Now.AddDays(-7).Date;
            }
            else if (date.Equals("lastmonth"))
            {
                dateOrder = DateTime.Now.AddDays(-31).Date;
            }
            List<HistoryModel> result = new List<HistoryModel>();
                        var query = string.Format("select h.*, u.UserName from Histories h inner join Users u on h.UserID = u.UserID where ((u.UserName like '%{0}%' or h.URL like '%{0}%' or h.GEO like '%{0}%' or h.Click like '%{0}%' or h.Date like '%{0}%')", searchString);
            if (!User.IsInRole("Admin"))
            {
                ViewBag.roleUser = false;
                var userID = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).Select(u => u.UserID).First();
                                query += string.Format(" AND u.UserID = '{0}' ", userID.ToString());
            }
            else
            {
                                ViewBag.roleUser = true;
            }
            query += string.Format(" AND h.DateDefault >= '{0}' ) ", dateOrder.ToString("yyyy-MM-dd"));
            var queryCount = string.Format("select count(*) from ({0}) as rc", query);
            ViewBag.count = Helper.ExecuteScalarDapper<int>(queryCount);             query += string.Format(" order by h.DateDefault DESC OFFSET {1} ROWS FETCH NEXT 50 rows only ", dateOrder.ToString("yyyy-MM-dd"), 50 * (page - 1));
                                                            result = Helper.GetDataFromDB<HistoryModel>(query);                         ViewBag.page = page;
            ViewBag.searchString = searchString;
            ViewBag.date = date;
            return View(result);
        }

        [AllowAnonymous]
        public void DeleteYesterdayHistory()
        {
            var query = string.Format(" delete from Histories where DateDefault < '{0}' ", DateTime.Now.ToString("yyyy-MM-dd"));
            Helper.ExecuteNonQueryStringStatic(query);
        }

        public ActionResult GetAllOfHistory()
        {
            var user = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
            var result = from u in db.Users join h in db.Histories on u.UserID equals h.UserID orderby h.DateDefault descending select new HistoryModel() { UserName = u.UserName, Click = h.Click, Date = h.Date, Device = h.Device, GEO = h.GEO, URL = h.URL, UserID = h.UserID, HistoryID = h.HistoryID, TotalClick = h.Click, Status = h.Status };
            List<HistoryModel> lh = new List<HistoryModel>();
            ViewBag.roleUser = user.Role;
            if (!user.Role)
            {
                result = result.Where(r => r.UserName.Equals(User.Identity.Name));
            }
            lh = result.ToList();
            return Json(lh);
        }

        [Authorize]
        [CheckUserExistAttribute]
        public ActionResult Search(string searchString = "", string date = "today", int page = 1)
        {
            var dateOrder = DateTime.Now.AddDays(-1).Date;
            if (date.Equals("yesterday"))
            {
                dateOrder = DateTime.Now.AddDays(-1).Date;
            }
            else if (date.Equals("lastweek"))
            {
                dateOrder = DateTime.Now.AddDays(-7).Date;
            }
            else if (date.Equals("lastmonth"))
            {
                dateOrder = DateTime.Now.AddDays(-31).Date;
            }
                List<HistoryModel> result = new List<HistoryModel>();
                            var query = string.Format("select h.*, u.UserName from Histories h inner join Users u on h.UserID = u.UserID where ((u.UserName like '%{0}%' or h.URL like '%{0}%' or h.GEO like '%{0}%' or h.Click like '%{0}%' or h.Date like '%{0}%')", searchString);
            if(!User.IsInRole("Admin")){
                ViewBag.roleUser = false;
                var userID = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).Select(u => u.UserID).First();
                                query += string.Format(" AND u.UserID = '{0}' ", userID.ToString());
            }
            else
            {
                                ViewBag.roleUser = true;
            }
            query += string.Format(" AND h.DateDefault >= '{0}' ) order by h.DateDefault DESC OFFSET {1} ROWS FETCH NEXT 50 rows only ", dateOrder.ToString("yyyy-MM-dd"), 50*(page-1));
            var queryCount = string.Format("select count(*) from ({0}) as rc", query);
            var pageCount = db.Database.SqlQuery<int>(queryCount).ToList();
            if(pageCount.Count > 0){
                ViewBag.count = pageCount[0];
            }
            result = db.Database.SqlQuery<HistoryModel>(query).ToList();
                        ViewBag.page = page;
            ViewBag.searchString = searchString;
            ViewBag.date = date;
            return View("Index", result);
        }

                public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

                [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

                                [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HistoryID,UserID,URL,Date,Click,GEO,Device")] History history)
        {
            if (ModelState.IsValid)
            {
                history.HistoryID = Guid.NewGuid();
                db.Histories.Add(history);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(history);
        }

                [Authorize(Roles = "Admin")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

                                [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "HistoryID,UserID,URL,Date,Click,GEO,Device")] History history)
        {
            if (ModelState.IsValid)
            {
                db.Entry(history).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(history);
        }

                [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

                [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            History history = db.Histories.Find(id);
            db.Histories.Remove(history);
            db.SaveChanges();
            return RedirectToAction("Index");
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
