using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class SystemParametersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "SystemParameters")]
        // GET: SystemParameters
        public ActionResult Index()
        {
            var systemParameters = db.SystemParameters.Count();
            if (systemParameters == 0)
            {
                return RedirectToAction("Create");
            } else
            {
                return RedirectToAction("Edit");
            }
            //return View();
        }

        // GET: SystemParameters/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SystemParameters systemParameters = db.SystemParameters.Find(id);
        //    if (systemParameters == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(systemParameters);
        //}

        [CustomAuth(Roles = "SystemParameters")]
        // GET: SystemParameters/Create
        public ActionResult Create()
        {
            var systemParameters = db.SystemParameters.Count();
            if (systemParameters == 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        // POST: SystemParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "SystemParameters")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SystemParameters systemParameters)
        {
            if (ModelState.IsValid)
            {
                db.SystemParameters.Add(systemParameters);
                db.SaveChanges();
                TempData["success"] = "Parametreler kaydedildi.";
                return RedirectToAction("Index");
            }

            return View(systemParameters);
        }

        [CustomAuth(Roles = "SystemParameters")]
        // GET: SystemParameters/Edit/5
        public ActionResult Edit()
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            SystemParameters systemParameters = db.SystemParameters.FirstOrDefault();
            if (systemParameters == null)
            {
                return HttpNotFound();
            }
            return View(systemParameters);
        }

        // POST: SystemParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "SystemParameters")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SystemParameters systemParameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemParameters).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Parametreler kaydedildi.";
                return RedirectToAction("Index");
            }
            return View(systemParameters);
        }

        // GET: SystemParameters/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SystemParameters systemParameters = db.SystemParameters.Find(id);
        //    if (systemParameters == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(systemParameters);
        //}

        //// POST: SystemParameters/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    SystemParameters systemParameters = db.SystemParameters.Find(id);
        //    db.SystemParameters.Remove(systemParameters);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
