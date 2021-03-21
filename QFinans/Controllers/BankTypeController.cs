using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFinans.Areas.Api.Models;
using QFinans.Models;
using QFinans.CustomFilters;
using PagedList;
using Microsoft.AspNet.Identity;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class BankTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankType
        [CustomAuth(Roles = "IndexBankType")]
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

            IQueryable<BankType> bankTypes = db.BankType.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                bankTypes = bankTypes.Where(x => x.Name.Contains(searchString));

                if (bankTypes.Any() == false)
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
            return View(bankTypes.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: BankType/Details/5
        [CustomAuth(Roles = "DetailsBankType")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankType bankType = await db.BankType.FindAsync(id);
            if (bankType == null)
            {
                return HttpNotFound();
            }
            return View(bankType);
        }

        // GET: BankType/Create
        [CustomAuth(Roles = "CreateBankType")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateBankType")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BankType bankType)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                bankType.AddUserId = _userId;
                bankType.AddDate = DateTime.Now;
                db.BankType.Add(bankType);
                await db.SaveChangesAsync();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(bankType);
        }

        // GET: BankType/Edit/5
        [CustomAuth(Roles = "EditBankType")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankType bankType = await db.BankType.FindAsync(id);
            if (bankType == null)
            {
                return HttpNotFound();
            }
            return View(bankType);
        }

        // POST: BankType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditBankType")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BankType bankType)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.BankType.Find(bankType.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                bankType.AddUserId = orjData.AddUserId;
                bankType.AddDate = orjData.AddDate;
                bankType.UpdateUserId = _userId;
                bankType.UpdateDate = DateTime.Now;
                db.Entry(bankType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                newContext.Dispose();
                TempData["success"] = "Kayıt düzenlendi.";
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(bankType);
        }

        // GET: BankType/Delete/5
        [CustomAuth(Roles = "DeleteBankType")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankType bankType = await db.BankType.FindAsync(id);
            if (bankType == null)
            {
                return HttpNotFound();
            }
            return View(bankType);
        }

        // POST: BankType/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteBankType")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            BankType bankType = await db.BankType.FindAsync(id);
            bankType.IsDeleted = true;
            bankType.UpdateUserId = _userId;
            bankType.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
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
