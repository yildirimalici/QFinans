using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFinans.Areas.Api.Models;
using QFinans.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using QFinans.CustomFilters;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class CustomerBankInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerBankInfo
        [CustomAuth(Roles = "IndexCustomerBankInfo")]
        public ActionResult Index(string currentFilter, string searchString, int? page, int? customPageSize)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IQueryable<CustomerBankInfo> customerBankInfo = db.CustomerBankInfo.Where(x => x.IsDeleted == false).OrderBy(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                customerBankInfo = customerBankInfo.Where(x => x.BankType.Name.Contains(searchString));

                if (customerBankInfo.Any() == false)
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
            return View(customerBankInfo.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: CustomerBankInfo/Details/5
        [CustomAuth(Roles = "DetailsCustomerBankInfo")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankInfo customerBankInfo = await db.CustomerBankInfo.FindAsync(id);
            if (customerBankInfo == null)
            {
                return HttpNotFound();
            }
            return View(customerBankInfo);
        }

        // GET: CustomerBankInfo/Create
        [CustomAuth(Roles = "CreateCustomerBankInfo")]
        public ActionResult Create()
        {
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name");
            return View();
        }

        // POST: CustomerBankInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "CreateCustomerBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerBankInfo customerBankInfo)
        {
            string _userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                customerBankInfo.AddUserId = _userId;
                customerBankInfo.AddDate = DateTime.Now;
                db.CustomerBankInfo.Add(customerBankInfo);
                await db.SaveChangesAsync();
                TempData["success"] = "Kayıt eklendi.";
                return RedirectToAction("Index");
            }
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", customerBankInfo.BankTypeId);
            return View(customerBankInfo);
        }

        // GET: CustomerBankInfo/Edit/5
        [CustomAuth(Roles = "EditCustomerBankInfo")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankInfo customerBankInfo = await db.CustomerBankInfo.FindAsync(id);
            if (customerBankInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", customerBankInfo.BankTypeId);
            return View(customerBankInfo);
        }

        // POST: CustomerBankInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuth(Roles = "EditCustomerBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerBankInfo customerBankInfo)
        {
            string _userId = User.Identity.GetUserId();
            var newContext = new ApplicationDbContext();
            var orjData = newContext.CustomerBankInfo.Find(customerBankInfo.Id);
            if (orjData == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                customerBankInfo.AddUserId = orjData.AddUserId;
                customerBankInfo.AddDate = orjData.AddDate;
                customerBankInfo.UpdateUserId = _userId;
                customerBankInfo.UpdateDate = DateTime.Now;
                db.Entry(customerBankInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                newContext.Dispose();
                TempData["success"] = "Kayıt düzenlendi.";
                return RedirectToAction("Index");
            }
            newContext.Dispose();
            ViewBag.BankTypeId = new SelectList(db.BankType.Where(x => x.IsDeleted == false), "Id", "Name", customerBankInfo.BankTypeId);
            return View(customerBankInfo);
        }

        // GET: CustomerBankInfo/Delete/5
        [CustomAuth(Roles = "DeleteCustomerBankInfo")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankInfo customerBankInfo = await db.CustomerBankInfo.FindAsync(id);
            if (customerBankInfo == null)
            {
                return HttpNotFound();
            }
            return View(customerBankInfo);
        }

        // POST: CustomerBankInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteCustomerBankInfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string _userId = User.Identity.GetUserId();
            CustomerBankInfo customerBankInfo = await db.CustomerBankInfo.FindAsync(id);
            customerBankInfo.IsDeleted = true;
            customerBankInfo.UpdateUserId = _userId;
            customerBankInfo.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            TempData["success"] = "Kayıt silindi.";
            return RedirectToAction("Index");
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
