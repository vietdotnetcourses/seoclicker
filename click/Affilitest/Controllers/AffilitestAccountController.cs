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
    [Authorize(Roles = "Admin")]
    public class AffilitestAccountController : Controller
    {
        private AffilitestdbEntities db = new AffilitestdbEntities();

                public ActionResult Index()
        {
            return View(db.AffilitestAccounts.OrderBy(a => a.Number).ToList());
        }

                public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AffilitestAccount affilitestaccount = db.AffilitestAccounts.Find(id);
            if (affilitestaccount == null)
            {
                return HttpNotFound();
            }
            return View(affilitestaccount);
        }

                public ActionResult Create()
        {
            return View();
        }

                                [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Email,Password,Active,AffilitestAccountID,Number")] AffilitestAccount affilitestaccount)
        {
            if (ModelState.IsValid)
            {
                if (db.AffilitestAccounts.Where(a => a.Number == affilitestaccount.Number).Count() > 0)
                {
                    ModelState.AddModelError(string.Empty, "số thứ tự này đã tồn tại");
                    return View();
                }
                affilitestaccount.AffilitestAccountID = Guid.NewGuid();
                db.AffilitestAccounts.Add(affilitestaccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(affilitestaccount);
        }

                public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AffilitestAccount affilitestaccount = db.AffilitestAccounts.Find(id);
            if (affilitestaccount == null)
            {
                return HttpNotFound();
            }
            return View(affilitestaccount);
        }

                                [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Email,Password,Active,AffilitestAccountID, Number")] AffilitestAccount affilitestaccount)
        {
            if (ModelState.IsValid)
            {
                if(db.AffilitestAccounts.Where(a=> a.Number == affilitestaccount.Number).Count() > 0){
                    ModelState.AddModelError(string.Empty, "số thứ tự này đã tồn tại");
                    return View();
                }
                db.Entry(affilitestaccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(affilitestaccount);
        }

                public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AffilitestAccount affilitestaccount = db.AffilitestAccounts.Find(id);
            if (affilitestaccount == null)
            {
                return HttpNotFound();
            }
            return View(affilitestaccount);
        }

                [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AffilitestAccount affilitestaccount = db.AffilitestAccounts.Find(id);
            db.AffilitestAccounts.Remove(affilitestaccount);
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
