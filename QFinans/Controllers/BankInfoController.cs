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
using Microsoft.AspNet.Identity;
using PagedList;
using QFinans.CustomFilters;

namespace QFinans.Controllers
{
    [Authorize]
    public class BankInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankInfo
        [CustomAuth(Roles = "IndexBankInfo")]
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

            IQueryable<BankInfo> bankInfo = db.BankInfo.Where(x => x.IsDeleted == false && x.IsArchive == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                bankInfo = bankInfo.Where(x => x.BankType.Name.Contains(searchString)
                                        || x.Name.Contains(searchString)
                                        || x.Surname.Contains(searchString)
                                        || x.AccountNumber.Contains(searchString)
                                        || x.BranchCode.Contains(searchString)
                                        || x.Iban.Contains(searchString)
                                        );

                if (bankInfo.Any() == false)
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
            return View(bankInfo.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "ArchiveBankInfo")]
        public ActionResult Archive(string currentFilter, string searchString, int? page, int? customPageSize)
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

            IQueryable<BankInfo> bankInfo = db.BankInfo.Where(x => x.IsDeleted == false && x.IsArchive == true).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                bankInfo = bankInfo.Where(x => x.BankType.Name.Contains(searchString)
                                        || x.Name.Contains(searchString)
                                        || x.Surname.Contains(searchString)
                                        || x.AccountNumber.Contains(searchString)
                                        || x.BranchCode.Contains(searchString)
                                        || x.Iban.Contains(searchString)
                                        );

                if (bankInfo.Any() == false)
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
            return View(bankInfo.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: BankInfo/Details/5
        [CustomAuth(Roles = "DetailsBankInfo")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = await db.BankInfo.FindAsync(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // GET: BankInfo/Create
        [CustomAuth(Roles = "CreateBankInfo")]
        public ActionResult Create()
        {
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name");
            return View();
        }

        // POST: BankInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BankInfo bankInfo)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                bankInfo.AddUserId = _userId;
                bankInfo.AddDate = DateTime.Now;
                db.BankInfo.Add(bankInfo);
                await db.SaveChangesAsync();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", bankInfo.BankTypeId);
            return View(bankInfo);
        }

        // GET: BankInfo/Edit/5
        [CustomAuth(Roles = "EditBankInfo")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = await db.BankInfo.FindAsync(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", bankInfo.BankTypeId);
            return View(bankInfo);
        }

        // POST: BankInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BankInfo bankInfo)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.BankInfo.Find(bankInfo.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                bankInfo.AddUserId = orjData.AddUserId;
                bankInfo.AddDate = orjData.AddDate;
                bankInfo.UpdateUserId = _userId;
                bankInfo.UpdateDate = DateTime.Now;
                db.Entry(bankInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                newContext.Dispose();
                TempData["success"] = "Kayıt düzenlendi.";
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", bankInfo.BankTypeId);
            return View(bankInfo);
        }

        // GET: BankInfo/Delete/5
        [CustomAuth(Roles = "DeleteBankInfo")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = await db.BankInfo.FindAsync(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // POST: BankInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            BankInfo bankInfo = await db.BankInfo.FindAsync(id);
            bankInfo.IsDeleted = true;
            bankInfo.UpdateUserId = _userId;
            bankInfo.UpdateDate = DateTime.Now;
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
