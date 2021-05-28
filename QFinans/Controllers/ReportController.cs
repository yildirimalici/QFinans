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
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [CustomAuth(Roles = "IndexReport")]
        // GET: Report
        public ActionResult Index(DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo, string organization, string currentOrganization)
        {
            if (currentDateFrom == null)
            {
                currentDateFrom = DateTime.Now.AddMonths(-1).Date;
            }

            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsMoneyTransfer == false);



            IQueryable<ApiUsers> _organization = db.ApiUsers.Where(x => x.Job == null);
            ViewBag.OrganizationList = _organization.ToList();

            if (organization == null)
            {
                organization = _organization.FirstOrDefault().Organization;
                accountTransactions = accountTransactions.Where(x => x.IsCoin == false && x.AddUserId == _organization.FirstOrDefault().UserName);
            }
            else if (organization == "Coinbase")
            {
                organization = "Coinbase";
                accountTransactions = accountTransactions.Where(x => x.IsCoin == true);
            }
            else
            {
                string _organizationUsername = _organization.Where(x => x.Organization == organization).FirstOrDefault().UserName;
                accountTransactions = accountTransactions.Where(x => x.IsCoin == false && x.AddUserId == _organizationUsername);
            }
            ViewBag.CurrentOrganization = organization;

            if (dateFrom == null) { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                accountTransactions = accountTransactions.Where(x => x.AddDate <= dateTo);

            var data = accountTransactions.OrderByDescending(x => x.AddDate).Select(x => new DailyTansactionReportViewModel
            {
                Id = x.Id,
                Year = x.AddDate.Year,
                Month = x.AddDate.Month,
                Day = x.AddDate.Day,
                NewDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,
                NewDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,

            }).ToList();

            var reportData = data.GroupBy(x => new { x.Year, x.Month, x.Day }).Select(g => new DailyTansactionReportSumViewModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Day = g.Key.Day,
                NewDepositCount = g.Sum(x => x.NewDepositCount),
                NewDepositSum = g.Sum(x => x.NewDepositSum),
                ConfirmDepositCount = g.Sum(x => x.ConfirmDepositCount),
                ConfirmDepositSum = g.Sum(x => x.ConfirmDepositSum),
                DenyDepositCount = g.Sum(x => x.DenyDepositCount),
                DenyDepositSum = g.Sum(x => x.DenyDepositSum),
                NewDrawCount = g.Sum(x => x.NewDrawCount),
                NewDrawSum = g.Sum(x => x.NewDrawSum),
                ConfirmDrawCount = g.Sum(x => x.ConfirmDrawCount),
                ConfirmDrawSum = g.Sum(x => x.ConfirmDrawSum),
                DenyDrawCount = g.Sum(x => x.DenyDrawCount),
                DenyDrawSum = g.Sum(x => x.DenyDrawSum)
            });

            //ViewBag.Data = data;

            var banCharge = db.SystemParameters.FirstOrDefault().BankCharge;
            if (banCharge == null)
            {
                ViewBag.BankCharge = 0;
            }
            else
            {
                ViewBag.BankCharge = banCharge;
            }

            return View(reportData);
        }

        [CustomAuth(Roles = "MonthlyReport")]
        public ActionResult Monthly()
        {
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.IsMoneyTransfer == false);

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                accountTransactions = db.AccountTransactions.Where(x => x.AddUserId == "user_api");
            }

            var data = accountTransactions.OrderByDescending(x => x.AddDate).Select(x => new DailyTansactionReportViewModel
            {
                Id = x.Id,
                Year = x.AddDate.Year,
                Month = x.AddDate.Month,
                Day = x.AddDate.Day,
                NewDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,
                NewDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,

            }).ToList();

            var reportData = data.GroupBy(x => new { x.Year, x.Month, x.Day }).Select(g => new DailyTansactionReportSumViewModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Day = g.Key.Day,
                NewDepositCount = g.Sum(x => x.NewDepositCount),
                NewDepositSum = g.Sum(x => x.NewDepositSum),
                ConfirmDepositCount = g.Sum(x => x.ConfirmDepositCount),
                ConfirmDepositSum = g.Sum(x => x.ConfirmDepositSum),
                DenyDepositCount = g.Sum(x => x.DenyDepositCount),
                DenyDepositSum = g.Sum(x => x.DenyDepositSum),
                NewDrawCount = g.Sum(x => x.NewDrawCount),
                NewDrawSum = g.Sum(x => x.NewDrawSum),
                ConfirmDrawCount = g.Sum(x => x.ConfirmDrawCount),
                ConfirmDrawSum = g.Sum(x => x.ConfirmDrawSum),
                DenyDrawCount = g.Sum(x => x.DenyDrawCount),
                DenyDrawSum = g.Sum(x => x.DenyDrawSum)
            });

            //ViewBag.Data = data;

            var banCharge = db.SystemParameters.FirstOrDefault().BankCharge;
            if (banCharge == null)
            {
                ViewBag.BankCharge = 0;
            }
            else
            {
                ViewBag.BankCharge = banCharge;
            }

            return View(reportData);
        }

        [CustomAuth(Roles = "BalanceReport")]
        public ActionResult Balance()
        {
            IQueryable<AccountTransactions> accountTransactions = db.AccountTransactions.Where(x => x.IsCoin == false && x.IsMoneyTransfer == false);

            if (Request.Url.Host == "www.qfinans.com" || Request.Url.Host == "qfinans.com")
            {
                accountTransactions = db.AccountTransactions.Where(x => x.AddUserId == "user_api");
            }

            var data = accountTransactions.OrderByDescending(x => x.AddDate).Select(x => new DailyTansactionReportViewModel
            {
                Id = x.Id,
                Year = x.AddDate.Year,
                Month = x.AddDate.Month,
                Day = x.AddDate.Day,
                NewDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,
                NewDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,

            }).ToList();

            var reportData = data.GroupBy(x => new { x.Year, x.Month, x.Day }).Select(g => new DailyTansactionReportSumViewModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Day = g.Key.Day,
                NewDepositCount = g.Sum(x => x.NewDepositCount),
                NewDepositSum = g.Sum(x => x.NewDepositSum),
                ConfirmDepositCount = g.Sum(x => x.ConfirmDepositCount),
                ConfirmDepositSum = g.Sum(x => x.ConfirmDepositSum),
                DenyDepositCount = g.Sum(x => x.DenyDepositCount),
                DenyDepositSum = g.Sum(x => x.DenyDepositSum),
                NewDrawCount = g.Sum(x => x.NewDrawCount),
                NewDrawSum = g.Sum(x => x.NewDrawSum),
                ConfirmDrawCount = g.Sum(x => x.ConfirmDrawCount),
                ConfirmDrawSum = g.Sum(x => x.ConfirmDrawSum),
                DenyDrawCount = g.Sum(x => x.DenyDrawCount),
                DenyDrawSum = g.Sum(x => x.DenyDrawSum)
            });

            //ViewBag.Data = data;

            var banCharge = db.SystemParameters.FirstOrDefault().BankCharge;
            if (banCharge == null)
            {
                ViewBag.BankCharge = 0;
            }
            else
            {
                ViewBag.BankCharge = banCharge;
            }

            return View(reportData);
        }

        [CustomAuth(Roles = "CashFlowReport")]
        public ActionResult CashFlow(DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo, int? type, int? currentType)
        {
            ViewBag.Type = db.CashFlowType.Where(x => x.IsDeleted == false).OrderBy(x => x.Name).ToList();

            if (currentDateFrom == null)
            {
                currentDateFrom = DateTime.Now.AddMonths(-1).Date;
            }

            IQueryable<CashFlow> cashFlow = db.CashFlow.Where(x => x.IsDeleted == false);

            if (dateFrom == null) { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                cashFlow = cashFlow.Where(x => x.AddDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                cashFlow = cashFlow.Where(x => x.AddDate <= dateTo);

            if (type == null) { type = currentType; }
            ViewBag.CurrentType = type;
            if (type.HasValue)
                if (type == 0)
                    cashFlow = cashFlow.Where(x => x.IsTransfer == true);
                else
                    cashFlow = cashFlow.Where(x => x.CashFlowTypeId == type);

            var data = cashFlow.OrderByDescending(x => x.AddDate).Select(x => new
            {
                Id = x.Id,
                Year = x.AddDate.Year,
                Month = x.AddDate.Month,
                Day = x.AddDate.Day,
                CashIn = x.IsCashIn == true ? x.Amount : 0,
                CashOut = x.IsCashIn == false ? x.Amount : 0

            }).ToList();

            var reportData = data.GroupBy(x => new { x.Year, x.Month, x.Day }).Select(g => new DailyTransactionReportCashFlowViewModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Day = g.Key.Day,
                CashIn = g.Sum(x => x.CashIn),
                CashOut = g.Sum(x => x.CashOut),
            });

            //ViewBag.Data = data;

            var banCharge = db.SystemParameters.FirstOrDefault().BankCharge;
            if (banCharge == null)
            {
                ViewBag.BankCharge = 0;
            }
            else
            {
                ViewBag.BankCharge = banCharge;
            }

            return View(reportData);
        }

        [CustomAuth(Roles = "DailyMoneyTransferReport")]
        // GET: Report
        public ActionResult DailyMoneyTransfer(DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo)
        {
            if (currentDateFrom == null)
            {
                currentDateFrom = DateTime.Now.AddMonths(-1).Date;
            }

            IQueryable<MoneyTransfer> moneyTransfer = db.MoneyTransfer;

            if (dateFrom == null) { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                moneyTransfer = moneyTransfer.Where(x => x.AddDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                moneyTransfer = moneyTransfer.Where(x => x.AddDate <= dateTo);

            var data = moneyTransfer.OrderByDescending(x => x.AddDate).Select(x => new DailyTansactionReportViewModel
            {
                Id = x.Id,
                Year = x.AddDate.Year,
                Month = x.AddDate.Month,
                Day = x.AddDate.Day,
                NewDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDepositCount = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDepositSum = x.Deposit == true && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,
                NewDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? 1 : 0,
                NewDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.New ? x.Amount : 0,
                ConfirmDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? 1 : 0,
                ConfirmDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm ? x.Amount : 0,
                DenyDrawCount = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? 1 : 0,
                DenyDrawSum = x.Deposit == false && x.TransactionStatus == Areas.Api.Models.TransactionStatus.Deny ? x.Amount : 0,

            }).ToList();

            var reportData = data.GroupBy(x => new { x.Year, x.Month, x.Day }).Select(g => new DailyTansactionReportSumViewModel
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Day = g.Key.Day,
                NewDepositCount = g.Sum(x => x.NewDepositCount),
                NewDepositSum = g.Sum(x => x.NewDepositSum),
                ConfirmDepositCount = g.Sum(x => x.ConfirmDepositCount),
                ConfirmDepositSum = g.Sum(x => x.ConfirmDepositSum),
                DenyDepositCount = g.Sum(x => x.DenyDepositCount),
                DenyDepositSum = g.Sum(x => x.DenyDepositSum),
                NewDrawCount = g.Sum(x => x.NewDrawCount),
                NewDrawSum = g.Sum(x => x.NewDrawSum),
                ConfirmDrawCount = g.Sum(x => x.ConfirmDrawCount),
                ConfirmDrawSum = g.Sum(x => x.ConfirmDrawSum),
                DenyDrawCount = g.Sum(x => x.DenyDrawCount),
                DenyDrawSum = g.Sum(x => x.DenyDrawSum)
            });

            //ViewBag.Data = data;

            var banCharge = db.SystemParameters.FirstOrDefault().BankCharge;
            if (banCharge == null)
            {
                ViewBag.BankCharge = 0;
            }
            else
            {
                ViewBag.BankCharge = banCharge;
            }

            return View(reportData);
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