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
    [Log]
    public class MoneyTransferController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public JsonResult Deposit(MoneyTransferDepositViewModel moneyTransferDepositViewModel)
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
                    var bankTypeId = db.CustomerBankInfo.Find(moneyTransferDepositViewModel.CustomerBankInfoId).BankTypeId;

                    var currentTime = DateTime.Now.TimeOfDay;
                    var eftStartTime = db.SystemParameters.Select(x => x.EftStartTime).FirstOrDefault();
                    var eftEndTime = db.SystemParameters.Select(x => x.EftEndTime).FirstOrDefault();

                    var bankInfoHavale = db.BankInfo.Where(x => x.IsDeleted == false && x.IsPassive == false && x.IsArchive == false && x.MinAmount <= moneyTransferDepositViewModel.Amount && x.BankTypeId == bankTypeId).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                    var bankInfoEft = db.BankInfo.Where(x => x.IsDeleted == false && x.IsPassive == false && x.IsArchive == false && x.MinAmount <= moneyTransferDepositViewModel.Amount && x.BankTypeId != bankTypeId).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                    BankInfo bankInfo;

                    if (bankInfoHavale != null)
                    {
                        bankInfo = bankInfoHavale;
                    }
                    else if (currentTime >= eftStartTime && currentTime <= eftEndTime && bankInfoEft != null)
                    {
                        bankInfo = bankInfoEft;
                    }
                    else
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "there is no available account"
                        };
                        return Json(jsonObject);
                    }

                    if (ModelState.IsValid)
                    {
                        AccountTransactions accountTransactions = new AccountTransactions();

                        using (var context = new ApplicationDbContext())
                        {
                            accountTransactions.UserName = moneyTransferDepositViewModel.UserName;
                            accountTransactions.Name = moneyTransferDepositViewModel.Name;
                            accountTransactions.SurName = moneyTransferDepositViewModel.SurName;
                            accountTransactions.Amount = moneyTransferDepositViewModel.Amount;
                            accountTransactions.OldAmount = moneyTransferDepositViewModel.Amount;
                            accountTransactions.CustomerBankInfoId = moneyTransferDepositViewModel.CustomerBankInfoId;
                            accountTransactions.Reference = moneyTransferDepositViewModel.Reference;
                            accountTransactions.Deposit = true;
                            accountTransactions.IsMoneyTransfer = true;
                            accountTransactions.TransactionStatus = TransactionStatus.New;
                            accountTransactions.BankInfoId = bankInfo.Id;
                            accountTransactions.Location = "api";
                            accountTransactions.AddUserId = _user.UserName;
                            accountTransactions.AddDate = DateTime.Now;
                            context.AccountTransactions.Add(accountTransactions);
                            context.SaveChanges();
                        }

                        var data = (from m in db.AccountTransactions
                                    where m.Id == accountTransactions.Id
                                    select new
                                    {
                                        id = m.Id,
                                        userName = m.UserName,
                                        name = m.Name,
                                        surName = m.SurName,
                                        amount = m.Amount,
                                        accountName = m.BankInfo.Name,
                                        accountSurName = m.BankInfo.Surname,
                                        branchCode = m.BankInfo.BranchCode,
                                        accountNumber = m.BankInfo.AccountNumber,
                                        iban = m.BankInfo.Iban,
                                        reference = m.Reference
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

        [HttpPost]
        public JsonResult Draw(MoneyTransferDrawViewModel moneyTransferDrawViewModel)
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
                    var userDrawCount = db.AccountTransactions.Where(x => x.Deposit == false && x.TransactionStatus == TransactionStatus.New && x.UserName == moneyTransferDrawViewModel.UserName).Count();

                    if (userDrawCount == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            AccountTransactions accountTransactions = new AccountTransactions();

                            using (var context = new ApplicationDbContext())
                            {
                                accountTransactions.UserName = moneyTransferDrawViewModel.UserName;
                                accountTransactions.Name = moneyTransferDrawViewModel.Name;
                                accountTransactions.SurName = moneyTransferDrawViewModel.SurName;
                                accountTransactions.Amount = moneyTransferDrawViewModel.Amount;
                                accountTransactions.OldAmount = moneyTransferDrawViewModel.Amount;
                                accountTransactions.CustomerBankInfoId = moneyTransferDrawViewModel.CustomerBankInfoId;
                                accountTransactions.CustomerIban = moneyTransferDrawViewModel.CustomerIban;
                                accountTransactions.Reference = moneyTransferDrawViewModel.Reference;
                                accountTransactions.Deposit = false;
                                accountTransactions.IsMoneyTransfer = true;
                                accountTransactions.TransactionStatus = TransactionStatus.New;
                                accountTransactions.Location = "api";
                                accountTransactions.AddUserId = _user.UserName;
                                accountTransactions.AddDate = DateTime.Now;
                                context.AccountTransactions.Add(accountTransactions);
                                context.SaveChanges();
                            }

                            var data = (from m in db.AccountTransactions
                                        where m.Id == accountTransactions.Id
                                        select new
                                        {
                                            id = m.Id,
                                            userName = m.UserName,
                                            name = m.Name,
                                            surName = m.SurName,
                                            amount = m.Amount,
                                            customerBankInfoId = m.CustomerBankInfoId,
                                            customerIban = m.CustomerIban,
                                            reference = m.Reference
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
                    }
                    else
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
    }
}