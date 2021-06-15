using Microsoft.Reporting.WebForms;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
                cashFlow = cashFlow.Where(x => x.TransactionDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                cashFlow = cashFlow.Where(x => x.TransactionDate <= dateTo);

            if (type == null) { type = currentType; }
            ViewBag.CurrentType = type;
            if (type.HasValue)
                if (type == 0)
                    cashFlow = cashFlow.Where(x => x.IsTransfer == true);
                else
                    cashFlow = cashFlow.Where(x => x.CashFlowTypeId == type);

            var data = cashFlow.OrderByDescending(x => x.TransactionDate).Select(x => new
            {
                Id = x.Id,
                Year = x.TransactionDate.Year,
                Month = x.TransactionDate.Month,
                Day = x.TransactionDate.Day,
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

        [CustomAuth(Roles = "UnRecordedPaparaDepositReport")]
        // GET: Report
        public ActionResult UnRecordedPaparaDeposit(DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo)
        {
            if (currentDateFrom == null)
            {
                currentDateFrom = DateTime.Now.Date;
            }

            IQueryable<UnRecordedDeposit> unRecordedDeposit = db.UnRecordedDeposit.Where(x => x.Papara == true && x.MoneyTransfer == false);

            if (dateFrom == null) { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                unRecordedDeposit = unRecordedDeposit.Where(x => x.AddDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                unRecordedDeposit = unRecordedDeposit.Where(x => x.AddDate <= dateTo);

            return View(unRecordedDeposit);
        }

        [CustomAuth(Roles = "UnRecordedMoneyTransferDepositReport")]
        // GET: Report
        public ActionResult UnRecordedMoneyTransferDeposit(DateTime? currentDateFrom, DateTime? currentDateTo, DateTime? dateFrom, DateTime? dateTo)
        {
            if (currentDateFrom == null)
            {
                currentDateFrom = DateTime.Now.Date;
            }

            IQueryable<UnRecordedDeposit> unRecordedDeposit = db.UnRecordedDeposit.Where(x => x.Papara == false && x.MoneyTransfer == true);

            if (dateFrom == null) { dateFrom = currentDateFrom; }
            ViewBag.CurrentDateFrom = dateFrom;
            if (dateFrom.HasValue)
                unRecordedDeposit = unRecordedDeposit.Where(x => x.AddDate >= dateFrom);

            if (dateTo == null) { dateTo = currentDateTo; }
            ViewBag.CurrentDateTo = dateTo;
            if (dateTo.HasValue)
                unRecordedDeposit = unRecordedDeposit.Where(x => x.AddDate <= dateTo);

            return View(unRecordedDeposit);
        }


        [CustomAuth(Roles = "AccountTransactionDetailsReport")]
        public ActionResult AccountTransactionDetails(string organization)
        {
            IQueryable<ApiUsers> _organization = db.ApiUsers.Where(x => x.Job == null);
            ViewBag.OrganizationList = _organization.ToList();

            if (organization == null)
            {
                organization = _organization.FirstOrDefault().Organization;
            }
            else if (organization == "Coinbase")
            {
                organization = "Coinbase";
            }
            else
            {
                string _organizationUsername = _organization.Where(x => x.Organization == organization).FirstOrDefault().UserName;
            }
            ViewBag.Organization = organization;
            return View();
        }

        [CustomAuth(Roles = "AccountTransactionDetailsReport")]
        public ActionResult AccountTransactionToExcel(string tip, DateTime? date, bool deposit, string organization)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "ReportAccountTransaction.rdlc");
            
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                TempData["warning"] = "Rapor dosyası bulunamadı.";
                return View("AccountTransactionDetails");
            }

            if (organization == null)
            {
                TempData["warning"] = "Organizasyon bulunamadı.";
                return View("AccountTransactionDetails");
            }

            DateTime starDate = date ?? DateTime.Now;
            DateTime endDate = starDate.AddDays(1);
            ViewBag.Tip = tip;
            ViewBag.Date = starDate;
            ViewBag.Deposit = starDate;
            ViewBag.Organization = organization;

            var _addUser = db.ApiUsers.Where(x => x.Organization == organization).Select(x => x.UserName).FirstOrDefault();

            List<AccountTransactions> at = new List<AccountTransactions>();

            at = (from a in db.AccountTransactions where a.AddDate>= date && a.AddDate<endDate && a.AddUserId == _addUser && a.Deposit == deposit && a.IsCoin == false && a.IsMoneyTransfer == false select a).ToList();

            ReportDataSource rdAccountTransactions = new ReportDataSource("DsAccountTransaction", at);

            lr.DataSources.Add(rdAccountTransactions);

            string reportType = tip;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tip + "</OutputFormat>" +
            "  <PageWidth>8.27in</PageWidth>" +
            "  <PageHeight>11.69in</PageHeight>" +
            "  <MarginTop>0in</MarginTop>" +
            "  <MarginLeft>0.19685in</MarginLeft>" +
            "  <MarginRight>0.07874in</MarginRight>" +
            "  <MarginBottom>0.07874in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
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