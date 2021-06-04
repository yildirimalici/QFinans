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

                if (_user.UserName == userName && _user.Password == password && _user.MoneyTransfer == true)
                {
                    var userDeposit = db.MoneyTransfer.Where(x => x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.UserName == moneyTransferDepositViewModel.UserName).FirstOrDefault();
                    if (userDeposit != null)
                    {
                        if (_user.IsShowLastDepositForPendingDeposit == true)
                        {
                            var data = (from m in db.MoneyTransfer
                                        where m.Id == userDeposit.Id
                                        select new
                                        {
                                            id = m.Id,
                                            userName = m.UserName,
                                            name = m.Name,
                                            middleName = m.MiddleName,
                                            surName = m.SurName,
                                            amount = m.Amount,
                                            accountName = m.BankInfo.Name,
                                            accountSurName = m.BankInfo.Surname,
                                            bankName = m.BankInfo.BankType.Name,
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
                                message = "pending transactions try later"
                            };
                            return Json(jsonObject);
                        }
                    }

                    var customerBankInfo = db.CustomerBankInfo.Find(moneyTransferDepositViewModel.CustomerBankInfoId);
                    var minFastLimit = db.SystemParameters.Select(x => x.MinFastLimit).FirstOrDefault();
                    var maxFastLimit = db.SystemParameters.Select(x => x.MaxFastLimit).FirstOrDefault();

                    var currentTime = DateTime.Now.TimeOfDay;
                    var eftStartTime = db.SystemParameters.Select(x => x.EftStartTime).FirstOrDefault();
                    var eftEndTime = db.SystemParameters.Select(x => x.EftEndTime).FirstOrDefault();

                    var bankInfoHavale = db.BankInfo.Where(
                            x => x.IsDeleted == false
                            && x.IsPassive == false
                            && x.IsArchive == false
                            && x.MinAmount <= moneyTransferDepositViewModel.Amount
                            && x.MaxAmount >= moneyTransferDepositViewModel.Amount
                            && x.BankTypeId == customerBankInfo.BankTypeId
                        ).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                    var bankInfoEft = db.BankInfo.Where(
                            x => x.IsDeleted == false
                            && x.IsPassive == false
                            && x.IsArchive == false
                            && x.MinAmount <= moneyTransferDepositViewModel.Amount
                            && x.MaxAmount >= moneyTransferDepositViewModel.Amount
                            && x.BankTypeId != customerBankInfo.BankTypeId
                        ).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                    var bankInfoFast = db.BankInfo.Where(
                            x => x.IsDeleted == false
                            && x.IsPassive == false
                            && x.IsArchive == false
                            && x.IsFast == true
                        ).OrderBy(x => x.OrderNumber).ThenBy(x => x.Id).FirstOrDefault();

                    BankInfo bankInfo;

                    var _dayOfWeek = (int)DateTime.Now.DayOfWeek;

                    if (bankInfoHavale != null)
                    {
                        bankInfo = bankInfoHavale;
                    }
                    else if (currentTime >= eftStartTime && currentTime <= eftEndTime && bankInfoEft != null && _dayOfWeek >= 1 && _dayOfWeek <=5)
                    {
                        bankInfo = bankInfoEft;
                    }
                    else if (bankInfoHavale == null && moneyTransferDepositViewModel.Amount >= minFastLimit && moneyTransferDepositViewModel.Amount <= maxFastLimit)
                    {
                        bankInfo = bankInfoFast;
                    }
                    else if ((currentTime < eftStartTime || currentTime > eftEndTime) && bankInfoHavale == null && moneyTransferDepositViewModel.Amount > maxFastLimit)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "please send a request within eft hours"
                        };
                        return Json(jsonObject);
                    }
                    else if ((_dayOfWeek == 0 || _dayOfWeek == 6) && bankInfoHavale == null && moneyTransferDepositViewModel.Amount > maxFastLimit)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "please send a request within eft hours"
                        };
                        return Json(jsonObject);
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

                    if (bankInfo == null)
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
                        MoneyTransfer moneyTransfer = new MoneyTransfer();

                        using (var context = new ApplicationDbContext())
                        {
                            moneyTransfer.UserName = moneyTransferDepositViewModel.UserName;
                            moneyTransfer.Name = moneyTransferDepositViewModel.Name;
                            moneyTransfer.MiddleName = moneyTransferDepositViewModel.MiddleName;
                            moneyTransfer.SurName = moneyTransferDepositViewModel.SurName;
                            moneyTransfer.Amount = moneyTransferDepositViewModel.Amount;
                            moneyTransfer.OldAmount = moneyTransferDepositViewModel.Amount;
                            moneyTransfer.CustomerBankInfoId = moneyTransferDepositViewModel.CustomerBankInfoId;
                            moneyTransfer.Reference = moneyTransferDepositViewModel.Reference;
                            moneyTransfer.Deposit = true;
                            //moneyTransfer.IsMoneyTransfer = true;
                            moneyTransfer.TransactionStatus = TransactionStatus.New;
                            moneyTransfer.BankInfoId = bankInfo.Id;
                            moneyTransfer.Location = "api";
                            moneyTransfer.AddUserId = _user.UserName;
                            moneyTransfer.AddDate = DateTime.Now;
                            context.MoneyTransfer.Add(moneyTransfer);
                            context.SaveChanges();
                        }
                        
                        var data = (from m in db.MoneyTransfer
                                    where m.Id == moneyTransfer.Id
                                    select new
                                    {
                                        id = m.Id,
                                        userName = m.UserName,
                                        name = m.Name,
                                        middleName = m.MiddleName,
                                        surName = m.SurName,
                                        amount = m.Amount,
                                        accountName = m.BankInfo.Name,
                                        accountSurName = m.BankInfo.Surname,
                                        bankName = m.BankInfo.BankType.Name,
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

                if (_user.UserName == userName && _user.Password == password && _user.MoneyTransfer == true)
                {
                    var userDepositCount = db.MoneyTransfer.Where(x => x.Deposit == false && x.TransactionStatus == TransactionStatus.New && x.UserName == moneyTransferDrawViewModel.UserName).Count();
                    if (userDepositCount > 0)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "pending transactions try later"
                        };
                        return Json(jsonObject);
                    }

                    var userDrawCount = db.MoneyTransfer.Where(x => x.Deposit == false && x.TransactionStatus == TransactionStatus.New && x.UserName == moneyTransferDrawViewModel.UserName).Count();

                    if (userDrawCount == 0)
                    {
                        
                        if (ModelState.IsValid)
                        {
                            MoneyTransfer moneyTransfer = new MoneyTransfer();

                            using (var context = new ApplicationDbContext())
                            {
                                moneyTransfer.UserName = moneyTransferDrawViewModel.UserName;
                                moneyTransfer.Name = moneyTransferDrawViewModel.Name;
                                moneyTransfer.MiddleName = moneyTransferDrawViewModel.MiddleName;
                                moneyTransfer.SurName = moneyTransferDrawViewModel.SurName;
                                moneyTransfer.Amount = moneyTransferDrawViewModel.Amount;
                                moneyTransfer.OldAmount = moneyTransferDrawViewModel.Amount;
                                moneyTransfer.CustomerBankInfoId = moneyTransferDrawViewModel.CustomerBankInfoId;
                                moneyTransfer.CustomerIban = moneyTransferDrawViewModel.CustomerIban;
                                moneyTransfer.Reference = moneyTransferDrawViewModel.Reference;
                                moneyTransfer.Deposit = false;
                                //moneyTransfer.IsMoneyTransfer = true;
                                moneyTransfer.TransactionStatus = TransactionStatus.New;
                                moneyTransfer.Location = "api";
                                moneyTransfer.AddUserId = _user.UserName;
                                moneyTransfer.AddDate = DateTime.Now;
                                context.MoneyTransfer.Add(moneyTransfer);
                                context.SaveChanges();
                            }

                            var data = (from m in db.MoneyTransfer
                                        where m.Id == moneyTransfer.Id
                                        select new
                                        {
                                            id = m.Id,
                                            userName = m.UserName,
                                            name = m.Name,
                                            middleName = m.MiddleName,
                                            surName = m.SurName,
                                            amount = m.Amount,
                                            customerBankInfoId = m.CustomerBankInfoId,
                                            customerBankName = m.CustomerBankInfo.BankType.Name,
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