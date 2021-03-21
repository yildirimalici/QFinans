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
    public class AccountAmountRedirectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexAccountAmountRedirect")]
        // GET: AccountAmountRedirect
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

            IQueryable<AccountAmountRedirect> accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountAmountRedirect = accountAmountRedirect.Where(x => x.Name.Contains(searchString) || x.MinAmount.ToString().Contains(searchString) || x.MaxAmount.ToString().Contains(searchString));

                if (accountAmountRedirect.Any() == false)
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
            return View(accountAmountRedirect.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsAccountAmountRedirect")]
        // GET: AccountAmountRedirect/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountAmountRedirect accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountAmountRedirect == null)
            {
                return HttpNotFound();
            }
            return View(accountAmountRedirect);
        }

        [CustomAuth(Roles = "CreateAccountAmountRedirect")]
        // GET: AccountAmountRedirect/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountAmountRedirect/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateAccountAmountRedirect")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountAmountRedirect accountAmountRedirect)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                accountAmountRedirect.AddUserId = _userId;
                accountAmountRedirect.AddDate = DateTime.Now;
                db.AccountAmountRedirect.Add(accountAmountRedirect);
                db.SaveChanges();
                TempData["success"] = "Tutar yönlendirme türü eklendi.";
                return RedirectToAction("Index");
            }

            return View(accountAmountRedirect);
        }

        [CustomAuth(Roles = "EditAccountAmountRedirect")]
        // GET: AccountAmountRedirect/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountAmountRedirect accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountAmountRedirect == null)
            {
                return HttpNotFound();
            }
            return View(accountAmountRedirect);
        }

        // POST: AccountAmountRedirect/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditAccountAmountRedirect")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountAmountRedirect accountAmountRedirect)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.AccountAmountRedirect.Find(accountAmountRedirect.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                accountAmountRedirect.AddUserId = orjData.AddUserId;
                accountAmountRedirect.AddDate = orjData.AddDate;
                accountAmountRedirect.UpdateUserId = _userId;
                accountAmountRedirect.UpdateDate = DateTime.Now;
                db.Entry(accountAmountRedirect).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Tutar yönlendirme türü düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(accountAmountRedirect);
        }

        [CustomAuth(Roles = "DeleteAccountAmountRedirect")]
        // GET: AccountAmountRedirect/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountAmountRedirect accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountAmountRedirect == null)
            {
                return HttpNotFound();
            }
            return View(accountAmountRedirect);
        }

        // POST: AccountAmountRedirect/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteAccountAmountRedirect")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            AccountAmountRedirect accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            accountAmountRedirect.IsDeleted = true;
            accountAmountRedirect.UpdateUserId = _userId;
            accountAmountRedirect.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Tutar yönlendirme türü silindi.";
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
