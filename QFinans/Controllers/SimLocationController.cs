using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using QFinans.Repostroies;
using QRCoder;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class SimLocationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexSimLocation")]
        // GET: SimLocation
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

            IQueryable<SimLocation> simLocation = db.SimLocation.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                simLocation = simLocation.Where(x => x.Name.Contains(searchString));

                if (simLocation.Any() == false)
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
            return View(simLocation.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsSimLocation")]
        // GET: SimLocation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SimLocation simLocation = db.SimLocation.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (simLocation == null)
            {
                return HttpNotFound();
            }

            //CustomFunctions qr = new CustomFunctions();
            //string code = qr.GenerateQR(simLocation.Name);
            //ViewBag.QR = code;

            return View(simLocation);
        }

        [CustomAuth(Roles = "CreateSimLocation")]
        // GET: SimLocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SimLocation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateSimLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SimLocation simLocation)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                simLocation.AddUserId = _userId;
                simLocation.AddDate = DateTime.Now;
                db.SimLocation.Add(simLocation);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(simLocation);
        }

        [CustomAuth(Roles = "EditSimLocation")]
        // GET: SimLocation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SimLocation simLocation = db.SimLocation.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (simLocation == null)
            {
                return HttpNotFound();
            }
            return View(simLocation);
        }

        // POST: SimLocation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditSimLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SimLocation simLocation)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.SimLocation.Find(simLocation.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                simLocation.AddUserId = orjData.AddUserId;
                simLocation.AddDate = orjData.AddDate;
                simLocation.UpdateUserId = _userId;
                simLocation.UpdateDate = DateTime.Now;
                db.Entry(simLocation).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(simLocation);
        }

        [CustomAuth(Roles = "DeleteSimLocation")]
        // GET: SimLocation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SimLocation simLocation = db.SimLocation.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (simLocation == null)
            {
                return HttpNotFound();
            }
            return View(simLocation);
        }

        // POST: SimLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteSimLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            SimLocation simLocation = db.SimLocation.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            simLocation.IsDeleted = true;
            simLocation.UpdateUserId = _userId;
            simLocation.UpdateDate = DateTime.Now;
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
