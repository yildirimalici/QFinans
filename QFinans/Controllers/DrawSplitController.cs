using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class DrawSplitController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexDrawSplit")]
        // GET: DrawSplit
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuth(Roles = "CreateDrawSplit")]
        public JsonResult Create(int accountTransactionsId, int accountInfoId, decimal amount)
        {
            string _userId = User.Identity.GetUserId();

            try
            {
                var _totalAmount = db.AccountTransactions.Find(accountTransactionsId).Amount;
                var _totalDrawSplitAmount = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == accountTransactionsId).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                if ((_totalDrawSplitAmount + amount) > _totalAmount)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "Bölme işleminin toplam tutarı çekim tutarından fazla olmaz."
                    };

                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
                DrawSplit drawSplit = new DrawSplit();
                drawSplit.AccountTransactionsId = accountTransactionsId;
                drawSplit.AccountInfoId = accountInfoId;
                drawSplit.Amount = amount;
                drawSplit.IsDeleted = false;
                drawSplit.AddUserId = _userId;
                drawSplit.AddDate = DateTime.Now;
                db.DrawSplit.Add(drawSplit);
                db.SaveChanges();

                var data = (from h in db.DrawSplit
                            where h.Id == drawSplit.Id
                            select new
                            {
                                id = h.Id,
                                hesap_id = h.AccountInfoId,
                                hesap = h.AccountInfo.Name + " " + h.AccountInfo.SurName + "(" + h.AccountInfo.AccountNumber + ")",
                                amount = h.Amount,
                                type = "success",
                                message = "Kayıt eklendi."
                            });

                return Json(data.First(), JsonRequestBehavior.AllowGet);
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
        [CustomAuth(Roles = "EditDrawSplit")]
        public JsonResult Edit(int id, int accountTransactionsId, int accountInfoId, decimal amount)
        {
            string _userId = User.Identity.GetUserId();

            try
            {
                var _totalAmount = db.AccountTransactions.Find(accountTransactionsId).Amount;
                var _totalDrawSplitAmount = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == accountTransactionsId && x.Id != id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                if ((_totalDrawSplitAmount + amount) > _totalAmount)
                {
                    JsonObjectViewModel jsonObject = new JsonObjectViewModel
                    {
                        type = "error",
                        message = "Bölme işleminin toplam tutarı çekim tutarından fazla olmaz."
                    };

                    return Json(jsonObject, JsonRequestBehavior.AllowGet);
                }
                DrawSplit drawSplit = db.DrawSplit.Find(id);
                drawSplit.AccountTransactionsId = accountTransactionsId;
                drawSplit.AccountInfoId = accountInfoId;
                drawSplit.Amount = amount;
                drawSplit.IsDeleted = false;
                drawSplit.UpdateUserId = _userId;
                drawSplit.UpdateDate = DateTime.Now;
                db.SaveChanges();

                var data = (from h in db.DrawSplit
                            where h.Id == id
                            select new
                            {
                                id = h.Id,
                                hesap_id = h.AccountInfoId,
                                hesap = h.AccountInfo.Name + " " + h.AccountInfo.SurName + "(" + h.AccountInfo.AccountNumber + ")",
                                amount = h.Amount,
                                type = "success",
                                message = "Kayıt düzenlendi."
                            });

                return Json(data.First(), JsonRequestBehavior.AllowGet);
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
        [CustomAuth(Roles = "DeleteDrawSplit")]
        public JsonResult Delete(int id)
        {
            string _userId = User.Identity.GetUserId();

            try
            {

                DrawSplit drawSplit = db.DrawSplit.Find(id);
                drawSplit.IsDeleted = true;
                drawSplit.UpdateUserId = _userId;
                drawSplit.UpdateDate = DateTime.Now;
                db.SaveChanges();

                JsonObjectViewModel jsonObject = new JsonObjectViewModel
                {
                    type = "success",
                    message = "Kayıt silindi."
                };

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

        [HttpPost]
        [CustomAuth(Roles = "IndexDrawSplit")]
        public JsonResult Sum(int id)
        {
            decimal total = db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountTransactionsId == id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            decimal amount = db.AccountTransactions.Find(id).Amount;
            decimal remaining = amount - total;
            string data = "Toplam: " + total.ToString("N0") + " / Kalan: " + remaining.ToString("N0");

            JsonObjectViewModel jsonObject = new JsonObjectViewModel
            {
                type = "success",
                message = data
            };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }

    }
}