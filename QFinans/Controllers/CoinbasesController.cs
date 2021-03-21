using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Controllers
{
    public class Temp
    {
        public string type { get; set; }
        public string message { get; set; }
        public decimal amount { get; set; }
    }

    [Authorize]
    [Log]
    public class CoinbasesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Coinbase
        [CustomAuth(Roles = "IndexCoinbases")]
        public ActionResult Index()
        {
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == true && x.TransactionStatus == TransactionStatus.Confirm);
            return View(accountTransactions);
        }

        // GET: Coinbase
        [CustomAuth(Roles = "DetailsCoinbases")]
        public ActionResult Details(string unitSymbol, string currentFilter, string searchString, int? page, int? customPageSize)
        {
            if (string.IsNullOrEmpty(unitSymbol))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.UnitSymbol = unitSymbol;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == true && x.TransactionStatus == TransactionStatus.Confirm && x.UnitSymbol == unitSymbol);

            if (!String.IsNullOrEmpty(searchString))
            {
                accountTransactions = accountTransactions.Where(x => x.UserName.Contains(searchString) || x.Name.Contains(searchString) || x.SurName.Contains(searchString));

                if (accountTransactions.Any() == false)
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

            return View(accountTransactions.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public JsonResult Convert(string id)
        {
            try
            {
                string _pairSymbol = id.ToUpper() + "_TRY";
                string _url = "https://api.btcturk.com/api/v2/ticker?pairSymbol=" + _pairSymbol;

                try
                {
                    Temp jsonObject = new Temp();

                    WebRequest request = WebRequest.Create(_url);
                    request.Method = "GET";
                    request.ContentType = "application/x-www-form-urlencoded";

                    WebResponse response = request.GetResponse();
                    using (var dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        JObject resultResolve = JObject.Parse(responseFromServer);
                        decimal _value = (decimal)resultResolve["data"][0]["last"];
                        jsonObject.type = "success";
                        jsonObject.message = null;
                        jsonObject.amount = _value;
                        response.Close();
                    }

                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Temp jsonObject = new Temp
                    {
                        type = "error",
                        message = ex.Message,
                        amount = 0
                    };
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Temp jsonObject = new Temp
                {
                    type = "error",
                    message = ex.Message,
                    amount = 0
                };
                return Json(jsonObject, JsonRequestBehavior.AllowGet);
            }
        }
    }
}