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
    public class CashFlowTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CashFlowType
        [CustomAuth(Roles = "IndexCashFlowType")]
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

            IQueryable<CashFlowType> cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                cashFlowType = cashFlowType.Where(x => x.Name.Contains(searchString));

                if (cashFlowType.Any() == false)
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
                ViewBag.CustomPageSize = 100;
            }
            int pageSize = (customPageSize ?? 100);
            int pageNumber = (page ?? 1);
            return View(cashFlowType.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: CashFlowType/Details/5
        [CustomAuth(Roles = "DetailsCashFlowType")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlowType cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlowType == null)
            {
                return HttpNotFound();
            }
            return View(cashFlowType);
        }

        // GET: CashFlowType/Create
        [CustomAuth(Roles = "CreateCashFlowType")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CashFlowType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateCashFlowType")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CashFlowType cashFlowType)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                cashFlowType.AddUserId = _userId;
                cashFlowType.AddDate = DateTime.Now;
                db.CashFlowType.Add(cashFlowType);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(cashFlowType);
        }

        // GET: CashFlowType/Edit/5
        [CustomAuth(Roles = "EditCashFlowType")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlowType cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlowType == null)
            {
                return HttpNotFound();
            }
            return View(cashFlowType);
        }

        // POST: CashFlowType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditCashFlowType")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CashFlowType cashFlowType)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.CashFlowType.Find(cashFlowType.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                cashFlowType.AddUserId = orjData.AddUserId;
                cashFlowType.AddDate = orjData.AddDate;
                cashFlowType.UpdateUserId = _userId;
                cashFlowType.UpdateDate = DateTime.Now;
                db.Entry(cashFlowType).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(cashFlowType);
        }

        // GET: CashFlowType/Delete/5
        [CustomAuth(Roles = "DeleteCashFlowType")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlowType cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlowType == null)
            {
                return HttpNotFound();
            }
            return View(cashFlowType);
        }

        // POST: CashFlowType/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteCashFlowType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            CashFlowType cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            cashFlowType.IsDeleted = true;
            cashFlowType.UpdateUserId = _userId;
            cashFlowType.UpdateDate = DateTime.Now;
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
