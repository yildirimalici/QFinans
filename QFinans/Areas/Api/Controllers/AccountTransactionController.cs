using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using QFinans.Repostroies;

namespace QFinans.Areas.Api.Controllers
{
    //[Authorize]
    [LogJson]
    public class AccountTransactionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Api/HesapIslemleri
        [HttpPost]
        public JsonResult Deposit(DepositViewModel depositViewModel)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                //string clientIp = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                //       Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject);
                }

                if (_user.UserName == userName && _user.Password == password)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    var userDepositCount = db.AccountTransactions.Where(x => x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.UserName == depositViewModel.UserName).Count();
                    if (userDepositCount > 0)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "pending transactions try later"
                        };
                        return Json(jsonObject);
                    } else
                    {
                        DateTime date = DateTime.Now.Date;
                        DateTime startDate = new DateTime(date.Year, date.Month, 1);
                        DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);

                        var freeTransactionNumber = db.SystemParameters.Select(x => (x.FreeTransactionNumber == null ? 0 : x.FreeTransactionNumber)).FirstOrDefault() ?? 0;
                        var initialAccountLimit = db.SystemParameters.Select(x => (x.InitialAmountLimit == null ? 0 : x.InitialAmountLimit)).FirstOrDefault();
                        var emergencyAccountId = db.SystemParameters.Select(x => (x.EmergencyAccountId == null ? 0 : x.EmergencyAccountId)).FirstOrDefault() ?? 0;
                        var freeFirstDifferentUserNumber = db.SystemParameters.Select(x => (x.FreeFirstDifferentUserNumber == null ? 0 : x.FreeFirstDifferentUserNumber)).FirstOrDefault() ?? 0;

                        var accountInfos = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsPassive == false && x.IsArchive == false

                                                                && x.AccountInfoType.TransactionLimit > (x.AccountTransactions.Where(t => t.Deposit == true && t.AddDate >= startDate && t.AddDate <= endDate && (t.TransactionStatus == TransactionStatus.New || t.TransactionStatus == TransactionStatus.Confirm)).Count()
                                                                + x.CashFlow.Where(a => a.IsDeleted == false && a.IsCashIn == true && a.TransactionDate >= startDate && a.TransactionDate <= endDate && (a.CashFlowType.IsTransactionCount == true || a.CashFlowTypeId == null)).Count())

                                                                && x.AccountInfoType.AmountLimit > (x.AccountTransactions.Where(t => t.Deposit == true && t.AddDate >= startDate && t.AddDate <= endDate && (t.TransactionStatus != TransactionStatus.Deny)).Select(t => t.Amount).DefaultIfEmpty(0).Sum()
                                                                + x.CashFlow.Where(a => a.IsDeleted == false && a.IsCashIn == true && a.TransactionDate >= startDate && a.TransactionDate <= endDate).Select(a => a.Amount).DefaultIfEmpty(0).Sum()
                                                                + depositViewModel.Amount)
                                                            );

                        var redirectAccountInfo = accountInfos.Where(x => x.AccountAmountRedirect.MinAmount <= depositViewModel.Amount && depositViewModel.Amount <= x.AccountAmountRedirect.MaxAmount).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                        var freeAccountInfo = accountInfos.Where(x => x.AccountAmountRedirectId == null && (x.AccountTransactions.Where(t => t.Deposit == true && t.AddDate >= startDate && t.AddDate <= endDate
                                                            && (t.TransactionStatus == TransactionStatus.New || t.TransactionStatus == TransactionStatus.Confirm)).Count()
                                                                + x.CashFlow.Where(a => a.IsDeleted == false && a.IsCashIn == true && a.TransactionDate >= startDate && a.TransactionDate <= endDate && (a.CashFlowType.IsTransactionCount == true || a.CashFlowTypeId == null)).Count()) < freeTransactionNumber

                                                            ).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();


                        var accountInfo = accountInfos.OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();



                        if (accountInfo == null)
                        {
                            JsonObjectViewModel jsonObject = new JsonObjectViewModel
                            {
                                type = "error",
                                message = "there is no available account"
                            };
                            return Json(jsonObject);
                        }

                        AccountTransactions accountTransactions = new AccountTransactions();

                        if (ModelState.IsValid)
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                accountTransactions.UserName = depositViewModel.UserName;
                                accountTransactions.Name = depositViewModel.Name;
                                accountTransactions.MiddleName = depositViewModel.MiddleName;
                                accountTransactions.SurName = depositViewModel.SurName;
                                accountTransactions.Amount = depositViewModel.Amount;
                                accountTransactions.OldAmount = depositViewModel.Amount;
                                accountTransactions.Reference = depositViewModel.Reference;
                                accountTransactions.Deposit = true;
                                accountTransactions.TransactionStatus = TransactionStatus.New;
                                if (redirectAccountInfo != null)
                                {
                                    accountTransactions.AccountInfoId = redirectAccountInfo.Id;
                                }
                                else if (freeAccountInfo != null && initialAccountLimit > 0 && depositViewModel.Amount >= initialAccountLimit)
                                {
                                    accountTransactions.AccountInfoId = freeAccountInfo.Id;
                                    accountTransactions.IsFree = true;
                                }
                                else if (accountInfo != null)
                                {
                                    accountTransactions.AccountInfoId = accountInfo.Id;
                                }
                                else
                                {
                                    accountTransactions.AccountInfoId = emergencyAccountId;
                                }

                                accountTransactions.Location = "api";
                                accountTransactions.AddUserId = _user.UserName;
                                accountTransactions.AddDate = DateTime.Now;

                                //accountInfo.TransactionCount += 1;
                                var _cashIn = db.CashFlow.Where(x => x.IsDeleted == false && x.AccountInfoId == accountTransactions.AccountInfoId && x.IsCashIn == true && x.TransactionDate >= startDate && x.TransactionDate <= endDate).OrderBy(x => x.TransactionDate).Take(freeFirstDifferentUserNumber).Count();
                                var _deposits = db.AccountTransactions.Where(x => x.AccountInfoId == accountTransactions.AccountInfoId && x.Deposit == true && x.AddDate >= startDate && x.AddDate <= endDate && (x.TransactionStatus == TransactionStatus.New || x.TransactionStatus == TransactionStatus.Confirm)).OrderBy(x => x.AddDate);
                                var _users = _deposits.GroupBy(x => x.UserName).Select(g => new { name = g.Key, count = g.Count(), date = g.Select(x => x.AddDate).Min() }).OrderBy(x => x.date).Take(freeFirstDifferentUserNumber - _cashIn);

                                if (_users.Count() == 0)
                                {
                                    accountTransactions.IsFree = true;
                                    accountTransactions.FirstDifferentTransaction = true;
                                }
                                if (_users.Any(x => x.name == accountTransactions.UserName))
                                {
                                    accountTransactions.IsFree = true;
                                    accountTransactions.FirstDifferentTransaction = true;
                                }

                                if ((_deposits.Count() + _cashIn) <= freeTransactionNumber)
                                {
                                    accountTransactions.IsFree = true;
                                }

                                context.AccountTransactions.Add(accountTransactions);
                                context.SaveChanges();
                            }

                        }
                        else
                        {
                            JsonObjectViewModel jsonObject = new JsonObjectViewModel
                            {
                                type = "error",
                                message = "dataset is not valid"
                            };
                            return Json(jsonObject);
                        }


                        string _qr_url = "https://" + Request.Url.Host + "/Home/GetQR/" + accountTransactions.Id.ToString();

                        var data = (from h in db.AccountTransactions
                                    where h.Id == accountTransactions.Id
                                    select new
                                    {
                                        id = h.Id,
                                        userName = h.UserName,
                                        name = h.Name,
                                        middleName = h.MiddleName,
                                        surName = h.SurName,
                                        amount = h.Amount,
                                        accountNumber = h.AccountInfo.AccountNumber,
                                        accountName = h.AccountInfo.Name,
                                        accountSurName = h.AccountInfo.SurName,
                                        reference = h.Reference,
                                        qrUrl = _qr_url
                                    });

                        return Json(data.First());
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject);
                }
            } catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject);
            }
        }

        [HttpPost]
        public JsonResult Draw(DrawViewModel drawViewModel)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                //string clientIp = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                //       Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject);
                }

                if (_user.UserName == userName && _user.Password == password)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    var userDrawCount = db.AccountTransactions.Where(x => x.Deposit == false && x.TransactionStatus == TransactionStatus.New && x.UserName == drawViewModel.UserName).Count();
                    if (userDrawCount == 0)
                    {
                        AccountTransactions accountTransactions = new AccountTransactions();

                        if (ModelState.IsValid)
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                accountTransactions.UserName = drawViewModel.UserName;
                                accountTransactions.Name = drawViewModel.Name;
                                accountTransactions.MiddleName = drawViewModel.MiddleName;
                                accountTransactions.SurName = drawViewModel.SurName;
                                accountTransactions.Amount = drawViewModel.Amount;
                                accountTransactions.OldAmount = drawViewModel.Amount;
                                accountTransactions.Reference = drawViewModel.Reference;
                                accountTransactions.Deposit = false;
                                accountTransactions.TransactionStatus = TransactionStatus.New;
                                accountTransactions.Location = "api";
                                accountTransactions.CustomerAccountNumber = drawViewModel.CustomerAccountNumber;
                                accountTransactions.AddUserId = _user.UserName;
                                accountTransactions.AddDate = DateTime.Now;
                                context.AccountTransactions.Add(accountTransactions);
                                context.SaveChanges();
                            }

                            var data = (from h in db.AccountTransactions
                                        where h.Id == accountTransactions.Id
                                        select new
                                        {
                                            id = h.Id,
                                            userName = h.UserName,
                                            name = h.Name,
                                            middleName = h.MiddleName,
                                            surName = h.SurName,
                                            amount = h.Amount,
                                            customerAccountNumber = h.CustomerAccountNumber,
                                            reference = h.Reference
                                        });

                            return Json(data.First());
                        }
                        else
                        {
                            JsonObjectViewModel jsonObject = new JsonObjectViewModel
                            {
                                type = "error",
                                message = "dataset is not valid"
                            };
                            return Json(jsonObject);
                        }
                    } else
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "pending transactions try later"
                        };
                        return Json(jsonObject);
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject);
                }
            }
            catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject);
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