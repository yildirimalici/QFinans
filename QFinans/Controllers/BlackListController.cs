using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class BlackListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexBlackList")]
        // GET: BlackList
        public ActionResult Index(string currentFilter, string searchString, int? page, int? customPageSize)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IQueryable<BlackList> blackList = db.BlackList.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                blackList = blackList.Where(x => x.UserName.Contains(searchString));

                if (blackList.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            if (customPageSize != null)
            {
                ViewBag.CustomPageSize = customPageSize;
            }
            else
            {
                ViewBag.CustomPageSize = 10;
            }
            int pageSize = (customPageSize ?? 10);
            int pageNumber = (page ?? 1);
            return View(blackList.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: BlackList/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlackList blackList = db.BlackList.Find(id);
        //    if (blackList == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blackList);
        //}

        // GET: BlackList/Create

        [CustomAuth(Roles = "CreateBlackList")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlackList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateBlackList")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlackList blackList)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                blackList.AddUserId = _userId;
                blackList.AddDate = DateTime.Now;
                db.BlackList.Add(blackList);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(blackList);
        }

        [CustomAuth(Roles = "EditBlackList")]
        // GET: BlackList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlackList blackList = db.BlackList.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (blackList == null)
            {
                return HttpNotFound();
            }
            return View(blackList);
        }

        // POST: BlackList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditBlackList")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlackList blackList)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.BlackList.Find(blackList.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                blackList.AddUserId = orjData.AddUserId;
                blackList.AddDate = orjData.AddDate;
                blackList.UpdateUserId = _userId;
                blackList.UpdateDate = DateTime.Now;
                db.Entry(blackList).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(blackList);
        }

        [CustomAuth(Roles = "DeleteBlackList")]
        // GET: BlackList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlackList blackList = db.BlackList.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (blackList == null)
            {
                return HttpNotFound();
            }
            return View(blackList);
        }

        // POST: BlackList/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteBlackList")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            BlackList blackList = db.BlackList.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            blackList.IsDeleted = true;
            blackList.UpdateUserId = _userId;
            blackList.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Kayıt silindi.";
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
