using Microsoft.AspNet.Identity;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Hubs;
using QFinans.Models;
using QFinans.Repostroies;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string _userId = User.Identity.GetUserId();
            var _user = db.Users.Find(_userId);
            if (_user.PaparaDashboard == false && _user.HavaleEFtDashboard == false)
            {
                return RedirectToAction("UnAuthorized", "CustomAuth");
            }
            else if (_user.HavaleEFtDashboard == true && _user.PaparaDashboard == false)
            {
                return RedirectToAction("HavaleEft");
            }

            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
           
            var account = db.AccountInfo.Where(x => x.IsDeleted == false);
            var accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
            var deposit = db.AccountTransactions.Where(x => x.Deposit == true && x.IsCoin == false && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
            var draw = db.AccountTransactions.Where(x => x.Deposit == false && x.IsCoin == false && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
                deposit = db.AccountTransactions.Where(x => x.Deposit == true && x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
                draw = db.AccountTransactions.Where(x => x.Deposit == false && x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
            }

            ViewBag.DepositCount = deposit.Count();
            ViewBag.NewDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.DrawCount = draw.Count();
            ViewBag.NewDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.ActiveAccountCount = account.Where(x => x.IsPassive == false).Count();
            ViewBag.ConfirmAccountTransactionCount = accountTransactions.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();

            ViewBag.CashInSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.CashOutSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.DepositFreeSum = deposit.Where(x => x.IsFree == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.CashInFreeSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.IsFree == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            var activeAccountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.IsPassive == false).OrderByDescending(x => x.Id).ToList();

            ViewBag.ActiveAccountInfo = activeAccountInfo;

            var activeBankInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.IsPassive == false).OrderByDescending(x => x.Id).ToList();

            ViewBag.ActiveBankInfo = activeBankInfo;

            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();
            
            return View(accountTransactions);
        }

        public ActionResult HavaleEft()
        {
            string _userId = User.Identity.GetUserId();
            var _user = db.Users.Find(_userId);
            if (_user.HavaleEFtDashboard == false)
            {
                return RedirectToAction("UnAuthorized", "CustomAuth");
            }

            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;

            var bankInfo = db.BankInfo.Where(x => x.IsDeleted == false);
            var moneyTransfer = db.MoneyTransfer.Where(x => x.AddDate >= startDate && x.AddDate < endDate);
            var deposit = db.MoneyTransfer.Where(x => x.Deposit == true && x.AddDate >= startDate && x.AddDate < endDate);
            var draw = db.MoneyTransfer.Where(x => x.Deposit == false && x.AddDate >= startDate && x.AddDate < endDate);

            ViewBag.DepositCount = deposit.Count();
            ViewBag.NewDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.DrawCount = draw.Count();
            ViewBag.NewDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.ActiveAccountCount = bankInfo.Where(x => x.IsPassive == false).Count();
            ViewBag.ConfirmAccountTransactionCount = moneyTransfer.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();

            ViewBag.CashInSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.CashOutSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            var activeAccountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.IsPassive == false).OrderByDescending(x => x.Id).ToList();

            ViewBag.ActiveAccountInfo = activeAccountInfo;

            var activeBankInfo = db.BankInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.IsPassive == false).OrderByDescending(x => x.Id).ToList();

            ViewBag.ActiveBankInfo = activeBankInfo;

            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();

            return View(moneyTransfer);
        }

        [HttpGet]
        public JsonResult GetDashboardJsonData()
        {
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
            var account = db.AccountInfo.Where(x => x.IsDeleted == false);
            var deposit = db.AccountTransactions.Where(x => x.Deposit == true && x.IsCoin == false && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
            var draw = db.AccountTransactions.Where(x => x.Deposit == false && x.IsCoin == false && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                deposit = db.AccountTransactions.Where(x => x.Deposit == true && x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
                draw = db.AccountTransactions.Where(x => x.Deposit == false && x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.AddDate >= startDate && x.AddDate < endDate);
            }

            ViewBag.DepositCount = deposit.Count();
            ViewBag.NewDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDepositCount = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDepositSum = deposit.Where(x => x.TransactionStatus == TransactionStatus.Deny).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.DrawCount = draw.Count();
            ViewBag.NewDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.ConfirmDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Count();
            ViewBag.DenyDrawCount = draw.Where(x => x.TransactionStatus == TransactionStatus.Deny).Count();
            ViewBag.NewDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.ConfirmDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.DenyDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.CashInSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.CashOutSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.DepositFreeSum = deposit.Where(x => x.IsFree == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.CashInFreeSum = db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.IsFree == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.ActivePaparaCount = account.Where(x => x.IsDeleted == false && x.IsPassive == false && x.IsArchive == false).OrderByDescending(x => x.OrderNumber).ThenByDescending(x => x.Id).Count();

            ViewBag.ActiveBankCount = db.BankInfo.Where(x => x.IsDeleted == false && x.IsPassive == false && x.IsArchive == false).OrderByDescending(x => x.OrderNumber).ThenByDescending(x => x.Id).Count();

            ViewBag.ActiveTotalAccountCount = ViewBag.ActivePaparaCount + ViewBag.ActiveBankCount;

            var bankDeposit = db.MoneyTransfer.Where(x => x.Deposit == true && x.AddDate >= startDate && x.AddDate < endDate);
            var bankDraw = db.MoneyTransfer.Where(x => x.Deposit == false && x.AddDate >= startDate && x.AddDate < endDate);

            ViewBag.BankNewDepositCount = bankDeposit.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.BankNewDepositSum = bankDeposit.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.BankConfirmDepositSum = bankDeposit.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            ViewBag.BankNewDrawCount = bankDraw.Where(x => x.TransactionStatus == TransactionStatus.New).Count();
            ViewBag.BankNewDrawSum = draw.Where(x => x.TransactionStatus == TransactionStatus.New).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            ViewBag.BankConfirmDrawSum = bankDraw.Where(x => x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

            DashboardJsonViewModel dashboardJsonViewModel = new DashboardJsonViewModel
            {
                DepositCount = ViewBag.DepositCount,
                NewDepositCount = ViewBag.NewDepositCount,
                ConfirmDepositCount = ViewBag.ConfirmDepositCount,
                DenyDepositCount = ViewBag.DenyDepositCount,
                NewDepositSum = ViewBag.NewDepositSum,
                ConfirmDepositSum = ViewBag.ConfirmDepositSum,
                DenyDepositSum = ViewBag.DenyDepositSum,
                DrawCount = ViewBag.DrawCount,
                NewDrawCount = ViewBag.NewDrawCount,
                ConfirmDrawCount = ViewBag.ConfirmDrawCount,
                DenyDrawCount = ViewBag.DenyDrawCount,
                NewDrawSum = ViewBag.NewDrawSum,
                ConfirmDrawSum = ViewBag.ConfirmDrawSum,
                DenyDrawSum = ViewBag.DenyDrawSum,
                ActivePaparaCount = ViewBag.ActivePaparaCount,
                ActiveBankCount = ViewBag.ActiveBankCount,
                ActiveTotalAccountCount = ViewBag.ActiveTotalAccountCount,
                CurrentDate = startDate.ToShortDateString(),
                CashInSum = ViewBag.CashInSum,
                CashOutSum = ViewBag.CashOutSum,
                DepositFreeSum = ViewBag.DepositFreeSum,
                CashInFreeSum = ViewBag.CashInFreeSum,
                TotalFreeSum = ViewBag.DepositFreeSum + ViewBag.CashInFreeSum,
                BankNewDepositCount = ViewBag.BankNewDepositCount,
                BankNewDepositSum = ViewBag.BankNewDepositSum,
                BankConfirmDepositSum = ViewBag.BankConfirmDepositSum,
                BankNewDrawCount = ViewBag.BankNewDrawCount,
                BankNewDrawSum = ViewBag.BankNewDrawSum,
                BankConfirmDrawSum = ViewBag.BankConfirmDrawSum
            };

            return Json(dashboardJsonViewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBalanceJsonData()
        {
            IQueryable<AccountInfo> accountInfo = db.AccountInfo.Where(a => a.IsDeleted == false && a.IsArchive == false);
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.IsMoneyTransfer == false && x.TransactionStatus == TransactionStatus.Confirm);

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.TransactionStatus == TransactionStatus.Confirm);
            }

            BalanceViewModel balanceViewModel = new BalanceViewModel
            {
                ConfirmDepositSum = accountTransactions.Where(x => x.Deposit == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum(),
                ConfirmDrawSum = accountTransactions.Where(x => x.Deposit == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum(),

                InitialBalance = db.SystemParameters.Select(x => x.InitialBalance).DefaultIfEmpty(0).FirstOrDefault(),
                BalanceEditAmount = db.SystemParameters.Select(x => x.BalanceEditAmount).DefaultIfEmpty(0).FirstOrDefault(),
                BankCharge = db.SystemParameters.Select(x => x.BankCharge).DefaultIfEmpty(0).FirstOrDefault(),

                Safe = accountInfo.Select(x => x.Balance).DefaultIfEmpty(0).Sum() ?? 0
                //Safe = accountTransactions.Where(x => x.Deposit == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum()
                //       + accountTransactions.Where(x => x.Deposit == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum()
            };
            return Json(balanceViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult NotificationShowData()
        {
            var accountTransactions = db.AccountTransactions.Where(x => x.TransactionStatus == TransactionStatus.New && x.NotificationDate == null).OrderBy(x => x.Id);
            if (accountTransactions.Count() > 0)
            {
                var data = accountTransactions.Select(x => new
                {
                    id = x.Id,
                    username = x.UserName,
                    deposit = x.Deposit,
                    amount = x.Amount,
                    date = x.AddDate.ToString()
                }).First();
                return Json(data, JsonRequestBehavior.AllowGet);
            } else
            {
                IDictionary<string, int> data = new Dictionary<string, int>()
                                            {
                                                {"id", 0}
                                            };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NotificationHideData(int? id)
        {
            if (id == null)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = "Id: " + id.ToString() + " is not found."
                };
                return Json(jsonObject);
            }
            AccountTransactions accountTransactions = db.AccountTransactions.Find(id);
            if (accountTransactions == null)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = "Id: " + id.ToString() + " is not found."
                };
                return Json(jsonObject);
            }

            accountTransactions.NotificationDate = DateTime.Now;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotificationHideAllData()
        {
            var accountTransactions = db.AccountTransactions.Where(x => x.NotificationDate == null).ToList();
            accountTransactions.ForEach(y => y.NotificationDate = DateTime.Now);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult NotificationShowDataForMoneyTransfer()
        {
            var moneyTransfer = db.MoneyTransfer.Where(x => x.TransactionStatus == TransactionStatus.New && x.NotificationDate == null).OrderBy(x => x.Id);
            if (moneyTransfer.Count() > 0)
            {
                var data = moneyTransfer.Select(x => new
                {
                    id = x.Id,
                    username = x.UserName,
                    deposit = x.Deposit,
                    amount = x.Amount,
                    date = x.AddDate.ToString()
                }).First();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IDictionary<string, int> data = new Dictionary<string, int>()
                                            {
                                                {"id", 0}
                                            };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NotificationHideDataForMoneyTransfer(int? id)
        {
            if (id == null)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = "Id: " + id.ToString() + " is not found."
                };
                return Json(jsonObject);
            }
            MoneyTransfer moneyTransfer = db.MoneyTransfer.Find(id);
            if (moneyTransfer == null)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = "Id: " + id.ToString() + " is not found."
                };
                return Json(jsonObject);
            }

            moneyTransfer.NotificationDate = DateTime.Now;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotificationHideAllDataForMoneyTransfer()
        {
            var moneyTransfer = db.MoneyTransfer.Where(x => x.NotificationDate == null).ToList();
            moneyTransfer.ForEach(y => y.NotificationDate = DateTime.Now);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotificationSound()
        {
            string _userId = User.Identity.GetUserId();
            var _user = db.Users.Find(_userId);
            IDictionary<string, bool> data = new Dictionary<string, bool>()
                                            {
                                                {"soundOff", _user.SoundOff}
                                            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotificationSoundChange()
        {
            string _userId = User.Identity.GetUserId();
            ApplicationUser _user = db.Users.Find(_userId);
            if(_user.SoundOff == true)
            {
                _user.SoundOff = false;
                db.SaveChanges();
                IDictionary<string, int> data = new Dictionary<string, int>()
                                            {
                                                {"soundOff", 0}
                                            };
                return Json(data, JsonRequestBehavior.AllowGet);
            } else
            {
                _user.SoundOff = true;
                db.SaveChanges();
                IDictionary<string, int> data = new Dictionary<string, int>()
                                            {
                                                {"soundOff", 1}
                                            };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return RedirectToAction("Index", "Home");
            //return View();
        }

        public ActionResult SetAccountAsPassive()
        {
            DateTime date = DateTime.Now.Date;
            DateTime startDate = new DateTime(date.Year, date.Month, 1); ;
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);
            var activeAccountInfo = (from a in db.AccountInfo
                                     where a.IsDeleted == false && a.IsPassive == false
                                     select new
                                     {
                                         a.Id,
                                         a.Name,
                                         a.SurName,
                                         a.AccountNumber,
                                         a.IsPassive,
                                         AccountInfoTypeName = a.AccountInfoType.Name,
                                         AccountInfoTypeTransactionLimit = a.AccountInfoType.TransactionLimit,
                                         AccountInfoTypeAmountLimit = a.AccountInfoType.AmountLimit,
                                         a.OrderNumber,

                                         ConfirmDepositCount = db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == a.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Count(),

                                         ConfirmDepositSum = db.AccountTransactions.Where(t => t.Deposit == true && t.AccountInfoId == a.Id && t.AddDate >= startDate && t.AddDate <= endDate && t.TransactionStatus == TransactionStatus.Confirm).Select(s => s.Amount).DefaultIfEmpty(0).Sum(),

                                         LastTransactionTime = db.AccountTransactions.Where(t => t.Deposit == true && t.TransactionStatus == TransactionStatus.Confirm).OrderByDescending(t => t.AddDate).FirstOrDefault().AddDate

                                     }).OrderByDescending(x => x.OrderNumber).ThenByDescending(x => x.Id);

            var gettingPassiveAccount = activeAccountInfo.Where(x => x.ConfirmDepositCount >= x.AccountInfoTypeTransactionLimit || x.ConfirmDepositSum >= x.AccountInfoTypeAmountLimit);

            var newContext = new ApplicationDbContext();
            foreach (var item in gettingPassiveAccount)
            {
                var acc = newContext.AccountInfo.Find(item.Id);
                if (item.LastTransactionTime.AddHours(1) < DateTime.Now)
                {
                    acc.IsPassive = true;
                }
                
            }
            newContext.SaveChanges();
            newContext.Dispose();

            return Json(gettingPassiveAccount, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNotificationAccountTransactions()
        {
            using(var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NotificationConnection"].ConnectionString))
            {
                connection.Open();
                //"SELECT [Id], [UserName], [Name], [SurName], [Amount], [Deposit] FROM [dbo].[AccountTransactions] WHERE [TransactionStatus]=1"
                using (SqlCommand command = new SqlCommand(@"SELECT [Id], [UserName], [Name], [SurName], [Amount], [Deposit] FROM [dbo].[AccountTransactions] WHERE [NotificationDate] IS NULL", connection))
                {
                    command.Notification = null;
                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependencyNotify_OnChange);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    var listData = reader.Cast<IDataRecord>()
                           .Select(x => new
                           {
                               id = x["Id"],
                               username = x["UserName"],
                               name = x["Name"],
                               surname = x["SurName"],
                               amount = x["Amount"],
                               deposit = x["Deposit"],
                           }).ToList();
                    //connection.Close();
                    return Json(new { listData }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void dependencyNotify_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var dependency = (SqlDependency)sender;
            dependency.OnChange -= new OnChangeEventHandler(dependencyNotify_OnChange);
            if (e.Type == SqlNotificationType.Change)
            {
                NotificationHub.Show();
            }
        }

        [AllowAnonymous]
        public ActionResult GetQR(int? id)
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

            CustomFunctions _qr = new CustomFunctions();
            var _qrImgSrc = _qr.GenerateQR("https://www.papara.com/personal/qr?accountNumber=" + accountTransactions.AccountInfo.AccountNumber + "&currency=0&amount=" + Math.Round(accountTransactions.Amount));
            ViewBag.Qr = _qrImgSrc;

            return View();
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