using Newtonsoft.Json.Linq;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Areas.Api.Controllers
{
    [Log]
    public class BotController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public JsonResult Deposit()
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var data = (from x in db.AccountTransactions
                                    where x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.IsBotCheck == false && x.IsCoin == false && x.IsMoneyTransfer == false
                                    select new
                                    {
                                        id = x.Id,
                                        username = x.UserName,
                                        name = x.Name + " " + x.SurName,
                                        amount = x.Amount,
                                        accountNumber = x.AccountInfo.AccountNumber.ToString(),
                                        accountName = x.AccountInfo.Name + " " + x.AccountInfo.SurName,
                                        date = x.AddDate
                                    }).ToList()
                                    .Select(x => new
                                    {
                                        x.id,
                                        x.username,
                                        x.name,
                                        x.amount,
                                        x.accountNumber,
                                        x.accountName,
                                        date = x.date.ToString("yyyy-MM-dd HH:mm:ss.fff")
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

        [HttpPost]
        public JsonResult Confirm(int? id)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    if (id == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    var transaction = db.AccountTransactions.Find(id);
                    transaction.TransactionStatus = TransactionStatus.Confirm;
                    transaction.IsBotCheck = true;
                    transaction.UpdateUserId = _user.UserName;
                    transaction.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                    var result = CallBackApiWebRequestForBot(transaction.Id);

                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "success",
                        message = result
                    };
                    return Json(jsonObject);
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
        public JsonResult Reject(int? id)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    if (id == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    var transaction = db.AccountTransactions.Find(id);
                    transaction.TransactionStatus = TransactionStatus.Deny;
                    transaction.IsBotCheck = true;
                    transaction.UpdateUserId = _user.UserName;
                    transaction.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                    string result = CallBackApiWebRequestForBot(transaction.Id);

                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "success",
                        message = result
                    };
                    return Json(jsonObject);
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
        public JsonResult Edit(int? id, decimal? amount, long? accountNumber)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    if (id == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    var transaction = db.AccountTransactions.Find(id);


                    if (amount != null)
                    {
                        transaction.Amount = (decimal)amount;
                    }
                    
                    if (accountNumber != null)
                    {
                        var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.AccountNumber == accountNumber).FirstOrDefault();
                        if (accountInfo != null)
                        {
                            transaction.AccountInfoId = accountInfo.Id;
                        }
                    }

                    transaction.IsBotCheck = true;
                    transaction.UpdateUserId = _user.UserName;
                    transaction.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "success",
                        message = "success"
                    };
                    return Json(jsonObject);
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
        public JsonResult SetBotCheck(int? id)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    if (id == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    var transaction = db.AccountTransactions.Find(id);
                    transaction.IsBotCheck = true;
                    transaction.UpdateUserId = _user.UserName;
                    transaction.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "success",
                        message = "success"
                    };
                    return Json(jsonObject);
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
        public JsonResult AddCashIn(decimal? amount, long? accountNumber, string explanation)
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];

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

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    if (amount == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    if (accountNumber == null)
                    {
                        return Json(new JsonObjectViewModel
                        {
                            type = "error",
                            message = "id can not be null"
                        });
                    }

                    var accountInfo = db.AccountInfo.Where(x => x.IsDeleted == false && x.AccountNumber == accountNumber).FirstOrDefault();

                    if (accountInfo != null)
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            CashFlow cashFlow = new CashFlow();
                            cashFlow.TransactionDate = DateTime.Now;
                            cashFlow.AccountInfoId = accountInfo.Id;
                            cashFlow.IsCashIn = true;
                            cashFlow.Amount = (decimal)amount;
                            cashFlow.IsFree = false;
                            cashFlow.Explanation = explanation;
                            cashFlow.AddUserId = _user.UserName;
                            cashFlow.AddDate = DateTime.Now;
                            context.CashFlow.Add(cashFlow);
                            context.SaveChanges();
                        }

                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "success",
                            message = "record added"
                        };
                        return Json(jsonObject);
                    }
                    else
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "account not found"
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

        private string CallBackApiWebRequestForBot(int? transid)
        {
            try
            {
                var _callbackUrl = db.CallbackUrl.FirstOrDefault();
                //"https://www.app-dinamo.com/api/paparapro/callback"
                var _url = _callbackUrl.Papara;
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

                    using (var context = new ApplicationDbContext())
                    {
                        AccountTransactions accountTransactions = context.AccountTransactions.Find(transid);
                        accountTransactions.ResponseDate = DateTime.Now;
                        accountTransactions.ResponseType = jsonObject.type;
                        accountTransactions.ResponseMessage = jsonObject.message;
                        context.SaveChanges();
                    }

                    response.Close();
                }

                return "success";
            }
            catch (Exception ex)
            {
                using (var context = new ApplicationDbContext())
                {
                    AccountTransactions accountTransactions = context.AccountTransactions.Find(transid);
                    accountTransactions.ResponseType = "error";
                    accountTransactions.ResponseMessage = ex.Message;
                    context.SaveChanges();
                }
                return ex.Message;
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
                    "&surname=" + at.SurName +
                    "&type=draw" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount +
                    "&status=reject";
                return transaction;
            }
            else
            {
                string transaction = "";
                return transaction;
            }
        }
    }
}