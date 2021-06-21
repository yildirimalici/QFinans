using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class PaparaxController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexPaparax")]
        // GET: Paparax
        public ActionResult Index()
        {
            var safe = db.AccountInfo.Where(a => a.IsDeleted == false && a.IsArchive == false).Select(x => x.Balance).DefaultIfEmpty(0).Sum() ?? 0;
            ViewBag.Safe = safe.ToString("N2");
            var paparax = db.Paparax.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Take(1).ToList();
            var balanceDiff = safe - paparax.Select(x => x.Balance).DefaultIfEmpty(0).FirstOrDefault();
            var paparaxBalanceDiff = db.SystemParameters.Select(x => x.PaparaxBalaceDiff).DefaultIfEmpty(0).FirstOrDefault();

            if (balanceDiff > paparaxBalanceDiff)
            {
                TempData["warning"] = "Toplam kasa farkı " + balanceDiff.ToString("N2") + " olup belirlenen " + paparaxBalanceDiff.ToString("N2") + " olan tutardan fazladır.";
            }
            else if (balanceDiff < paparaxBalanceDiff)
            {
                TempData["warning"] = "Toplam kasa farkı " + balanceDiff.ToString("N2") + " olup belirlenen " + paparaxBalanceDiff.ToString("N2") + " olan tutardan azdır.";
            }
            return View(paparax);
        }

        [CustomAuth(Roles = "DetailsPaparax")]
        // GET: Paparax/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paparax paparax = db.Paparax.Find(id);
            if (paparax == null)
            {
                return HttpNotFound();
            }
            return View(paparax);
        }

        [CustomAuth(Roles = "CreatePaparax")]
        // GET: Paparax/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Paparax/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuth(Roles = "CreatePaparax")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Paparax paparax)
        {
            var safe = db.AccountInfo.Where(a => a.IsDeleted == false && a.IsArchive == false).Select(x => x.Balance).DefaultIfEmpty(0).Sum() ?? 0;
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                paparax.Safe = safe;
                paparax.AddDate = DateTime.Now;
                paparax.AddUserId = _userId;
                db.Paparax.Add(paparax);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paparax);
        }

        [CustomAuth(Roles = "EditPaparax")]
        // GET: Paparax/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paparax paparax = db.Paparax.Find(id);
            if (paparax == null)
            {
                return HttpNotFound();
            }
            return View(paparax);
        }

        // POST: Paparax/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuth(Roles = "EditPaparax")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Paparax paparax)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var _paparax = db.Paparax.Find(paparax.Id);
                _paparax.Balance = paparax.Balance;
                _paparax.UpdateDate = DateTime.Now;
                _paparax.UpdateUserId = _userId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paparax);
        }

        [CustomAuth(Roles = "DeletePaparax")]
        // GET: Paparax/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paparax paparax = db.Paparax.Find(id);
            if (paparax == null)
            {
                return HttpNotFound();
            }
            return View(paparax);
        }

        // POST: Paparax/Delete/5
        [CustomAuth(Roles = "DeletePaparax")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            Paparax paparax = db.Paparax.Find(id);
            paparax.IsDeleted = true;
            paparax.UpdateDate = DateTime.Now;
            paparax.UpdateUserId = _userId;
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
