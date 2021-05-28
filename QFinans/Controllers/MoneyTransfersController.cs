using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class MoneyTransfersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MoneyTransfers
        [CustomAuth(Roles = "DepositMoneyTransfers")]
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
            int? bankInfoId,
            int? currentBankInfoId
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
            IQueryable<MoneyTransfer> moneyTransfers = db.MoneyTransfer.Where(x => x.Deposit == true).Include(x => x.BankInfo);

            if (transactionStatus != null) { page = 1; } else { transactionStatus = currentTransactionStatus; }
            ViewBag.CurrentTransactionStatusId = transactionStatus;
            if (transactionStatus.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.TransactionStatus == transactionStatus);

            if (dateFrom != null) { page = 1; } else { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.AddDate >= dateFrom);

            if (dateTo != null) { page = 1; } else { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.AddDate <= dateTo);

            if (bankInfoId != null) { page = 1; } else { bankInfoId = currentBankInfoId; }
            ViewBag.CurrentAccountInfoId = bankInfoId;
            if (bankInfoId.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.BankInfoId == bankInfoId);

            if (!String.IsNullOrEmpty(searchString))
            {
                moneyTransfers = moneyTransfers.Where(x => x.UserName.Contains(searchString)
                                                        || x.Name.Contains(searchString)
                                                        || x.SurName.Contains(searchString)
                                                        || x.Id.ToString() == searchString
                                                        || (x.Name + " " + x.SurName).Contains(searchString)
                                                        || x.BankInfo.AccountNumber.ToString().Contains(searchString));

                if (moneyTransfers.Any() == false)
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
            return View(moneyTransfers.OrderBy(x => x.TransactionStatus).ThenByDescending(x => x.AddDate).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DrawMoneyTransfers")]
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
            int? bankInfoId,
            int? currentBankInfoId
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
            IQueryable<MoneyTransfer> moneyTransfers = db.MoneyTransfer.Where(x => x.Deposit == false).Include(x => x.BankInfo);

            if (transactionStatus != null) { page = 1; } else { transactionStatus = currentTransactionStatus; }
            ViewBag.CurrentTransactionStatusId = transactionStatus;
            if (transactionStatus.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.TransactionStatus == transactionStatus);

            if (dateFrom != null) { page = 1; } else { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.AddDate >= dateFrom);

            if (dateTo != null) { page = 1; } else { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.AddDate <= dateTo);

            if (bankInfoId != null) { page = 1; } else { bankInfoId = currentBankInfoId; }
            ViewBag.CurrentAccountInfoId = bankInfoId;
            if (bankInfoId.HasValue)
                moneyTransfers = moneyTransfers.Where(x => x.BankInfoId == bankInfoId);

            if (!String.IsNullOrEmpty(searchString))
            {
                moneyTransfers = moneyTransfers.Where(x => x.UserName.Contains(searchString)
                                                        || x.Name.Contains(searchString)
                                                        || x.SurName.Contains(searchString)
                                                        || x.Id.ToString() == searchString
                                                        || (x.Name + " " + x.SurName).Contains(searchString)
                                                        || x.BankInfo.AccountNumber.ToString().Contains(searchString));

                if (moneyTransfers.Any() == false)
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
            //var accountTransactions = db.AccountTransactions.Where(x => x.Deposit == true).Include(h => h.AccountInfo).OrderByDescending(x => x.AddDate);
            return View(moneyTransfers.OrderBy(x => x.TransactionStatus).ThenByDescending(x => x.AddDate).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "DetailsMoneyTransfers")]
        // GET: Islemler/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoneyTransfer moneyTransfers = db.MoneyTransfer.Find(id);
            if (moneyTransfers == null)
            {
                return HttpNotFound();
            }
            return View(moneyTransfers);
        }

        [CustomAuth(Roles = "CreateDepositMoneyTransfers")]
        public ActionResult CreateDeposit()
        {
            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.BankInfoId = new SelectList(bankInfo, "Value", "Text");

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text");

            return View();
        }

        // POST: Islemler/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateDepositMoneyTransfers")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeposit(MoneyTransfer moneyTransfer)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var account = db.BankInfo.Find(moneyTransfer.BankInfoId);
                moneyTransfer.Deposit = true;
                moneyTransfer.TransactionStatus = TransactionStatus.New;
                moneyTransfer.OldAmount = moneyTransfer.Amount;
                moneyTransfer.Location = "web_panel";
                moneyTransfer.AddUserId = _userId;
                moneyTransfer.AddDate = DateTime.Now;
                moneyTransfer.BankInfoId = moneyTransfer.BankInfoId;
                moneyTransfer.CustomerBankInfoId = moneyTransfer.CustomerBankInfoId;

                db.MoneyTransfer.Add(moneyTransfer);
                db.SaveChanges();
                TempData["success"] = "Talep oluşturuldu.";
                return RedirectToAction("Deposit");
            }

            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.AccountInfoId = new SelectList(bankInfo, "Value", "Text", moneyTransfer.BankInfoId);

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text", moneyTransfer.CustomerBankInfoId);

            return View(moneyTransfer);
        }

        [CustomAuth(Roles = "CreateDrawMoneyTransfers")]
        public ActionResult CreateDraw()
        {
            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.BankInfoId = new SelectList(bankInfo, "Value", "Text");

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text");

            return View();
        }

        // POST: Islemler/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateDrawMoneyTransfers")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDraw(MoneyTransfer moneyTransfer)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                moneyTransfer.Deposit = false;
                moneyTransfer.TransactionStatus = TransactionStatus.New;
                moneyTransfer.OldAmount = moneyTransfer.Amount;
                moneyTransfer.Location = "web_panel";
                moneyTransfer.AddUserId = _userId;
                moneyTransfer.AddDate = DateTime.Now;
                moneyTransfer.BankInfoId = moneyTransfer.BankInfoId;
                moneyTransfer.CustomerBankInfoId = moneyTransfer.CustomerBankInfoId;
                moneyTransfer.CustomerIban = moneyTransfer.CustomerIban;
                db.MoneyTransfer.Add(moneyTransfer);
                db.SaveChangesAsync();
                TempData["success"] = "Talep oluşturuldu.";
                return RedirectToAction("Draw");
            }

            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.BankInfoId = new SelectList(bankInfo, "Value", "Text", moneyTransfer.BankInfoId);

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text", moneyTransfer.CustomerBankInfoId);

            return View(moneyTransfer);
        }


        [CustomAuth(Roles = "EditMoneyTransfers")]
        // GET: MoneyTransfers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(id);
            if (moneyTransfer == null)
            {
                return HttpNotFound();
            }
            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.BankInfoId = new SelectList(bankInfo, "Value", "Text", moneyTransfer.BankInfoId);

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text", moneyTransfer.CustomerBankInfoId);

            return View(moneyTransfer);
        }

        [HttpPost]
        [CustomAuth(Roles = "EditMoneyTransfers")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MoneyTransfer moneyTransfer)
        {
            string _userId = User.Identity.GetUserId();
            MoneyTransfer _moneyTransfer = db.MoneyTransfer.Find(moneyTransfer.Id);
            if (_moneyTransfer == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                _moneyTransfer.Amount = moneyTransfer.Amount;
                _moneyTransfer.BankInfoId = moneyTransfer.BankInfoId;
                _moneyTransfer.CustomerBankInfoId = moneyTransfer.CustomerBankInfoId;
                _moneyTransfer.CustomerIban = moneyTransfer.CustomerIban;
                _moneyTransfer.Note = moneyTransfer.Note;
                _moneyTransfer.UpdateUserId = _userId;
                _moneyTransfer.UpdateDate = DateTime.Now;
                db.SaveChangesAsync();
                TempData["success"] = "Kayıt düzenledi.";

                if (_moneyTransfer.Deposit == true)
                {
                    return RedirectToAction("Deposit");
                }
                else
                {
                    return RedirectToAction("Draw");
                }
            }
            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.Name + " " + x.Surname + " (" + x.BankType.Name + " - " + x.Iban + ")"
            }).ToList();
            ViewBag.BankInfoId = new SelectList(bankInfo, "Value", "Text", moneyTransfer.BankInfoId);

            var customerBankInfoId = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList().Select(x => new SelectListItem
            {
                Selected = false,
                Value = x.Id.ToString(),
                Text = x.BankType.Name
            }).ToList();
            ViewBag.CustomerBankInfoId = new SelectList(customerBankInfoId, "Value", "Text", moneyTransfer.CustomerBankInfoId);

            return View(moneyTransfer);
        }

        [CustomAuth(Roles = "ConfirmDepositMoneyTransfers")]
        public ActionResult ConfirmDeposit(int? id)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(id);
            if (moneyTransfer == null)
            {
                return HttpNotFound();
            }

            moneyTransfer.TransactionStatus = TransactionStatus.Confirm;
            moneyTransfer.UpdateUserId = _userId;
            moneyTransfer.UpdateDate = DateTime.Now;
            db.SaveChangesAsync();
            TempData["success"] = "Talep onaylandı.";
            return RedirectToAction("CallBackApi", "MoneyTransfers", new { transid = id });
        }

        [CustomAuth(Roles = "ConfirmDrawMoneyTransfers")]
        public ActionResult ConfirmDraw(int? id, int bankInfoId)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(id);
            if (moneyTransfer == null)
            {
                return HttpNotFound();
            }

            moneyTransfer.BankInfoId = bankInfoId;
            moneyTransfer.TransactionStatus = TransactionStatus.Confirm;
            moneyTransfer.UpdateUserId = _userId;
            moneyTransfer.UpdateDate = DateTime.Now;
            db.SaveChangesAsync();
            TempData["success"] = "Talep onayladı.";
            return RedirectToAction("CallBackApi", "MoneyTransfers", new { transid = id });
        }

        [CustomAuth(Roles = "DenyMoneyTransfers")]
        public ActionResult Deny(int? id, string aciklama)
        {
            string _userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(id);
            if (moneyTransfer == null)
            {
                return HttpNotFound();
            }

            moneyTransfer.Note = aciklama;
            moneyTransfer.TransactionStatus = TransactionStatus.Deny;
            moneyTransfer.UpdateUserId = _userId;
            moneyTransfer.UpdateDate = DateTime.Now;
            db.SaveChangesAsync();
            TempData["success"] = "Talep red edildi.";
            return RedirectToAction("CallBackApi", "MoneyTransfers", new { transid = id });
        }

        [CustomAuth(Roles = "CallBackApiMoneyTransfers")]
        public ActionResult CallBackApi(int? transid)
        {
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(transid);

            try
            {
                var _callbackUrl = db.CallbackUrl.FirstOrDefault();
                string _url;
                if (moneyTransfer.Deposit == false)
                {
                    _url = _callbackUrl.MoneyTransferDraw;
                }
                else
                {
                    _url = _callbackUrl.MoneyTransferDeposit;
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

                        moneyTransfer.ResponseDate = DateTime.Now;
                        moneyTransfer.ResponseType = "json";
                        moneyTransfer.ResponseMessage = responseFromServer;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = ex.Message
                        };

                        moneyTransfer.ResponseDate = DateTime.Now;
                        moneyTransfer.ResponseType = jsonObject.type;
                        moneyTransfer.ResponseMessage = jsonObject.message;
                        db.SaveChanges();
                    }

                    response.Close();

                    if (moneyTransfer.Deposit == true)
                    {
                        return RedirectToAction("Deposit", "MoneyTransfers");
                    }
                    else
                    {
                        return RedirectToAction("Draw", "MoneyTransfers");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                if (moneyTransfer.Deposit == true)
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
            MoneyTransfer at = db.MoneyTransfer.Find(id);
            if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=success" +
                    "&reference=" + at.Reference;
                return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == true)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=rejct" +
                    "&reference=" + at.Reference;
                return transaction;
            }
            else if (at.TransactionStatus == TransactionStatus.Confirm && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=success" +
                    "&reference=" + at.Reference;
                return transaction;
            }

            else if (at.TransactionStatus == TransactionStatus.Deny && at.Deposit == false)
            {
                string transaction =
                    "id=" + at.Id +
                    "&username=" + at.UserName +
                    "&name=" + at.Name +
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=reject" +
                    "&reference=" + at.Reference;
                return transaction;
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