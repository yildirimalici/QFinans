using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System.Globalization;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class AccountTransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "DepositAccountTransactions")]
        // GET: Islemler
        public ActionResult Deposit(
            string currentFilter,
            string searchString,
            int? page,
            int? customPageSize,
            DateTime? currentDateFrom,
            DateTime? currentDateTo,
            DateTime? dateFrom,
            DateTime? dateTo,
            TransactionStatus? transactionStatus,
            TransactionStatus? currentTransactionStatus,
            int? accountInfoId,
            int? currentAccountInfoId
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

            DateTime date = DateTime.Now.Date;
            DateTime startDate = currentDateFrom ?? new DateTime(date.Year, date.Month, 1).AddHours(-1);

            //IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.Deposit == true && x.AddDate >= startDate).Include(x => x.AccountInfo);
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.Deposit == true).Include(x => x.AccountInfo);

            if (transactionStatus != null) { page = 1; } else { transactionStatus = currentTransactionStatus; }
            ViewBag.CurrentTransactionStatusId = transactionStatus;
            if (transactionStatus.HasValue)
                accountTransactions = accountTransactions.Where(x => x.TransactionStatus == transactionStatus);

            if (dateFrom != null) { page = 1; } else { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate >= dateFrom);

            if (dateTo != null) { page = 1; } else { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate <= dateTo);

            if (accountInfoId != null) { page = 1; } else { accountInfoId = currentAccountInfoId; }
            ViewBag.CurrentAccountInfoId = accountInfoId;
            if (accountInfoId.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AccountInfoId == accountInfoId);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountTransactions = accountTransactions.Where(x => x.UserName.Contains(searchString)
                                                        || x.Name.Contains(searchString)
                                                        || x.SurName.Contains(searchString)
                                                        || x.Id.ToString() == searchString
                                                        || (x.Name + " " + x.SurName).Contains(searchString)
                                                        || x.AccountInfo.AccountNumber.ToString().Contains(searchString));

                if (accountTransactions.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            int defaultPageSize = db.SystemParameters.Select(x => (x.DepositPageSize == null ? 10 : x.DepositPageSize)).FirstOrDefault() ?? 10;

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
            //var accountTransactions = db.AccountTransactions.Where(x => x.Deposit == true).Include(h => h.AccountInfo).OrderByDescending(x => x.AddDate);
            return View(accountTransactions.OrderBy(x => x.TransactionStatus).ThenByDescending(x => x.AddDate).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DrawAccountTransactions")]
        public ActionResult Draw(
            string currentFilter,
            string searchString,
            int? page,
            int? customPageSize,
            DateTime? currentDateFrom,
            DateTime? currentDateTo,
            DateTime? dateFrom,
            DateTime? dateTo,
            TransactionStatus? transactionStatus,
            TransactionStatus? currentTransactionStatus,
            int? accountInfoId,
            int? currentAccountInfoId
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

            DateTime date = DateTime.Now.Date;
            DateTime startDate = currentDateFrom ?? new DateTime(date.Year, date.Month, 1).AddHours(-1);

            //IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.Deposit == false && x.AddDate >= startDate).Include(h => h.AccountInfo).Include(h => h.DrawSplit);
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.Deposit == false).Include(h => h.AccountInfo).Include(h => h.DrawSplit);

            if (dateFrom != null) { page = 1; } else { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate >= dateFrom);

            if (dateTo != null) { page = 1; } else { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate <= dateTo);

            if (transactionStatus != null) { page = 1; } else { transactionStatus = currentTransactionStatus; }
            ViewBag.CurrentTransactionStatus = transactionStatus;
            if (transactionStatus.HasValue)
                accountTransactions = accountTransactions.Where(x => x.TransactionStatus == transactionStatus);

            if (accountInfoId != null) { page = 1; } else { accountInfoId = currentAccountInfoId; }
            ViewBag.CurrentAccountInfoId = accountInfoId;
            if (accountInfoId.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AccountInfoId == accountInfoId);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountTransactions = accountTransactions.Where(x => x.UserName.Contains(searchString)
                                                        || x.Name.Contains(searchString)
                                                        || x.SurName.Contains(searchString)
                                                        || x.Id.ToString() == searchString
                                                        || (x.Name + " " + x.SurName).Contains(searchString)
                                                        || x.AccountInfo.AccountNumber.ToString().Contains(searchString));

                if (accountTransactions.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            int defaultPageSize = db.SystemParameters.Select(x => (x.DrawPageSize == null ? 10 : x.DrawPageSize)).FirstOrDefault() ?? 10;

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

            //DateTime date = DateTime.Now.Date;
            //DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            //DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            //var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            //{
            //    Selected = false,
            //    Value = x.Id.ToString(),
            //    Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString()
            //    + ") (Çekim: "+ db.AccountTransactions.Where(t => t.Deposit == false && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count().ToString()

            //    + ") (Bakiye: " + (
            //        db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus != TransactionStatus.Deny).Select(s => s.Amount).DefaultIfEmpty(0).Sum()
            //        - (db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus != TransactionStatus.Deny).Select(s => s.Amount).DefaultIfEmpty(0).Sum() * 3.4M / 100)
            //        - db.AccountTransactions.Where(t => t.Deposit == false && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Select(s => s.Amount).DefaultIfEmpty(0).Sum()
            //        ).ToString("N0")
            //    + ")"
            //}).ToList();
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text");
            return View(accountTransactions.OrderBy(x => x.TransactionStatus).ThenByDescending(x => x.AddDate).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsAccountTransactions")]
        // GET: Islemler/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }
            return View(accountTransactions);
        }

        // GET: Islemler/Create
        [CustomAuth(Roles = "CreateDepositAccountTransactions")]
        public ActionResult CreateDeposit()
        {
            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text");
            //ViewBag.HesapBilgileriId = new SelectList(db.HesapBilgileri, "Id", "Isim");
            return View();
        }

        // POST: Islemler/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateDepositAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeposit(AccountTransactions accountTransactions)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var account = db.AccountInfo.Find(accountTransactions.AccountInfoId);
                accountTransactions.Deposit = true;
                accountTransactions.TransactionStatus = TransactionStatus.New;
                accountTransactions.OldAmount = accountTransactions.Amount;
                accountTransactions.Location = "web_panel";
                accountTransactions.AddUserId = _userId;
                accountTransactions.AddDate = DateTime.Now;

                accountTransactions.AccountInfoId = accountTransactions.AccountInfoId;

                db.AccountTransactions.Add(accountTransactions);
                db.SaveChanges();
                TempData["success"] = "Talep oluşturuldu.";
                return RedirectToAction("Deposit");
            }

            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);

            //ViewBag.HesapBilgileriId = new SelectList(db.HesapBilgileri, "Id", "Isim", hesapIslemleri.HesapBilgileriId);
            return View(accountTransactions);
        }

        // GET: Islemler/Create
        [CustomAuth(Roles = "CreateDrawAccountTransactions")]
        public ActionResult CreateDraw()
        {
            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text");
            //ViewBag.HesapBilgileriId = new SelectList(db.HesapBilgileri, "Id", "Isim");
            return View();
        }

        // POST: Islemler/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateDrawAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDraw(AccountTransactions accountTransactions)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                accountTransactions.Deposit = false;
                accountTransactions.TransactionStatus = TransactionStatus.New;
                accountTransactions.OldAmount = accountTransactions.Amount;
                accountTransactions.Location = "web_panel";
                accountTransactions.AddUserId = _userId;
                accountTransactions.AddDate = DateTime.Now;
                db.AccountTransactions.Add(accountTransactions);
                db.SaveChanges();
                TempData["success"] = "Talep oluşturuldu.";
                return RedirectToAction("Draw");
            }

            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);

            //ViewBag.HesapBilgileriId = new SelectList(db.HesapBilgileri, "Id", "Isim", hesapIslemleri.HesapBilgileriId);
            return View(accountTransactions);
        }

        [CustomAuth(Roles = "EditAccountTransactions")]
        // GET: Islemler/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ") (Yatırım: " + db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count().ToString() + ") (Çekim: " + db.AccountTransactions.Where(t => t.Deposit == false && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count().ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);
            //ViewBag.HesapBilgileriId = new SelectList(db.HesapBilgileri, "Id", "Isim", hesapIslemleri.HesapBilgileriId);

            ViewBag.AccountInfo = db.AccountInfo.Where(x => x.IsDeleted == false);

            var drawSplit = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == id);
            ViewBag.DrawSplit = drawSplit;

            return View(accountTransactions);
        }

        // POST: Islemler/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountTransactions accountTransactions)
        {
            string _userId = User.Identity.GetUserId();
            var orjData = db.AccountTransactions.Find(accountTransactions.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                orjData.UpdateUserId = _userId;
                orjData.UpdateDate = DateTime.Now;
                orjData.Amount = accountTransactions.Amount;
                orjData.AccountInfoId = accountTransactions.AccountInfoId;
                orjData.IsFree = accountTransactions.IsFree;
                orjData.Note = accountTransactions.Note;

                db.SaveChanges();
              
                TempData["success"] = "Talep düzenlendi.";
                if (orjData.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                } else
                {
                    return RedirectToAction("Draw");
                }
            }

            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ") (Yatırım: " + db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count().ToString() + ") (Çekim: " + db.AccountTransactions.Where(t => t.Deposit == false && t.AccountInfoId == x.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count().ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);
            
            return View(accountTransactions);
        }

        [CustomAuth(Roles = "ConfirmDepositAccountTransactions")]
        public ActionResult ConfirmDeposit(int? id, bool isFree)
        {
            string _userId = User.Identity.GetUserId();

            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            accountTransactions.IsFree = isFree;
            accountTransactions.TransactionStatus = TransactionStatus.Confirm;
            accountTransactions.UpdateUserId = _userId;
            accountTransactions.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Talep onaylandı.";

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                return RedirectToAction("CallBackApi", "AccountTransactions", new { transid = id });

            }
            else if (Request.Url.Host == "nano.qfinans.com")
            {
                return RedirectToAction("CallBackApiHash", "AccountTransactions", new { transid = id });
                //TempData["warning"] = "Callback api çalışmadı.";
                //return RedirectToAction("Deposit");
            }
            else if (Request.Url.Host == "www.pppropanel.com" || Request.Url.Host == "pppropanel.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else if (Request.Url.Host == "www.iprimepay.com" || Request.Url.Host == "iprimepay.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else
            {
                TempData["warning"] = "Callback api çalışmadı.";
                return RedirectToAction("Deposit");
            }

            
        }

        [CustomAuth(Roles = "ConfirmDrawAccountTransactions")]
        public ActionResult ConfirmDraw(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderBy(x => x.IsPassive).ThenByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);

            ViewBag.AccountInfo = db.AccountInfo.Where(x => x.IsDeleted == false);

            var drawSplit = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == id);
            ViewBag.DrawSplit = drawSplit;

            return View(accountTransactions);

        }

        [HttpPost]
        [CustomAuth(Roles = "ConfirmDrawAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDraw(int? id, int? accountInfoId)
        {
            string _userId = User.Identity.GetUserId();
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.SurName + " (" + x.AccountNumber.ToString() + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(accountInfo, "Value", "Text", accountTransactions.AccountInfoId);

            ViewBag.AccountInfo = db.AccountInfo.Where(x => x.IsDeleted == false);

            var drawSplit = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == id);
            ViewBag.DrawSplit = drawSplit;

            var drawSplitAmount = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == accountTransactions.Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            var drawSplitCount = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == accountTransactions.Id).Count();

            if (drawSplitCount > 0 && accountTransactions.Amount != drawSplitAmount)
            {
                TempData["warning"] = "Yatırım tutarı ile bölme işleminin toplam tutarı eşlememektedir.";
                return View(accountTransactions);
            }

            if (drawSplitCount == 0 && accountInfoId == null && accountTransactions.IsCoin == false)
            {
                TempData["warning"] = "Lütfen hesap seçiniz.";
                return View(accountTransactions);
            }

            accountTransactions.AccountInfoId = accountInfoId;
            accountTransactions.TransactionStatus = TransactionStatus.Confirm;
            accountTransactions.UpdateUserId = _userId;
            accountTransactions.UpdateDate = DateTime.Now;
            db.SaveChanges();
            TempData["success"] = "Talep onaylandı.";

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                return RedirectToAction("CallBackApi", "AccountTransactions", new { transid = id });

            }
            else if (Request.Url.Host == "nano.qfinans.com")
            {
                return RedirectToAction("CallBackApiHash", "AccountTransactions", new { transid = id });
                //TempData["warning"] = "Callback api çalışmadı.";
                //return RedirectToAction("Draw");
            }
            else if (Request.Url.Host == "www.pppropanel.com" || Request.Url.Host == "pppropanel.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else if (Request.Url.Host == "www.iprimepay.com" || Request.Url.Host == "iprimepay.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else
            {
                TempData["warning"] = "Callback api çalışmadı.";
                return RedirectToAction("Draw");
            }

        }

        [CustomAuth(Roles = "EditCoinAccountTransactions")]
        public ActionResult EditCoin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string _userId = User.Identity.GetUserId();
            AccountTransactions accountTransactions = db.AccountTransactions.Where(x => x.Id == id && x.IsCoin == true).FirstOrDefault();

            return View(accountTransactions);
        }

        [HttpPost]
        [CustomAuth(Roles = "EditCoinAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult EditCoin(int? id, AccountTransactions transactions)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string _userId = User.Identity.GetUserId();
            AccountTransactions accountTransactions = db.AccountTransactions.Where(x => x.Id == id && x.IsCoin == true).FirstOrDefault();
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            accountTransactions.Amount = transactions.Amount;
            accountTransactions.Note = transactions.Note;
            accountTransactions.UpdateUserId = _userId;
            accountTransactions.UpdateDate = DateTime.Now;
            db.SaveChanges();

            TempData["warning"] = "Kayıt düzenlendi.";
            if (accountTransactions.Deposit == true)
            {
                return RedirectToAction("Deposit");
            }
            else
            {
                return RedirectToAction("Draw");
            }
        }

        [CustomAuth(Roles = "ConfirmDepositAccountTransactions")]
        public ActionResult ConfirmCoin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string _userId = User.Identity.GetUserId();
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            accountTransactions.TransactionStatus = TransactionStatus.Confirm;
            accountTransactions.UpdateUserId = _userId;
            accountTransactions.UpdateDate = DateTime.Now;
            db.SaveChanges();
            
            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                return RedirectToAction("CallBackApi", "AccountTransactions", new { transid = id });

            }
            else if (Request.Url.Host == "nano.qfinans.com")
            {
                return RedirectToAction("CallBackApiHash", "AccountTransactions", new { transid = id });
            }
            else
            {
                TempData["warning"] = "Callback api çalışmadı.";
                if(accountTransactions.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                }
                else
                {
                    return RedirectToAction("Draw");
                }
            }
        }


        [CustomAuth(Roles = "DenyAccountTransactions")]
        public ActionResult Deny(int? id)
        {
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            if (accountTransactions.Deposit == true)
            {
                return RedirectToAction("Deposit");
            }
            else
            {
                return RedirectToAction("Draw");
            }
        }

        [HttpPost]
        [CustomAuth(Roles = "DenyAccountTransactions")]
        [ValidateAntiForgeryToken]
        public ActionResult Deny(int? id, string aciklama)
        {
            string _userId = User.Identity.GetUserId();
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                return HttpNotFound();
            }

            //if(accountTransactions.Deposit == true && accountTransactions.TransactionStatus != TransactionStatus.Deny)
            //{
            //    accountTransactions.AccountInfo.TransactionCount += -1;
            //}

            accountTransactions.TransactionStatus = TransactionStatus.Deny;
            accountTransactions.Note = aciklama;
            accountTransactions.UpdateUserId = _userId;
            accountTransactions.UpdateDate = DateTime.Now;
            db.SaveChanges();
            //TempData["success"] = "Talep red edildi.";
            //return RedirectToAction("CallBackApi", "AccountTransactions", new { transid = id });
            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                return RedirectToAction("CallBackApi", "AccountTransactions", new { transid = id });

            }
            else if (Request.Url.Host == "nano.qfinans.com")
            {
                return RedirectToAction("CallBackApiHash", "AccountTransactions", new { transid = id });
                //TempData["warning"] = "Callback api çalışmadı.";
                //return RedirectToAction("Draw");
            }
            else if (Request.Url.Host == "www.pppropanel.com" || Request.Url.Host == "pppropanel.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else if (Request.Url.Host == "www.iprimepay.com" || Request.Url.Host == "iprimepay.com")
            {
                return RedirectToAction("CallBackApiHashForPppropanel", "AccountTransactions", new { transid = id });
            }
            else
            {
                TempData["warning"] = "Callback api çalışmadı.";
                return RedirectToAction("Draw");
            }
        }

        //// GET: Islemler/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    HesapIslemleri hesapIslemleri = db.HesapIslemleri.Find(id);
        //    if (hesapIslemleri == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(hesapIslemleri);
        //}

        //// POST: Islemler/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    HesapIslemleri hesapIslemleri = db.HesapIslemleri.Find(id);
        //    db.HesapIslemleri.Remove(hesapIslemleri);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [CustomAuth(Roles = "CallBackApiAccountTransactions")]
        public ActionResult CallBackApi(int? transid)
        {
            AccountTransactions accountTransactions = db.AccountTransactions.Find(transid);
            var _callbackUrl = db.CallbackUrl.Where(x => x.BrandId == accountTransactions.BrandId).FirstOrDefault();
            string _url;
            if (accountTransactions.IsCoin == true)
            {
                //_url = "https://www.app-dinamo.com/api/coinbase/callback";
                _url = _callbackUrl.Coinbase;
            } else if (accountTransactions.IsMoneyTransfer == true)
            {
                if (accountTransactions.Deposit == true)
                {
                    TempData["warning"] = "Callback api çalışmadı.";
                    return RedirectToAction("Deposit");
                }
                else
                {
                    TempData["warning"] = "Callback api çalışmadı.";
                    return RedirectToAction("Draw");
                }
            } else
            {
                //_url = "https://www.app-dinamo.com/api/paparapro/callback";
                if (accountTransactions.Deposit == false)
                {
                    _url = _callbackUrl.PaparaDraw;
                }
                else
                {
                    _url = _callbackUrl.PaparaDeposit;
                }
            }

            string data = GetCallBackApiData(transid);
            WebRequest request = WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                dynamic result = JObject.Parse(responseFromServer);
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = result.type,
                    message = result.status + " | " + result.message
                };
                
                accountTransactions.ResponseDate = DateTime.Now;
                accountTransactions.ResponseType = jsonObject.type;
                accountTransactions.ResponseMessage = jsonObject.message;
                db.SaveChanges();

                response.Close();

                if (accountTransactions.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                }
                else
                {
                    return RedirectToAction("Draw");
                }
            }
        }

        private string GetCallBackApiData(int? id)
        {
            AccountTransactions at = db.AccountTransactions.Find(id);
            if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    //"&middlename=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=success";
                return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    //"&middlename=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=reject";
                return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    //"&middlename=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=success";
                return transaction;
            }

            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    //"&middlename=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=reject";
                return transaction;
            } else
            {
                string transaction = "";
                return transaction;
            }
        }

        [CustomAuth(Roles = "CallBackApiAccountTransactions")]
        public ActionResult CallBackApiHash(int? transid)
        {
            AccountTransactions accountTransactions = db.AccountTransactions.Find(transid);
            try
            {
                var _callbackUrl = db.CallbackUrl.Where(x => x.BrandId == accountTransactions.BrandId).FirstOrDefault();
                string _url;
                if (accountTransactions.IsMoneyTransfer == true)
                {
                    if (accountTransactions.Deposit == true)
                    {
                        TempData["warning"] = "Callback api çalışmadı.";
                        return RedirectToAction("Deposit");
                    }
                    else
                    {
                        TempData["warning"] = "Callback api çalışmadı.";
                        return RedirectToAction("Draw");
                    }
                }
                else
                {

                    if (accountTransactions.Deposit == false)
                    {
                        //_url = "http://payments1.betconstruct.com/Bets/PaymentsCallback/Pp_Pro_PaparaPG/PayOut.php"
                        _url = _callbackUrl.PaparaDraw;
                    }
                    else
                    {
                        //_url = "http://payments1.betconstruct.com/Bets/PaymentsCallback/Pp_Pro_PaparaPG/PayResult.php";
                        _url = _callbackUrl.PaparaDeposit;
                    }
                }
                string data = GetCallBackApiHashData(transid);
                WebRequest request = WebRequest.Create(_url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = data;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                using (dataStream = response.GetResponseStream())
                {
                    try
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        //dynamic result = JObject.Parse(responseFromServer);
                        //JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        //{
                        //    type = result.type,
                        //    message = result.status + " | " + result.message
                        //};

                        accountTransactions.ResponseDate = DateTime.Now;
                        accountTransactions.ResponseType = "json";
                        accountTransactions.ResponseMessage = responseFromServer;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = ex.Message
                        };

                        accountTransactions.ResponseDate = DateTime.Now;
                        accountTransactions.ResponseType = jsonObject.type;
                        accountTransactions.ResponseMessage = jsonObject.message;
                        db.SaveChanges();
                    }

                    response.Close();

                    if (accountTransactions.Deposit == true)
                    {
                        return RedirectToAction("Deposit");
                    }
                    else
                    {
                        return RedirectToAction("Draw");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                if (accountTransactions.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                }
                else
                {
                    return RedirectToAction("Draw");
                }
            }
        }

        private string GetCallBackApiHashData(int? id)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            AccountTransactions at = db.AccountTransactions.Find(id);
            if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == true)
            {
                string transaction =
                    "orderId=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=success";

                string salt = "orderId,username,name,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                { 
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == true)
            {
                string transaction =
                    "orderId=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=reject";

                string salt = "orderId,username,name,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == false)
            {
                string transaction =
                    "reference=" + at.Reference +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=success";

                string salt = "reference,username,name,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }

            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == false)
            {
                string transaction =
                    "reference=" + at.Reference +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=reject";

                string salt = "reference,username,name,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else
            {
                string transaction = "";
                return transaction;
            }
        }

        [CustomAuth(Roles = "CallBackApiAccountTransactions")]
        public ActionResult CallBackApiHashForPppropanel(int? transid)
        {
            AccountTransactions accountTransactions = db.AccountTransactions.Find(transid);
            try
            {
                var _callbackUrl = db.CallbackUrl.Where(x => x.BrandId == accountTransactions.BrandId).FirstOrDefault();

                string _url;
                if (accountTransactions.IsMoneyTransfer == true)
                {
                    if (accountTransactions.Deposit == true)
                    {
                        TempData["warning"] = "Callback api çalışmadı.";
                        return RedirectToAction("Deposit");
                    }
                    else
                    {
                        TempData["warning"] = "Callback api çalışmadı.";
                        return RedirectToAction("Draw");
                    }
                }
                else
                {

                    if (accountTransactions.Deposit == false)
                    {
                        //_url = "http://payments1.betconstruct.com/Bets/PaymentsCallback/Pp_Pro_PaparaPG/PayOut.php"
                        _url = _callbackUrl.PaparaDraw;
                    }
                    else
                    {
                        //_url = "http://payments1.betconstruct.com/Bets/PaymentsCallback/Pp_Pro_PaparaPG/PayResult.php";
                        _url = _callbackUrl.PaparaDeposit;
                    }
                }
                string data = GetCallBackApiHashDataForPppropanel(transid);
                WebRequest request = WebRequest.Create(_url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = data;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                using (dataStream = response.GetResponseStream())
                {
                    try
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        //dynamic result = JObject.Parse(responseFromServer);
                        //JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        //{
                        //    type = result.type,
                        //    message = result.status + " | " + result.message
                        //};

                        accountTransactions.ResponseDate = DateTime.Now;
                        accountTransactions.ResponseType = "json";
                        accountTransactions.ResponseMessage = responseFromServer;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = ex.Message
                        };

                        accountTransactions.ResponseDate = DateTime.Now;
                        accountTransactions.ResponseType = jsonObject.type;
                        accountTransactions.ResponseMessage = jsonObject.message;
                        db.SaveChanges();
                    }

                    response.Close();

                    if (accountTransactions.Deposit == true)
                    {
                        return RedirectToAction("Deposit");
                    }
                    else
                    {
                        return RedirectToAction("Draw");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                if (accountTransactions.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                }
                else
                {
                    return RedirectToAction("Draw");
                }
            }
        }

        private string GetCallBackApiHashDataForPppropanel(int? id)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            AccountTransactions at = db.AccountTransactions.Find(id);
            if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&middleName=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=success";

                string salt = "id,username,name,middleName,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&middleName=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=reject";

                string salt = "id,username,name,middleName,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&middleName=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=success";

                string salt = "id,username,name,middleName,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }

            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&middleName=" + at.MiddleName +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
                    "&status=reject";

                string salt = "id,username,name,middleName,surname,type,message,amount,status";

                string data = transaction + salt;

                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(data);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    string query = transaction + "&hash=" + hash.ToLower();
                    return query;
                }

                //return transaction;
            }
            else
            {
                string transaction = "";
                return transaction;
            }
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
