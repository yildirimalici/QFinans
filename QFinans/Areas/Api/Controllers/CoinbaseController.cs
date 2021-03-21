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
    public class CoinbaseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public JsonResult Deposit(DepositViewModelCoin depositViewModelCoin)
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
                    return Json(jsonObject);
                }

                if (_user.UserName == userName && _user.Password == password)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    Cryptocurrency cryptocurrency = db.Cryptocurrency.Where(x => x.IsDeleted == false && x.UnitSymbol == depositViewModelCoin.UnitSymbol).FirstOrDefault();

                    if (cryptocurrency != null)
                    {
                        if (depositViewModelCoin.Amount < cryptocurrency.MinAmount)
                        {
                            JsonObjectViewModel jsonObject = new JsonObjectViewModel
                            {
                                type = "error",
                                message = "please make an amount " + cryptocurrency.MinAmount.ToString("N0") + " ₺ or more"
                            };
                            return Json(jsonObject);
                        }
                    }
                    else
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "please select correct unit symbol"
                        };
                        return Json(jsonObject);
                    }

                    var userDepositCount = db.AccountTransactions.Where(x => x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.UserName == depositViewModelCoin.UserName && x.IsCoin == true && x.Amount == depositViewModelCoin.Amount).Count();

                    if (userDepositCount > 0)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "pending transactions try later"
                        };
                        return Json(jsonObject);
                    }
                    else
                    {
                        AccountTransactions accountTransactions = new AccountTransactions();

                        if (ModelState.IsValid)
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                try
                                {
                                    accountTransactions.UserName = depositViewModelCoin.UserName;
                                    accountTransactions.Name = depositViewModelCoin.Name;
                                    accountTransactions.SurName = depositViewModelCoin.SurName;
                                    accountTransactions.Amount = depositViewModelCoin.Amount;
                                    accountTransactions.OldAmount = depositViewModelCoin.Amount;
                                    accountTransactions.Reference = depositViewModelCoin.Reference;
                                    accountTransactions.UnitSymbol = depositViewModelCoin.UnitSymbol.ToUpper();
                                    accountTransactions.Deposit = true;
                                    accountTransactions.TransactionStatus = TransactionStatus.New;
                                    accountTransactions.Location = "api";
                                    accountTransactions.IsCoin = true;
                                    accountTransactions.AddUserId = _user.UserName;
                                    accountTransactions.AddDate = DateTime.Now;
                                    context.AccountTransactions.Add(accountTransactions);
                                    context.SaveChanges();

                                    CoinParameters coinParameter = db.CoinParameters.FirstOrDefault();

                                    WebRequest requestCoin = WebRequest.Create("https://api.commerce.coinbase.com/charges");
                                    var headersCoin = requestCoin.Headers;
                                    headersCoin["X-CC-Api-Key"] = coinParameter.ApiKey;
                                    headersCoin["X-CC-Version"] = coinParameter.Version;
                                    requestCoin.Method = "POST";
                                    requestCoin.ContentType = "application/json";

                                    AccountTransactions accountTransactions2 = context.AccountTransactions.Find(accountTransactions.Id);
                                    string postData = "{\"name\": \""
                                        + accountTransactions2.Id + " - " + accountTransactions2.Name + "\","
                                            + "\"description\": \"" + accountTransactions2.Id + " - " + accountTransactions2.UserName + " - " + accountTransactions2.Name + " " + accountTransactions2.SurName + " - " + accountTransactions2.Amount.ToString(nfi) + "\","
                                            + "\"local_price\": {"
                                            + "\"amount\": \"" + accountTransactions2.Amount.ToString(nfi) + "\","
                                            + "\"currency\": \"TRY\""
                                            + "},"
                                            + "\"pricing_type\": \"fixed_price\","
                                            + "\"metadata\": {"
                                            + "\"customer_id\": \"" + accountTransactions2.Id + "_" + accountTransactions2.UserName + "\","
                                            + "\"customer_name\": \"" + accountTransactions2.Name + " " + accountTransactions2.SurName + "\""
                                            + "},"
                                            + "\"redirect_url\": \"https://charge/completed/page\","
                                            + "\"redirect_url\": \"https://charge/canceled/page\""
                                        + "}";
                                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                                    requestCoin.ContentLength = byteArray.Length;
                                    Stream dataStream = requestCoin.GetRequestStream();
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    dataStream.Close();

                                    WebResponse responseCoin = requestCoin.GetResponse();
                                    using (dataStream = responseCoin.GetResponseStream())
                                    {
                                        StreamReader reader = new StreamReader(dataStream);
                                        string responseFromServer = reader.ReadToEnd();
                                        dynamic result = JObject.Parse(responseFromServer);

                                        accountTransactions2.ReferenceCoin = Convert.ToString(result.data.id);
                                        accountTransactions2.ResponseDateCoin = DateTime.Now;
                                        accountTransactions2.UrlCoin = Convert.ToString(result.data.hosted_url);
                                        accountTransactions2.UrlCoinApi = "https://api.commerce.coinbase.com/charges/" + Convert.ToString(result.data.code);
                                        accountTransactions2.ResponseMessageCoin = Convert.ToString(result.data);
                                        string _coinType = db.Cryptocurrency.Where(x => x.IsDeleted == false && x.UnitSymbol == depositViewModelCoin.UnitSymbol).Select(x => x.Name.ToLower()).FirstOrDefault();
                                        accountTransactions2.ExchangeValue = Convert.ToDouble(result.data.pricing[_coinType].amount);
                                        context.SaveChanges();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    AccountTransactions accountTransactions3 = context.AccountTransactions.Find(accountTransactions.Id);
                                    if (accountTransactions3 != null)
                                    {
                                        accountTransactions3.TransactionStatus = TransactionStatus.Deny;
                                        context.SaveChanges();
                                    }
                                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                                    {
                                        type = "error",
                                        message = ex.Message
                                    };
                                    return Json(jsonObject);
                                }
                            }

                            var dataCoin = (from h in db.AccountTransactions
                                            where h.Id == accountTransactions.Id
                                            select new
                                            {
                                                id = h.Id,
                                                userName = h.UserName,
                                                name = h.Name,
                                                surName = h.SurName,
                                                amount = h.Amount,
                                                referece = h.Reference,
                                                url = h.UrlCoin,
                                                unitSymbol = h.UnitSymbol
                                            });

                            return Json(dataCoin.First());

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
        public JsonResult Draw(DrawViewModelCoin drawViewModelCoin)
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
                    return Json(jsonObject);
                }

                if (_user.UserName == userName && _user.Password == password)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    Cryptocurrency cryptocurrency = db.Cryptocurrency.Where(x => x.IsDeleted == false && x.UnitSymbol == drawViewModelCoin.UnitSymbol).FirstOrDefault();

                    if (cryptocurrency != null)
                    {
                        if (drawViewModelCoin.Amount < cryptocurrency.MinAmount)
                        {
                            JsonObjectViewModel jsonObject = new JsonObjectViewModel
                            {
                                type = "error",
                                message = "please make an amount " + cryptocurrency.MinAmount.ToString("N0") + " ₺ or more"
                            };
                            return Json(jsonObject);
                        }
                    } else
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "please select correct unit symbol"
                        };
                        return Json(jsonObject);
                    }

                    var userDepositCount = db.AccountTransactions.Where(x => x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.UserName == drawViewModelCoin.UserName && x.IsCoin == true && x.Amount == drawViewModelCoin.Amount).Count();

                    if (userDepositCount > 0)
                    {
                        JsonObjectViewModel jsonObject = new JsonObjectViewModel
                        {
                            type = "error",
                            message = "pending transactions try later"
                        };
                        return Json(jsonObject);
                    }
                    else
                    {
                        AccountTransactions accountTransactions = new AccountTransactions();

                        if (ModelState.IsValid)
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                try
                                {
                                    string _pairSymbol = drawViewModelCoin.UnitSymbol.ToUpper() + "_TRY";
                                    string _url = "https://api.btcturk.com/api/v2/ticker?pairSymbol=" + _pairSymbol;
                                    try
                                    {
                                        WebRequest request = WebRequest.Create(_url);
                                        request.Method = "GET";
                                        request.ContentType = "application/x-www-form-urlencoded";

                                        WebResponse response = request.GetResponse();
                                        using (var dataStream = response.GetResponseStream())
                                        {
                                            StreamReader reader = new StreamReader(dataStream);
                                            string responseFromServer = reader.ReadToEnd();
                                            JObject resultResolve = JObject.Parse(responseFromServer);
                                            response.Close();
                                            accountTransactions.CoinConvertTimestamp = resultResolve["data"][0]["timestamp"].ToString();
                                            accountTransactions.ExchangeUnitValue = Convert.ToDecimal(resultResolve["data"][0]["last"]);
                                            accountTransactions.ExchangeValue = Convert.ToDouble(drawViewModelCoin.Amount) / Convert.ToDouble(accountTransactions.ExchangeUnitValue);
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

                                    accountTransactions.UserName = drawViewModelCoin.UserName;
                                    accountTransactions.Name = drawViewModelCoin.Name;
                                    accountTransactions.SurName = drawViewModelCoin.SurName;
                                    accountTransactions.Amount = drawViewModelCoin.Amount;
                                    accountTransactions.OldAmount = drawViewModelCoin.Amount;
                                    accountTransactions.Reference = drawViewModelCoin.Reference;
                                    accountTransactions.UnitSymbol = drawViewModelCoin.UnitSymbol.ToUpper();
                                    accountTransactions.CustomerWalletAddress = drawViewModelCoin.CustomerWalletAddress;
                                    accountTransactions.DestinationTag = drawViewModelCoin.CustomerDestinationTag;
                                    accountTransactions.Deposit = false;
                                    accountTransactions.TransactionStatus = TransactionStatus.New;
                                    accountTransactions.Location = "api";
                                    accountTransactions.IsCoin = true;
                                    accountTransactions.AddUserId = _user.UserName;
                                    accountTransactions.AddDate = DateTime.Now;
                                    context.AccountTransactions.Add(accountTransactions);
                                    context.SaveChanges();
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

                            var dataCoin = (from h in db.AccountTransactions
                                            where h.Id == accountTransactions.Id
                                            select new
                                            {
                                                id = h.Id,
                                                userName = h.UserName,
                                                name = h.Name,
                                                surName = h.SurName,
                                                amount = h.Amount,
                                                referece = h.Reference,
                                                //url = h.UrlCoin,
                                                unitSymbol = h.UnitSymbol,
                                                customerWalletAddress = h.CustomerWalletAddress,
                                                customerDestinationTag = h.DestinationTag
                                            });

                            return Json(dataCoin.First());

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

        [HttpGet]
        public JsonResult CheckCoinPayments()
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                Guid key = new Guid(headers["Key"]);
                string userName = headers["UserName"];
                string password = headers["Password"];
                string job = headers["Job"];
                //Guid key = new Guid("63359b40-a7cb-4ec6-afd4-d30d1b4c682a");
                //string userName = "user_coinbase";
                //string password = "da476a94-6f5d-4450-b9d3-d4b79e2d989f";
                //string job = "true";

                JsonObjectViewModel jsonObject = new JsonObjectViewModel();

                var _user = db.ApiUsers.Where(x => x.Key == key).FirstOrDefault();

                if (_user == null)
                {
                    jsonObject.type = "error";
                    jsonObject.message = "unauthorized";
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.Job == job)
                {
                    CoinParameters _params = db.CoinParameters.FirstOrDefault();

                    List<AccountTransactions> coins = db.AccountTransactions.Where(x => x.Deposit == true && x.TransactionStatus == TransactionStatus.New && x.IsCoin == true).ToList();

                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    if (coins.Count() > 0)
                    {
                        foreach (var _coin in coins)
                        {
                            DateTime _dateNow = DateTime.Now;
                            TimeSpan _span = _dateNow.Subtract(_coin.AddDate);

                            if ((int)_span.TotalMinutes > _params.WaitingTime)
                            {
                                string _urlCancel = _coin.UrlCoinApi + "/cancel";
                                WebRequest requestCoin = WebRequest.Create(_urlCancel);
                                var headersCoin = requestCoin.Headers;
                                headersCoin["X-CC-Api-Key"] = _params.ApiKey;
                                headersCoin["X-CC-Version"] = _params.Version;
                                requestCoin.Method = "POST";

                                WebResponse responseCoin = requestCoin.GetResponse();
                                using (var dataStreamCoin = responseCoin.GetResponseStream())
                                {
                                    StreamReader readerCoin = new StreamReader(dataStreamCoin);
                                    string responseFromServerCoin = readerCoin.ReadToEnd();
                                    JObject resultCoin = JObject.Parse(responseFromServerCoin);
                                    var _status = resultCoin["data"]["timeline"];
                                    foreach (var item in _status)
                                    {
                                        if ((string)item["status"] == "CANCELED")
                                        {
                                            _coin.TransactionStatus = TransactionStatus.Deny;
                                            db.SaveChanges();
                                            CallBackApiWebRequestForCoin(_coin.Id);
                                        }
                                    }
                                }
                                
                            }
                            else
                            {
                                WebRequest requestCoin = WebRequest.Create(_coin.UrlCoinApi);
                                var headersCoin = requestCoin.Headers;
                                //headersCoin["X-CC-Api-Key"] = "916702e8-57eb-4c6a-a91c-bef2d1b2c1cb";
                                //headersCoin["Content-Type"] = "application/json";
                                //headersCoin["X-CC-Version"] = "2018-03-22";
                                headersCoin["X-CC-Api-Key"] = _params.ApiKey;
                                headersCoin["X-CC-Version"] = _params.Version;
                                requestCoin.Method = "GET";

                                WebResponse responseCoin = requestCoin.GetResponse();
                                using (var dataStreamCoin = responseCoin.GetResponseStream())
                                {
                                    StreamReader readerCoin = new StreamReader(dataStreamCoin);
                                    string responseFromServerCoin = readerCoin.ReadToEnd();
                                    JObject resultCoin = JObject.Parse(responseFromServerCoin);
                                    var _status = resultCoin["data"]["timeline"];
                                    
                                    foreach (var item in _status)
                                    {
                                        if ((string)item["status"] == "UNRESOLVED")
                                        {
                                            foreach (var item2 in _status)
                                            {
                                                if ((string)item2["status"] != "RESOLVED")
                                                {
                                                    WebRequest requestCoinResolve = WebRequest.Create(_coin.UrlCoinApi + "/resolve");
                                                    var headersCoinResolve = requestCoinResolve.Headers;
                                                    headersCoinResolve["X-CC-Api-Key"] = _params.ApiKey;
                                                    headersCoinResolve["X-CC-Version"] = _params.Version;
                                                    requestCoinResolve.Method = "POST";

                                                    WebResponse responseCoinResolve = requestCoinResolve.GetResponse();
                                                    using (var dataStreamCoinResolve = responseCoinResolve.GetResponseStream())
                                                    {
                                                        StreamReader readerCoinResolve = new StreamReader(dataStreamCoinResolve);
                                                        string responseFromServerCoinResolve = readerCoinResolve.ReadToEnd();
                                                        JObject resultCoinResolve = JObject.Parse(responseFromServerCoinResolve);
                                                        var _statusResolve = resultCoinResolve["data"]["timeline"];

                                                        foreach (var itemResolve in _statusResolve)
                                                        {
                                                            if ((string)item["status"] == "RESOLVED")
                                                            {
                                                                _coin.TransactionStatus = TransactionStatus.Confirm;
                                                                _coin.PaymentLocalAmount = (double)resultCoinResolve["data"]["payments"][0]["value"]["local"]["amount"];
                                                                _coin.PaymentLocalCurrency = (string)resultCoinResolve["data"]["payments"][0]["value"]["local"]["currency"];
                                                                _coin.PaymentCriytoAmount = (double)resultCoinResolve["data"]["payments"][0]["value"]["crypto"]["amount"];
                                                                _coin.PaymentCriytoCurrency = (string)resultCoinResolve["data"]["payments"][0]["value"]["crypto"]["currency"];
                                                                _coin.Amount = (decimal)resultCoinResolve["data"]["payments"][0]["value"]["local"]["amount"];
                                                                db.SaveChanges();
                                                                CallBackApiWebRequestForCoin(_coin.Id);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((string)item["status"] == "COMPLETED")
                                        {
                                            WebRequest requestCoinResolve = WebRequest.Create(_coin.UrlCoinApi);
                                            var headersCoinResolve = requestCoinResolve.Headers;
                                            headersCoinResolve["X-CC-Api-Key"] = _params.ApiKey;
                                            headersCoinResolve["X-CC-Version"] = _params.Version;
                                            requestCoinResolve.Method = "GET";

                                            WebResponse responseCoinResolve = requestCoinResolve.GetResponse();
                                            using (var dataStreamCoinResolve = responseCoinResolve.GetResponseStream())
                                            {
                                                StreamReader readerCoinResolve = new StreamReader(dataStreamCoinResolve);
                                                string responseFromServerCoinResolve = readerCoinResolve.ReadToEnd();
                                                JObject resultCoinResolve = JObject.Parse(responseFromServerCoinResolve);
                                                var _statusResolve = resultCoinResolve["data"]["timeline"];

                                                foreach (var itemResolve in _statusResolve)
                                                {
                                                    _coin.PaymentLocalAmount = (double)resultCoinResolve["data"]["payments"][0]["value"]["local"]["amount"];
                                                    _coin.PaymentLocalCurrency = (string)resultCoinResolve["data"]["payments"][0]["value"]["local"]["currency"];
                                                    _coin.PaymentCriytoAmount = (double)resultCoinResolve["data"]["payments"][0]["value"]["crypto"]["amount"];
                                                    _coin.PaymentCriytoCurrency = (string)resultCoinResolve["data"]["payments"][0]["value"]["crypto"]["currency"];
                                                    _coin.TransactionStatus = TransactionStatus.Confirm;
                                                    db.SaveChanges();
                                                    CallBackApiWebRequestForCoin(_coin.Id);
                                                }
                                            }
                                        }
                                        else if ((string)item["status"] == "CANCELED" || (string)item["status"] == "EXPIRED")
                                        {
                                            _coin.TransactionStatus = TransactionStatus.Deny;
                                            db.SaveChanges();
                                            CallBackApiWebRequestForCoin(_coin.Id);
                                        }
                                    }

                                }
                            }
                        }

                        jsonObject.type = "success";
                        jsonObject.message = "success";
                    }
                    else
                    {
                        jsonObject.type = "error";
                        jsonObject.message = "there is no new transaction.";
                    }
                }
                else
                {
                    jsonObject.type = "error";
                    jsonObject.message = "user not found";
                }

                return Json(jsonObject, JsonRequestBehavior.AllowGet);
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

        private string CallBackApiWebRequestForCoin(int? transid)
        {
            try
            {
                var _callbackUrl = db.CallbackUrl.FirstOrDefault();
                //"https://www.app-dinamo.com/api/coinbase/callback"
                var _url = _callbackUrl.Coinbase;
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
                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "error",
                    message = ex.Message
                };
                using (var context = new ApplicationDbContext())
                {
                    AccountTransactions accountTransactions = context.AccountTransactions.Find(transid);
                    accountTransactions.ResponseType = jsonObject.type;
                    accountTransactions.ResponseMessage = jsonObject.message;
                    context.SaveChanges();
                }
                return ex.Message;
            }
        }

        private string GetCallBackApiData(int? id)
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
                    "&surname=" + at.SurName +
                    "&type=deposit" +
                    "&message=" + at.Note +
                    "&amount=" + at.Amount.ToString(nfi) +
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
                    "&amount=" + at.Amount.ToString(nfi) +
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
                    "&amount=" + at.Amount.ToString(nfi) +
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
                    "&amount=" + at.Amount.ToString(nfi) +
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