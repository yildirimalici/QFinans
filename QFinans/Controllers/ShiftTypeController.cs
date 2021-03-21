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
    public class ShiftTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexShiftType")]
        // GET: ShiftType
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

            IQueryable<ShiftType> shift = db.ShiftType.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                shift = shift.Where(x => x.Name.Contains(searchString));

                if (shift.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false).OrderByDescending(x => x.Id);
            ViewBag.AccountInfo = accountInfo;

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
            return View(shift.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsShiftType")]
        // GET: ShiftType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftType shift = db.ShiftType.Where(x => x.IsDeleted == false && x.Id == id).Include(x => x.ShiftTypeAccountInfo).FirstOrDefault();
            if (shift == null)
            {
                return HttpNotFound();
            }

            var accountInfoIds = db.ShiftTypeAccountInfo.Where(x => x.IsDeleted == false && x.ShiftTypeId == shift.Id).Select(x => x.AccountInfoId).ToArray();

            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && !accountInfoIds.Contains(x.Id)).OrderByDescending(x => x.Id);
            ViewBag.AccountInfo = accountInfo;

            var accountInfoEdit = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false).OrderByDescending(x => x.Id);
            ViewBag.AccountInfoEdit = accountInfoEdit;

            return View(shift);
        }

        [CustomAuth(Roles = "CreateShiftType")]
        // GET: ShiftType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShiftType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShiftType shift)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                shift.AddUserId = _userId;
                shift.AddDate = DateTime.Now;
                db.ShiftType.Add(shift);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            return View(shift);
        }

        [CustomAuth(Roles = "EditShiftType")]
        // GET: ShiftType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftType shift = db.ShiftType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // POST: ShiftType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShiftType shift)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.ShiftType.Find(shift.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                shift.AddUserId = orjData.AddUserId;
                shift.AddDate = orjData.AddDate;
                shift.UpdateUserId = _userId;
                shift.UpdateDate = DateTime.Now;
                db.Entry(shift).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            return View(shift);
        }

        [CustomAuth(Roles = "DeleteShiftType")]
        // GET: ShiftType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftType shift = db.ShiftType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // POST: ShiftType/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            ShiftType shift = db.ShiftType.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            shift.IsDeleted = true;
            shift.UpdateUserId = _userId;
            shift.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Kayıt silindi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuth(Roles = "AddAccountInfoShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccountInfo(int shiftTypeId, int accountInfoId, int? action, int? accountAmountRedirectId)
        {
            string _userId = User.Identity.GetUserId();

            try
            {
                if (db.ShiftTypeAccountInfo.Where(x => x.IsDeleted == false && x.IsClosed == false && x.ShiftTypeId == shiftTypeId).Any(x => x.AccountInfoId == accountInfoId))
                {
                    TempData["warning"] = "Aynı shifte yalnızca bir hesap ekleyebilirsiniz.";
                } else
                {
                    ShiftTypeAccountInfo shiftTypeAccountInfo = new ShiftTypeAccountInfo();
                    shiftTypeAccountInfo.ShiftTypeId = shiftTypeId;
                    shiftTypeAccountInfo.AccountInfoId = accountInfoId;
                    shiftTypeAccountInfo.AddUserId = _userId;
                    shiftTypeAccountInfo.AddDate = DateTime.Now;
                    db.ShiftTypeAccountInfo.Add(shiftTypeAccountInfo);
                    db.SaveChanges();
                    TempData["success"] = "Kayıt eklendi.";
                }

                if (accountAmountRedirectId != null)
                {
                    AccountInfo accountInfo = db.AccountInfo.Find(accountInfoId);
                    accountInfo.AccountAmountRedirectId = accountAmountRedirectId;
                    accountInfo.UpdateUserId = _userId;
                    accountInfo.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["danger"] = ex.Message;
            }

            return action > 0
                ? RedirectToAction("Index", "AccountInfo")
                : RedirectToAction("Details", new { id = shiftTypeId });
        }

        [HttpPost]
        [CustomAuth(Roles = "EditAccountInfoShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccountInfo(int shiftTypeAccountInfoId, int accountInfoId, int? action)
        {
            string _userId = User.Identity.GetUserId();
            ShiftTypeAccountInfo shiftTypeAccountInfo = db.ShiftTypeAccountInfo.Find(shiftTypeAccountInfoId);
            if (shiftTypeAccountInfo == null)
            {
                return HttpNotFound();
            }

            try
            {
                shiftTypeAccountInfo.AccountInfoId = accountInfoId;
                shiftTypeAccountInfo.UpdateUserId = _userId;
                shiftTypeAccountInfo.UpdateDate = DateTime.Now;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
            }
            catch (Exception ex)
            {
                TempData["danger"] = ex.Message;
            }

            return action > 0
                ? RedirectToAction("Index", "AccountInfo")
                : RedirectToAction("Details", new { id = shiftTypeAccountInfo.ShiftTypeId });
        }

        [CustomAuth(Roles = "CloseAccountInfoShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseAccountInfo(int id, int? action)
        {
            string _userId = User.Identity.GetUserId();
            ShiftTypeAccountInfo shiftTypeAccountInfo = db.ShiftTypeAccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();

            AccountInfo accountInfo = db.AccountInfo.Find(shiftTypeAccountInfo.AccountInfoId);

            //AccountInfoViewModel accountInfoViewModel = new AccountInfoViewModel();
            //accountInfoViewModel.Id = accountInfo.Id;
            //accountInfoViewModel.Name = accountInfo.Name;
            //accountInfoViewModel.SurName = accountInfo.SurName;
            //accountInfoViewModel.AccountNumber = accountInfo.AccountNumber;
            //accountInfoViewModel.AccountInfoTypeId = accountInfo.AccountInfoTypeId;
            //accountInfoViewModel.AccountAmountRedirectId = accountInfo.AccountAmountRedirectId;
            //accountInfoViewModel.IsPassive = accountInfo.IsPassive;
            //accountInfoViewModel.OrderNumber = accountInfo.OrderNumber;
            //accountInfoViewModel.IsArchive = accountInfo.IsArchive;

            shiftTypeAccountInfo.Balance = accountInfo.Balance;
            shiftTypeAccountInfo.IsClosed = true;
            shiftTypeAccountInfo.StartTime = shiftTypeAccountInfo.ShiftType.StartTime;
            shiftTypeAccountInfo.EndTime = shiftTypeAccountInfo.ShiftType.EndTime;
            shiftTypeAccountInfo.ClosedUserId = _userId;
            shiftTypeAccountInfo.UpdateUserId = _userId;
            shiftTypeAccountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Kayıt çıkarıldı.";

            return action > 0
                ? RedirectToAction("Index", "AccountInfo")
                : RedirectToAction("Details", new { id = shiftTypeAccountInfo.ShiftTypeId });
        }

        [CustomAuth(Roles = "DeleteAccountInfoShiftType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountInfo(int id, int? action)
        {
            string _userId = User.Identity.GetUserId();
            ShiftTypeAccountInfo shiftTypeAccountInfo = db.ShiftTypeAccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            shiftTypeAccountInfo.IsDeleted = true;
            shiftTypeAccountInfo.UpdateUserId = _userId;
            shiftTypeAccountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Kayıt silindi.";

            return action > 0
                ? RedirectToAction("Index", "AccountInfo")
                : RedirectToAction("Details", new { id = shiftTypeAccountInfo.ShiftTypeId });
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
