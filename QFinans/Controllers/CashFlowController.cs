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
    public class CashFlowController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexCashFlow")]
        // GET: CashFlow
        public ActionResult Index(string currentFilter, string searchString, int? page, int? customPageSize, int? accountInfoId, int? currentAccountInfoId, DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo, int? cashFlowTypeId, int? currentCashFlowTypeId)
        {
            IOrderedQueryable<AccountInfo> accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id);
            ViewBag.AccountInfo = accountInfo;

            IOrderedQueryable<CashFlowType> cashFlowType = db.CashFlowType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id);
            ViewBag.CashFlowType = cashFlowType;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IQueryable<CashFlow> cashFlow = db.CashFlow.Where(x => x.IsDeleted == false).Include(c => c.AccountInfo).OrderBy(x => x.Id);

            string _userId = User.Identity.GetUserId();
            var _user = db.Users.Find(_userId);
            DateTime date = DateTime.Now.Date;
            DateTime startDate = date.AddHours(-1);
            DateTime endDate = date.AddDays(1).AddSeconds(-1);

            if (_user.IsAdmin == false && _user.IsShowCashFlow == false)
            {
                cashFlow = cashFlow.Where(x => (x.AddUserId == _userId || x.CashFlowType.IsOtherUserSee == true) && x.TransactionDate >= startDate && x.TransactionDate <= endDate);
            } 

            if (accountInfoId != null) { page = 1; } else { accountInfoId = currentAccountInfoId; }
            ViewBag.CurrentAccountInfoId = accountInfoId;
            if (accountInfoId.HasValue)
                cashFlow = cashFlow.Where(x => x.AccountInfoId == accountInfoId);

            if (!String.IsNullOrEmpty(searchString))
            {
                cashFlow = cashFlow.Where(x => x.Explanation.Contains(searchString) || x.AccountInfo.Name.Contains(searchString) || x.AccountInfo.SurName.Contains(searchString));

                if (cashFlow.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            if (dateFrom != null) { page = 1; } else { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                cashFlow = cashFlow.Where(x => x.TransactionDate >= dateFrom);

            if (dateTo != null) { page = 1; } else { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                cashFlow = cashFlow.Where(x => x.TransactionDate <= dateTo);

            if (cashFlowTypeId != null) { page = 1; } else { cashFlowTypeId = currentCashFlowTypeId; }
            ViewBag.CurrentCashFlowTypeId = cashFlowTypeId;
            if (cashFlowTypeId.HasValue)
                cashFlow = cashFlow.Where(x => x.CashFlowTypeId == cashFlowTypeId);

            ViewBag.CashFlowSum = cashFlow.Where(x => x.IsCashIn == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum() - cashFlow.Where(x => x.IsCashIn == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

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
            return View(cashFlow.OrderByDescending(x => x.TransactionDate).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsCashFlow")]
        // GET: CashFlow/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlow cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlow == null)
            {
                return HttpNotFound();
            }
            return View(cashFlow);
        }

        [CustomAuth(Roles = "CreateCashFlow")]
        // GET: CashFlow/Create
        public ActionResult Create()
        {
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text");

            ViewBag.CashFlowTypeId = new SelectList(db.CashFlowType.Where(x => x.IsDeleted == false), "Id", "Name");
            return View();
        }

        // POST: CashFlow/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateCashFlow")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CashFlow cashFlow)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                cashFlow.AddUserId = _userId;
                cashFlow.AddDate = DateTime.Now;
                db.CashFlow.Add(cashFlow);
                db.SaveChanges();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }

            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", cashFlow.AccountInfoId);
            ViewBag.CashFlowTypeId = new SelectList(db.CashFlowType.Where(x => x.IsDeleted == false), "Id", "Name", cashFlow.CashFlowTypeId);
            return View(cashFlow);
        }

        [CustomAuth(Roles = "EditCashFlow")]
        // GET: CashFlow/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlow cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlow == null)
            {
                return HttpNotFound();
            }
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", cashFlow.AccountInfoId);
            ViewBag.CashFlowTypeId = new SelectList(db.CashFlowType.Where(x => x.IsDeleted == false), "Id", "Name", cashFlow.CashFlowTypeId);
            return View(cashFlow);
        }

        // POST: CashFlow/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditCashFlow")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CashFlow cashFlow)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.CashFlow.Find(cashFlow.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                cashFlow.AddUserId = orjData.AddUserId;
                cashFlow.AddDate = orjData.AddDate;
                cashFlow.UpdateUserId = _userId;
                cashFlow.UpdateDate = DateTime.Now;
                db.Entry(cashFlow).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Kayıt düzenlendi.";
                newContext.Dispose();
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", cashFlow.AccountInfoId);
            ViewBag.CashFlowTypeId = new SelectList(db.CashFlowType.Where(x => x.IsDeleted == false), "Id", "Name", cashFlow.CashFlowTypeId);
            return View(cashFlow);
        }

        [CustomAuth(Roles = "DeleteCashFlow")]
        // GET: CashFlow/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashFlow cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (cashFlow == null)
            {
                return HttpNotFound();
            }
            return View(cashFlow);
        }

        // POST: CashFlow/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteCashFlow")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            CashFlow cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            cashFlow.IsDeleted = true;
            cashFlow.UpdateUserId = _userId;
            cashFlow.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Kayıt silindi.";
            return RedirectToAction("Index");
        }

        [CustomAuth(Roles = "CreateCashFlow")]
        public ActionResult AddTransfer(int fromAccountInfoId, int toAccountInfoId, DateTime transactionDate, decimal amount, bool isFree, string explanation)
        {
            string _userId = User.Identity.GetUserId();
            
            try
            {
                var _toAccountInfo = db.AccountInfo.Find(toAccountInfoId);
                var _fromAccountInfo = db.AccountInfo.Find(fromAccountInfoId);

                using (var context = new ApplicationDbContext())
                {
                    CashFlow cashFlowFrom = new CashFlow();
                    cashFlowFrom.IsTransfer = true;
                    cashFlowFrom.AccountInfoId = fromAccountInfoId;
                    cashFlowFrom.TransactionDate = transactionDate;
                    cashFlowFrom.IsCashIn = false;
                    cashFlowFrom.Amount = amount;
                    cashFlowFrom.IsFree = false;
                    cashFlowFrom.Explanation = "(" + _toAccountInfo.Id.ToString() + " " + _toAccountInfo.Name + " " + _toAccountInfo.SurName + " - " + _toAccountInfo.AccountNumber.ToString() + ") " + explanation;
                    cashFlowFrom.AddUserId = _userId;
                    cashFlowFrom.AddDate = DateTime.Now;
                    context.CashFlow.Add(cashFlowFrom);
                    context.SaveChanges();
                }

                using (var context = new ApplicationDbContext())
                {
                    CashFlow cashFlowTo = new CashFlow();
                    cashFlowTo.IsTransfer = true;
                    cashFlowTo.AccountInfoId = toAccountInfoId;
                    cashFlowTo.TransactionDate = transactionDate;
                    cashFlowTo.IsCashIn = true;
                    cashFlowTo.Amount = amount;
                    cashFlowTo.IsFree = isFree;
                    cashFlowTo.Explanation = "(" + _fromAccountInfo.Id.ToString() + " " + _fromAccountInfo.Name + " " + _fromAccountInfo.SurName + " - " + _fromAccountInfo.AccountNumber.ToString() + ") " + explanation;
                    cashFlowTo.AddUserId = _userId;
                    cashFlowTo.AddDate = DateTime.Now;
                    context.CashFlow.Add(cashFlowTo);
                    context.SaveChanges();
                }

                TempData["success"] = "Kayıt eklendi";
            }
            catch (Exception ex)
            {
                TempData["danger"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuth(Roles = "IndexCashFlow")]
        public JsonResult GetType(int id)
        {

            string data = db.CashFlowType.Find(id).IsExplanationRequired.ToString();

            JsonObjectViewModel jsonObject = new JsonObjectViewModel
            {
                type = "success",
                message = data
            };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
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
