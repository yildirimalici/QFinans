using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Areas.Api.Controllers
{
    [LogJson]
    public class DataTransferController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public JsonResult Transaction(int? transactionId)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.Job == "true")
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var data = (from x in context.AccountTransactions
                                    where x.Id>transactionId && x.TransactionStatus == TransactionStatus.Confirm && x.IsMoneyTransfer == false
                                    orderby x.Id
                                    select new
                                    {
                                        id = x.Id,
                                        username = x.UserName,
                                        name = x.Name,
                                        middleName = x.MiddleName,
                                        surname = x.SurName,
                                        amount = x.Amount,
                                        isFree = x.IsFree,
                                        isDeposit = x.Deposit,
                                        isCooin = x.IsCoin,
                                        accountNumber = x.AccountInfo.AccountNumber.ToString(),
                                        accountName = x.AccountInfo.Name,
                                        accountSurname = x.AccountInfo.SurName,
                                        date = x.AddDate,
                                        addUser = x.AddUserId
                                    }).Take(100).ToList()
                                    .Select(x => new
                                    {
                                        x.id,
                                        x.username,
                                        x.name,
                                        x.middleName,
                                        x.surname,
                                        x.amount,
                                        x.isFree,
                                        x.isDeposit,
                                        x.isCooin,
                                        x.accountNumber,
                                        x.accountName,
                                        x.accountSurname,
                                        date = x.date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        x.addUser
                                    });
                        return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult HavaleEft(int? havaleEftId)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.Job == "true")
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var data = (from x in context.MoneyTransfer
                                    where x.Id > havaleEftId && x.TransactionStatus == TransactionStatus.Confirm
                                    orderby x.Id
                                    select new
                                    {
                                        id = x.Id,
                                        username = x.UserName,
                                        name = x.Name,
                                        middleName = x.MiddleName,
                                        surname = x.SurName,
                                        amount = x.Amount,
                                        isDeposit = x.Deposit,
                                        iban = x.BankInfo.Iban,
                                        bankId = x.Id,
                                        bankName = x.BankInfo.BankType.Name,
                                        accountName = x.BankInfo.Name,
                                        accountSurname = x.BankInfo.Surname,
                                        date = x.AddDate,
                                        addUser = x.AddUserId
                                    }).Take(100).ToList()
                                    .Select(x => new
                                    {
                                        x.id,
                                        x.username,
                                        x.name,
                                        x.middleName,
                                        x.surname,
                                        x.amount,
                                        x.isDeposit,
                                        x.iban,
                                        x.bankId,
                                        x.bankName,
                                        x.accountName,
                                        x.accountSurname,
                                        date = x.date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        x.addUser
                                    });
                        return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CashFlow(int cashFlowId)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.Job == "true")
                {
                    
                    using (var context = new ApplicationDbContext())
                    {
                        var data = (from x in context.CashFlow
                                    where x.Id > cashFlowId && x.IsDeleted == false
                                    orderby x.Id
                                    select new
                                    {
                                        id = x.Id,
                                        cashFlowTypeId = x.CashFlowTypeId,
                                        cashFlowType = x.CashFlowType.Name,
                                        amount = x.Amount,
                                        isFree = x.IsFree,
                                        isCahIn = x.IsCashIn,
                                        isTransfer = x.IsTransfer,
                                        explanation = x.Explanation,
                                        accountNumber = x.AccountInfo.AccountNumber.ToString(),
                                        accountName = x.AccountInfo.Name,
                                        accountSurname = x.AccountInfo.SurName,
                                        date = x.TransactionDate,
                                        addUser = x.AddUserId
                                    }).Take(100).ToList()
                                    .Select(x => new
                                    {
                                        x.id,
                                        x.cashFlowTypeId,
                                        x.cashFlowType,
                                        x.amount,
                                        x.isFree,
                                        x.isCahIn,
                                        x.isTransfer,
                                        x.explanation,
                                        x.accountNumber,
                                        x.accountName,
                                        x.accountSurname,
                                        date = x.date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                        x.addUser
                                    });
                        return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult Parameters()
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "unauthorized"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.Job == "true")
                {
                    using (var context = new ApplicationDbContext())
                    {
                        IQueryable<AccountTransactions> accountTransactions = context.AccountTransactions.Where(x => x.IsCoin == false && x.IsMoneyTransfer == false && x.TransactionStatus == TransactionStatus.Confirm);

                        if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
                        {
                            accountTransactions = context.AccountTransactions.Where(x => x.IsCoin == false && x.AddUserId == "user_api" && x.IsMoneyTransfer == false && x.TransactionStatus == TransactionStatus.Confirm);
                        }

                        var _confirmDepositSum = accountTransactions.Where(x => x.Deposit == true).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                        var _confirmDrawSum = accountTransactions.Where(x => x.Deposit == false).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                        var _initialBalance = context.SystemParameters.Select(x => x.InitialBalance).DefaultIfEmpty(0).FirstOrDefault();
                        var _balanceEditAmount = context.SystemParameters.Select(x => x.BalanceEditAmount).DefaultIfEmpty(0).FirstOrDefault();
                        var _bankCharge = context.SystemParameters.Select(x => x.BankCharge).DefaultIfEmpty(0).FirstOrDefault();

                        var _balance = _initialBalance + _confirmDepositSum - (_confirmDepositSum * _bankCharge / 100) - _confirmDrawSum + _balanceEditAmount;
                        var _safe = context.AccountInfo.Where(a => a.IsDeleted == false && a.IsArchive == false).Select(x => x.Balance).DefaultIfEmpty(0).Sum() ?? 0;

                        DateTime startDate = DateTime.Now.Date;
                        DateTime endDate = DateTime.Now.AddDays(1).Date;
                        var _confirmDepositSumDaily = accountTransactions.Where(x => x.Deposit == true && x.AddDate >= startDate && x.AddDate < endDate).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                        var _confirmDrawSumDaily = accountTransactions.Where(x => x.Deposit == false && x.AddDate >= startDate && x.AddDate < endDate).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

                        var _paparaxBalance = context.Paparax.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Take(1).Select(x => x.Balance).DefaultIfEmpty(0).FirstOrDefault();

                        var data = (from x in context.SystemParameters
                                    orderby x.Id
                                    select new
                                    {
                                        confirmDepositSumDaily = _confirmDepositSumDaily,
                                        confirmDrawSumDaily = _confirmDrawSumDaily,
                                        balanceEditAmount = x.BalanceEditAmount,
                                        bankCharge = _bankCharge,
                                        balance = _balance,
                                        safe = _safe,
                                        paparaxBalance = _paparaxBalance,
                                        paparaxBalanceDiffParams = x.PaparaxBalaceDiff 
                                    }).Take(1).ToList();
                                    
                        return Json(data.First(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "user not found"
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                return Json(jsonObject, JsonRequestBehavior.AllowGet);
            }
        }
    }
}