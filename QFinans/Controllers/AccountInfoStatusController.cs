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
    public class AccountInfoStatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexAccountInfoStatus")]
        // GET: AccountInfoStatus
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

            IQueryable<AccountInfoStatus> accountInfoStatus = db.AccountInfoStatus.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountInfoStatus = accountInfoStatus.Where(x => x.Name.Contains(searchString));

                if (accountInfoStatus.Any() == false)
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
            return View(accountInfoStatus.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsAccountInfoStatus")]
        // GET: AccountInfoStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoStatus accountInfoStatus = db.AccountInfoStatus.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoStatus == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoStatus);
        }

        [CustomAuth(Roles = "CreateAccountInfoStatus")]
        // GET: AccountInfoStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountInfoStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateAccountInfoStatus")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountInfoStatus accountInfoStatus)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                accountInfoStatus.AddUserId = _userId;
                accountInfoStatus.AddDate = DateTime.Now;
                db.AccountInfoStatus.Add(accountInfoStatus);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(accountInfoStatus);
        }

        [CustomAuth(Roles = "EditAccountInfoStatus")]
        // GET: AccountInfoStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoStatus accountInfoStatus = db.AccountInfoStatus.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoStatus == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoStatus);
        }

        // POST: AccountInfoStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditAccountInfoStatus")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountInfoStatus accountInfoStatus)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.AccountInfoStatus.Find(accountInfoStatus.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                accountInfoStatus.AddUserId = orjData.AddUserId;
                accountInfoStatus.AddDate = orjData.AddDate;
                accountInfoStatus.UpdateUserId = _userId;
                accountInfoStatus.UpdateDate = DateTime.Now;
                db.Entry(accountInfoStatus).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(accountInfoStatus);
        }

        [CustomAuth(Roles = "DeleteAccountInfoStatus")]
        // GET: AccountInfoStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoStatus accountInfoStatus = db.AccountInfoStatus.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoStatus == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoStatus);
        }

        // POST: AccountInfoStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteAccountInfoStatus")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            AccountInfoStatus accountInfoStatus = db.AccountInfoStatus.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            accountInfoStatus.IsDeleted = true;
            accountInfoStatus.UpdateUserId = _userId;
            accountInfoStatus.UpdateDate = DateTime.Now;
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
