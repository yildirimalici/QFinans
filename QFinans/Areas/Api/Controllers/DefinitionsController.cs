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
    public class DefinitionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public JsonResult Cryptocurrency()
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

                if (_user.UserName == userName && _user.Password == password)
                {
                    var data = (from c in db.Cryptocurrency
                                where c.IsDeleted == false
                                select new
                                {
                                    name = c.Name,
                                    unitSymbol = c.UnitSymbol,
                                    minAmount = c.MinAmount
                                });
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
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
        public JsonResult CustomerBankInfo()
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
                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }

                if (_user.UserName == userName && _user.Password == password && _user.MoneyTransfer == true)
                {
                    var data = (from d in db.CustomerBankInfo
                                where d.IsDeleted == false && d.IsActive == true
                                select new
                                {
                                    id = d.Id,
                                    name = d.BankType.Name
                                });
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
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