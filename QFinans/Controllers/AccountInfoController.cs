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
    public class AccountInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexAccountInfo")]
        // GET: HesapBilgileri
        public ActionResult Index(
            string currentFilter,
            string searchString,
            int? page,
            int? customPageSize,
            int? simLocationId,
            int? currentSimLocationId,
            int? accountInfoTypeId,
            int? currentAccountInfoTypeId,
            int? accountAmountRedirectId,
            int? currentAccountAmountRedirectId,
            string sortOrder,
            decimal? balanceFrom,
            decimal? currentBalanceFrom,
            decimal? balanceTo,
            decimal? currentBalanceTo,
            decimal? transactionCountDepositFrom,
            decimal? currentTransactionCountDepositFrom,
            decimal? transactionCountDepositTo,
            decimal? currentTransactionCountDepositTo
            )
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

            IQueryable<AccountInfo> accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false);

            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.TransactionCountDepositSortParm = sortOrder == "TransactionCountDeposit" ? "transactionCountDeposit_desc" : "TransactionCountDeposit";
            ViewBag.BalanceSortParm = sortOrder == "Balance" ? "balance_desc" : "Balance";

            switch (sortOrder)
            {
                case "id_asc":
                    accountInfo = accountInfo.OrderBy(s => s.Id);
                    break;
                case "TransactionCountDeposit":
                    accountInfo = accountInfo.OrderBy(s => s.TransactionCountDeposit);
                    break;
                case "transactionCountDeposit_desc":
                    accountInfo = accountInfo.OrderByDescending(s => s.TransactionCountDeposit);
                    break;
                case "Balance":
                    accountInfo = accountInfo.OrderBy(s => s.Balance);
                    break;
                case "balance_desc":
                    accountInfo = accountInfo.OrderByDescending(s => s.Balance);
                    break;
                default:
                    accountInfo = accountInfo.OrderByDescending(s => s.Id);
                    break;
            }

            //for select list
            ViewBag.AccountInfo = accountInfo;
            //var readyAccountInfo = accountInfo.Where(x => x.IsReady == true);
            //var activeAccountInfo = accountInfo.Where(x => x.IsPassive == false && x.IsReady == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountInfo = accountInfo.Where(x => x.Name.Contains(searchString)
                                                    || x.SurName.Contains(searchString)
                                                    || x.Phone.Contains(searchString)
                                                    || x.Email.Contains(searchString)
                                                    || x.Device.Contains(searchString)
                                                    || x.Id.ToString() == searchString
                                                    || x.AccountNumber.ToString().Contains(searchString));

                //readyAccountInfo = readyAccountInfo.Where(x => x.Name.Contains(searchString)
                //                                    || x.SurName.Contains(searchString)
                //                                    || x.Id.ToString() == searchString
                //                                    || x.AccountNumber.ToString().Contains(searchString));

                if (accountInfo.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            
            if (accountInfoTypeId != null) { page = 1; } else { accountInfoTypeId = currentAccountInfoTypeId; }
            ViewBag.CurrentAccountInfoTypeId = accountInfoTypeId;
            if (accountInfoTypeId.HasValue)
                accountInfo = accountInfo.Where(x => x.AccountInfoTypeId == accountInfoTypeId);

            if (accountAmountRedirectId != null) { page = 1; } else { accountAmountRedirectId = currentAccountAmountRedirectId; }
            ViewBag.CurrentAccountAmountRedirectId = accountAmountRedirectId;
            if (accountAmountRedirectId.HasValue)
                accountInfo = accountInfo.Where(x => x.AccountAmountRedirectId == accountAmountRedirectId);

            if (simLocationId != null) { page = 1; } else { simLocationId = currentSimLocationId; }
            ViewBag.CurrentSimLocationId = simLocationId;
            if (simLocationId.HasValue)
                accountInfo = accountInfo.Where(x => x.SimLocationId == simLocationId);

            if (balanceFrom != null) { page = 1; } else { balanceFrom = currentBalanceFrom; }
            ViewBag.CurrentBalanceFrom = balanceFrom;
            if (balanceFrom.HasValue)
                accountInfo = accountInfo.Where(x => x.Balance >= balanceFrom);

            if (balanceTo != null) { page = 1; } else { balanceTo = currentBalanceTo; }
            ViewBag.CurrentBalanceTo = balanceTo;
            if (balanceTo.HasValue)
                accountInfo = accountInfo.Where(x => x.Balance <= balanceTo);

            if (transactionCountDepositFrom != null) { page = 1; } else { transactionCountDepositFrom = currentTransactionCountDepositFrom; }
            ViewBag.CurrentTransactionCountDepositFrom = transactionCountDepositFrom;
            if (transactionCountDepositFrom.HasValue)
                accountInfo = accountInfo.Where(x => x.TransactionCountDeposit >= transactionCountDepositFrom);

            if (transactionCountDepositTo != null) { page = 1; } else { transactionCountDepositTo = currentTransactionCountDepositTo; }
            ViewBag.CurrentTransactionCountDepositTo = transactionCountDepositTo;
            if (transactionCountDepositTo.HasValue)
                accountInfo = accountInfo.Where(x => x.TransactionCountDeposit <= transactionCountDepositTo);

            int defaultPageSize = db.SystemParameters.Select(x => (x.AccountInfoPageSize == null ? 10 : x.AccountInfoPageSize)).FirstOrDefault() ?? 10;

            if (customPageSize != null)
            {
                ViewBag.CustomPageSize = customPageSize;
            }
            else
            {
                ViewBag.CustomPageSize = defaultPageSize;
            }

            //ViewBag.ReadyAccountInfo = readyAccountInfo.OrderByDescending(x => x.Id);
            //ViewBag.ActiveAccountInfo = activeAccountInfo.OrderByDescending(x => x.OrderNumber).ThenByDescending(x => x.Id);

            int pageSize = (customPageSize ?? defaultPageSize);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            
            return View(accountInfo.ToPagedList(pageNumber, pageSize));

        }

        [CustomAuth(Roles = "ArchiveAccountAccountInfo")]
        public ActionResult ArchiveAccount(string currentFilter, string searchString, int? page, int? customPageSize)
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
            DateTime _date = DateTime.Now;

            IQueryable<AccountInfo> accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == true).Include(h => h.AccountTransactions).OrderByDescending(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountInfo = accountInfo.Where(x => x.Name.Contains(searchString)
                                                    || x.SurName.Contains(searchString)
                                                    || x.Phone.Contains(searchString)
                                                    || x.Email.Contains(searchString)
                                                    || x.Device.Contains(searchString)
                                                    || x.Id.ToString() == searchString
                                                    || x.AccountNumber.ToString().Contains(searchString));

                if (accountInfo.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            int defaultPageSize = db.SystemParameters.Select(x => (x.AccountInfoPageSize == null ? 25 : x.AccountInfoPageSize)).FirstOrDefault() ?? 25;

            if (customPageSize != null)
            {
                ViewBag.CustomPageSize = customPageSize;
            }
            else
            {
                ViewBag.CustomPageSize = defaultPageSize;
            }

            int pageSize = (customPageSize ?? defaultPageSize);
            int pageNumber = (page ?? 1);
            //return View(db.AccountInfo.Where(x => x.IsDeleted == false).Include(x => x.AccountTransactions).ToList());
            return View(accountInfo.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));

        }

        [CustomAuth(Roles = "DetailsAccountInfo")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();

            if (accountInfo == null)
            {
                return HttpNotFound();
            }

            int[] drawSplitAccountTransactionsId = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountInfoId == accountInfo.Id).Select(x => x.AccountTransactionsId).Distinct().ToArray();

            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.AccountInfoId == accountInfo.Id || drawSplitAccountTransactionsId.Contains(x.Id));

            ViewBag.AccountTransactions = accountTransactions.Where(x => x.TransactionStatus == TransactionStatus.Confirm).OrderByDescending(x => x.Id);
            ViewBag.AccountTransactionsCount = accountTransactions.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DepositConfirmCount = accountTransactions.Where(x => x.Deposit == true).Count();
            ViewBag.DrawConfirmCount = accountTransactions.Where(x => x.Deposit == false).Count();

            IQueryable<CashFlow> cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.AccountInfoId == accountInfo.Id).Include(x => x.CashFlowType);

            ViewBag.CashFlow = cashFlow.OrderByDescending(x => x.Id);
            ViewBag.CashFlowCount = cashFlow.Count();

            return View(accountInfo);
        }

        // GET: HesapBilgileri/Create
        [CustomAuth(Roles = "CreateAccountInfo")]
        public ActionResult Create()
        {
            var accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (İşlem Limiti: " + x.TransactionLimit.ToString("N0") + ") (Tutar Limiti: " + x.AmountLimit.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountInfoTypeId = new SelectList(accountInfoType, "Value", "Text");

            var accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (Min Tutar: " + x.MinAmount.ToString("N0") + ") (Max Tutar: " + x.MaxAmount.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountAmountRedirectId = new SelectList(accountAmountRedirect, "Value", "Text");

            //var shiftTypeId = db.ShiftType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            //{
            //    Selected = false,
            //    Value = x.Id.ToString(),
            //    Text = x.Name + " (" + x.StartTime.ToString(@"hh\:mm") + " - " + x.EndTime.ToString(@"hh\:mm") + ")"
            //}).ToList();
            //ViewBag.shiftTypeId = new SelectList(shiftTypeId, "Value", "Text");

            ViewBag.SimLocationId = new SelectList(db.SimLocation.Where(x => x.IsDeleted == false), "Id", "Name");
            ViewBag.AccountInfoStatusId = new SelectList(db.AccountInfoStatus.Where(x => x.IsDeleted == false), "Id", "Name");

            return View();
        }

        // POST: HesapBilgileri/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountInfo accountInfo)
        {
            string _userId = User.Identity.GetUserId();

            var accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (İşlem Limiti: " + x.TransactionLimit.ToString("N0") + ") (Tutar Limiti: " + x.AmountLimit.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountInfoTypeId = new SelectList(accountInfoType, "Value", "Text", accountInfo.AccountInfoTypeId);

            var accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (Min Tutar: " + x.MinAmount.ToString("N0") + ") (Max Tutar: " + x.MaxAmount.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountAmountRedirectId = new SelectList(accountAmountRedirect, "Value", "Text", accountInfo.AccountAmountRedirectId);

            //var shiftTypeId = db.ShiftType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            //{
            //    Selected = false,
            //    Value = x.Id.ToString(),
            //    Text = x.Name + " (" + x.StartTime.ToString(@"hh\:mm") + " - " + x.EndTime.ToString(@"hh\:mm") + ")"
            //}).ToList();
            //ViewBag.shiftTypeId = new SelectList(shiftTypeId, "Value", "Text", accountInfo.ShiftTypeId);

            ViewBag.SimLocationId = new SelectList(db.SimLocation.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.SimLocationId);
            ViewBag.AccountInfoStatusId = new SelectList(db.AccountInfoStatus.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.AccountInfoStatusId);

            int accountCount = db.AccountInfo.Where(x => x.IsDeleted == false && x.AccountNumber == accountInfo.AccountNumber).Count();

            if (accountCount != 0 )
            {
                TempData["error"] = "Hesap daha önce eklendiği için aynı hesabı tekrar ekleyemezsiniz.";
                return View(accountInfo);
            }

            if (ModelState.IsValid)
            {
                accountInfo.AddUserId = _userId;
                accountInfo.AddDate = DateTime.Now;
                db.AccountInfo.Add(accountInfo);
                db.SaveChanges();
                TempData["success"] = "Hesap eklendi.";
                
                if (accountInfo.IsArchive == true)
                {
                    return RedirectToAction("ArchiveAccount");
                } else
                {
                    return RedirectToAction("Index");
                }
                
            }

            return View(accountInfo);
        }

        // GET: HesapBilgileri/Edit/5
        [CustomAuth(Roles = "EditAccountInfo")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }
            var accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (İşlem Limiti: " + x.TransactionLimit.ToString("N0") + ") (Tutar Limiti: " + x.AmountLimit.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountInfoTypeId = new SelectList(accountInfoType, "Value", "Text", accountInfo.AccountInfoTypeId);

            var accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (Min Tutar: " + x.MinAmount.ToString("N0") + ") (Max Tutar: " + x.MaxAmount.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountAmountRedirectId = new SelectList(accountAmountRedirect, "Value", "Text", accountInfo.AccountAmountRedirectId);
            ViewBag.AccountInfoStatusId = new SelectList(db.AccountInfoStatus.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.AccountInfoStatusId);

            //var shiftTypeId = db.ShiftType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            //{
            //    Selected = false,
            //    Value = x.Id.ToString(),
            //    Text = x.Name + " (" + x.StartTime.ToString(@"hh\:mm") + " - " + x.EndTime.ToString(@"hh\:mm") + ")"
            //}).ToList();
            //ViewBag.shiftTypeId = new SelectList(shiftTypeId, "Value", "Text", accountInfo.ShiftTypeId);

            ViewBag.SimLocationId = new SelectList(db.SimLocation.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.SimLocationId);

            return View(accountInfo);
        }

        // POST: HesapBilgileri/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountInfo accountInfo)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.AccountInfo.Find(accountInfo.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                accountInfo.TransactionCountDeposit = orjData.TransactionCountDeposit;
                accountInfo.TransactionCountDraw = orjData.TransactionCountDraw;
                accountInfo.ConfirmDepositSum = orjData.ConfirmDepositSum;
                accountInfo.ConfirmDepositSumNotFree = orjData.ConfirmDepositSumNotFree;
                accountInfo.ConfirmDepositSumCurrentMonth = orjData.ConfirmDepositSumCurrentMonth;
                accountInfo.ConfirmDepositSumCurrentMonthNotFree = orjData.ConfirmDepositSumCurrentMonthNotFree;
                accountInfo.ConfirmDrawSum = orjData.ConfirmDrawSum;
                accountInfo.ConfirmDrawSumCurrentMonth = orjData.ConfirmDrawSumCurrentMonth;
                accountInfo.DrawSplitSum = orjData.DrawSplitSum;
                accountInfo.DrawSplitSumCurrentMonth = orjData.DrawSplitSumCurrentMonth;
                accountInfo.CashInSum = orjData.CashInSum;
                accountInfo.CashInSumNotFree = orjData.CashInSumNotFree;
                accountInfo.CashInSumCurrentMonth = orjData.CashInSumCurrentMonth;
                accountInfo.CashInSumCurrentMonthNotFree = orjData.CashInSumCurrentMonthNotFree;
                accountInfo.CashOutSum = orjData.CashOutSum;
                accountInfo.CashOutSumCurrentMonth = orjData.CashOutSumCurrentMonth;
                accountInfo.Balance = orjData.Balance;
                accountInfo.BalanceCurrentMonth = orjData.BalanceCurrentMonth;
                accountInfo.BankCharge = orjData.BankCharge;
                accountInfo.BankChargeCurrentMonth = orjData.BankChargeCurrentMonth;
                accountInfo.LastUser = orjData.LastUser;

                accountInfo.TotalAmount = orjData.TotalAmount;
                accountInfo.AddUserId = orjData.AddUserId;
                accountInfo.AddDate = orjData.AddDate;
                accountInfo.UpdateUserId = _userId;
                accountInfo.UpdateDate = DateTime.Now;
                db.Entry(accountInfo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Hesap düzenlendi.";
                newContext.Dispose();
                
                if (accountInfo.IsArchive == true)
                {
                    return RedirectToAction("ArchiveAccount");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            newContext.Dispose();
            var accountInfoType = db.AccountInfoType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (İşlem Limiti: " + x.TransactionLimit.ToString("N0") + ") (Tutar Limiti: " + x.AmountLimit.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountInfoTypeId = new SelectList(accountInfoType, "Value", "Text", accountInfo.AccountInfoTypeId);

            var accountAmountRedirect = db.AccountAmountRedirect.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " (Min Tutar: " + x.MinAmount.ToString("N0") + ") (Max Tutar: " + x.MaxAmount.ToString("N0") + ")"
            }).ToList();
            ViewBag.AccountAmountRedirectId = new SelectList(accountAmountRedirect, "Value", "Text", accountInfo.AccountAmountRedirectId);

            //var shiftTypeId = db.ShiftType.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            //{
            //    Selected = false,
            //    Value = x.Id.ToString(),
            //    Text = x.Name + " (" + x.StartTime.ToString(@"hh\:mm") + " - " + x.EndTime.ToString(@"hh\:mm") + ")"
            //}).ToList();
            //ViewBag.shiftTypeId = new SelectList(shiftTypeId, "Value", "Text", accountInfo.ShiftTypeId);

            ViewBag.SimLocationId = new SelectList(db.SimLocation.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.SimLocationId);
            ViewBag.AccountInfoStatusId = new SelectList(db.AccountInfoStatus.Where(x => x.IsDeleted == false), "Id", "Name", accountInfo.AccountInfoStatusId);

            return View(accountInfo);
        }

        [CustomAuth(Roles = "TotalAmountAccountInfo")]
        public ActionResult TotalAmount(string currentFilter, string searchString, int? page, int? customPageSize)
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

            //DateTime date = DateTime.Now.Date;
            //DateTime startDate = new DateTime(date.Year, date.Month, 1);
            //DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            DateTime startDate = db.AccountTransactions.OrderBy(x => x.AddDate).Select(x => x.AddDate).FirstOrDefault();
            DateTime endDate = db.AccountTransactions.OrderByDescending(x => x.AddDate).Select(x => x.AddDate).FirstOrDefault().AddMonths(1);

            //decimal _bankChargeRatio = 3.4M;

            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.AddDate >= startDate && x.AddDate <= endDate);
            IQueryable<CashFlow> cashFlow = db.CashFlow.Where(x => x.IsDeleted == false && x.TransactionDate >= startDate && x.TransactionDate <= endDate);

            IQueryable<DrawSplit> drawSplit = db.DrawSplit.Where(a => a.IsDeleted == false && a.AccountTransactions.AddDate >= startDate && a.AccountTransactions.AddDate <= endDate && a.AccountTransactions.TransactionStatus != TransactionStatus.Deny);

            IQueryable<AccountInfo> accountInfo = db.AccountInfo.Where(a => a.IsDeleted == false);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountInfo = accountInfo.Where(x => x.Name.Contains(searchString)
                                                    || x.SurName.Contains(searchString)
                                                    || x.AccountNumber.ToString().Contains(searchString));

                if (accountInfo.Any() == false)
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
            //return View(db.AccountInfo.Where(x => x.IsDeleted == false).Include(x => x.AccountTransactions).ToList());
            return View(accountInfo.OrderByDescending(x => x.Balance).ToPagedList(pageNumber, pageSize));

        }

        [CustomAuth(Roles = "EditTotalAmountAccountInfo")]
        public ActionResult EditTotalAmount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }
            return View(accountInfo);
        }

        [HttpPost]
        [CustomAuth(Roles = "EditTotalAmountAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult EditTotalAmount(int? id, decimal? totalAmount)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }
            accountInfo.UpdateUserId = _userId;
            accountInfo.UpdateDate = DateTime.Now;
            accountInfo.TotalAmount = totalAmount;
            db.SaveChanges();
            TempData["success"] = "Hesap bakiyesi düzenlendi.";
            return RedirectToAction("TotalAmount");

        }

        // GET: HesapBilgileri/Delete/5
        [CustomAuth(Roles = "DeleteAccountInfo")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }
            return View(accountInfo);
        }

        // POST: HesapBilgileri/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            AccountInfo accountInfo = db.AccountInfo.Find(id);
            //db.HesapBilgileri.Remove(hesapBilgileri);
            accountInfo.IsDeleted = true;
            accountInfo.UpdateUserId = _userId;
            accountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Hesap silindi.";
            
            if (accountInfo.IsArchive == true)
            {
                return RedirectToAction("ArchiveAccount");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [CustomAuth(Roles = "SendArchieveAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult SendArchieve(int? id)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }
           
            accountInfo.IsArchive = true;
            accountInfo.UpdateUserId = _userId;
            accountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Hesap arşivlendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuth(Roles = "SetPassiveAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult SetPassive(int? id)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }

            accountInfo.IsPassive = true;
            accountInfo.UpdateUserId = _userId;
            accountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Hesap pasifleştirildi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuth(Roles = "SetActiveAccountInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult SetActive(int? id, int? accountAmountRedirectId)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountInfo accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.Id == id).FirstOrDefault();
            if (accountInfo == null)
            {
                return HttpNotFound();
            }

            if(accountAmountRedirectId == null)
            {
                TempData["warning"] = "Lütfen tutar yönlendirme seçiniz.";
                return RedirectToAction("Index");
            }

            accountInfo.IsPassive = false;
            accountInfo.AccountAmountRedirectId = accountAmountRedirectId;
            accountInfo.UpdateUserId = _userId;
            accountInfo.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Hesap aktifleştirildi.";
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
