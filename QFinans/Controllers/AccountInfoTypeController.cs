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
    public class AccountInfoTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexAccountInfoType")]
        // GET: AccountInfoType
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

            IQueryable<AccountInfoType> accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountInfoType = accountInfoType.Where(x => x.Name.Contains(searchString));

                if (accountInfoType.Any() == false)
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
                ViewBag.CustomPageSize = 25;
            }
            int pageSize = (customPageSize ?? 25);
            int pageNumber = (page ?? 1);
            //return View(accountInfoType);
            return View(accountInfoType.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsAccountInfoType")]
        // GET: AccountInfoType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoType accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoType == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoType);
        }

        [CustomAuth(Roles = "CreateAccountInfoType")]
        // GET: AccountInfoType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountInfoType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateAccountInfoType")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountInfoType accountInfoType)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                accountInfoType.AddUserId = _userId;
                accountInfoType.AddDate = DateTime.Now;
                db.AccountInfoType.Add(accountInfoType);
                db.SaveChanges();
                TempData["success"] = "Hesap türü eklendi.";
                return RedirectToAction("Index");
            }

            return View(accountInfoType);
        }

        [CustomAuth(Roles = "EditAccountInfoType")]
        // GET: AccountInfoType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoType accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoType == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoType);
        }

        // POST: AccountInfoType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditAccountInfoType")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountInfoType accountInfoType)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.AccountInfoType.Find(accountInfoType.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                accountInfoType.AddUserId = orjData.AddUserId;
                accountInfoType.AddDate = orjData.AddDate;
                accountInfoType.UpdateUserId = _userId;
                accountInfoType.UpdateDate = DateTime.Now;
                db.Entry(accountInfoType).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Hesap türü düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(accountInfoType);
        }

        [CustomAuth(Roles = "DeleteAccountInfoType")]
        // GET: AccountInfoType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfoType accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfoType == null)
            {
                return HttpNotFound();
            }
            return View(accountInfoType);
        }

        // POST: AccountInfoType/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteAccountInfoType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            AccountInfoType accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            accountInfoType.IsDeleted = true;
            accountInfoType.UpdateUserId = _userId;
            accountInfoType.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Hesap türü silindi.";
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
