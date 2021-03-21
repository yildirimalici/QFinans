using QFinans.Areas.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class AccountInfoViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public DateTime Date
        {
            get
            {
                DateTime _date = DateTime.Now.Date;
                return _date;
            }
        }
        public DateTime StartDateMonth
        {
            get
            {
                DateTime _date = DateTime.Now.Date;
                DateTime _dateStart = new DateTime(Date.Year, Date.Month, 1);
                return _dateStart;
            }
        }
        public DateTime EndDateMonth
        {
            get
            {
                DateTime _date = DateTime.Now.Date;
                DateTime _dateStart = new DateTime(Date.Year, Date.Month, 1);
                DateTime _dateEnd = _dateStart.AddMonths(1).AddSeconds(-1);
                return _dateEnd;
            }
        }

        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        public string SurName { get; set; }

        [Display(Name = "Hesap No")]
        public Int64 AccountNumber { get; set; }

        [Display(Name = "Durum")]
        public bool IsPassive { get; set; }

        [Display(Name = "Telefon")]
        public string Phone => db.AccountInfo.Find(Id).Phone;

        [Display(Name = "Email")]
        public string Email => db.AccountInfo.Find(Id).Email;

        [Display(Name = "Sim Lokasyonu")]
        public string SimLocation => db.AccountInfo.Find(Id).SimLocation.Name;

        public int? SimLocationId { get; set; }

        [Display(Name = "Hesap Durumu")]
        public string AccountInfoStatus => db.AccountInfo.Find(Id).AccountInfoStatus.Name;

        [Display(Name = "İşlem Sayısı")]
        public int TransactionCount => db.AccountTransactions.Where(x => x.Deposit == true && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.AddDate >= StartDateMonth && x.AddDate <= EndDateMonth).Count() + db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.AccountInfoId == Id && x.TransactionDate >= StartDateMonth && x.TransactionDate <= EndDateMonth).Count();

        [Display(Name = "Farklı Kişilerden Gelen İşlem Sayısı")]
        public int DifferentUsersTransactionCount => db.AccountTransactions.Where(x => x.Deposit == true && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.AddDate >= StartDateMonth && x.AddDate <= EndDateMonth).Select(x => x.UserName).Distinct().Count() + db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.AccountInfoId == Id && x.TransactionDate >= StartDateMonth && x.TransactionDate <= EndDateMonth).Count();

        [Display(Name = "Hesap Türü")]
        public string AccountInfoTypeName => db.AccountInfo.Find(Id).AccountInfoType.Name;
        public int AccountInfoTypeId { get; set; }

        [Display(Name = "Hesap Türü İşlem Sayısı Limiti")]
        public int AccountInfoTypeTransactionLimit => db.AccountInfo.Find(Id).AccountInfoType.TransactionLimit;

        [Display(Name = "Hesap Türü Tutar Limiti")]
        public decimal AccountInfoTypeAmountLimit => db.AccountInfo.Find(Id).AccountInfoType.AmountLimit;

        [Display(Name = "Nakit Yatırma Limiti")]
        public decimal AccountInfoTypeCashInAmountLimit => db.AccountInfo.Find(Id).AccountInfoType.CashInAmountLimit;

        [Display(Name = "Tutar Yönlendirme Türü")]
        public string AccountAmountRedirectName => db.AccountInfo.Find(Id).AccountAmountRedirect.Name;
        public int? AccountAmountRedirectId { get; set; }

        [Display(Name = "Düzeltme Tutarı")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalAmount => db.AccountInfo.Find(Id).TotalAmount ?? 0;

        [Display(Name = "Sıra No")]
        public int OrderNumber { get; set; }

        [Display(Name = "Arşiv")]
        public bool IsArchive { get; set; }

        [Display(Name = "Cihaz")]
        public string Device => db.AccountInfo.Find(Id).Device;

        [Display(Name = "Toplam Yatırım")]
        public decimal DepositSum => db.AccountTransactions.Where(x => x.Deposit == true && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Çekim")]
        public decimal DrawSum => db.AccountTransactions.Where(x => x.Deposit == false && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.DrawSplit.Where(a => a.IsDeleted == false).Count() == 0).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Split Çekim Sayısı")]
        public int DrawSplitCount => db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountInfoId == Id).Count();

        [Display(Name = "Toplam Split Çekim")]
        public decimal DrawSplitSum => db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountInfoId == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Cash Out")]
        public decimal CashOutSum => db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == false && x.AccountInfoId == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Cash In")]
        public decimal CashInSum => db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.AccountInfoId == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Komisyon")]
        public decimal BankCharge => (db.AccountTransactions.Where(x => x.Deposit == true && x.IsFree == false && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm).Select(x => x.Amount).DefaultIfEmpty(0).Sum() + db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.IsFree == false && x.AccountInfoId == Id).Select(x => x.Amount).DefaultIfEmpty(0).Sum()) * 3.4M / 100;

        [NotMapped]
        [Display(Name = "Bakiye")]
        public decimal Balance => DepositSum + CashInSum - CashOutSum - DrawSum - DrawSplitSum - BankCharge;

        [Display(Name = "Dolgu Rengi")]
        public string BgColor => db.AccountInfo.Find(Id).AccountInfoType.BgColor;

        [Display(Name = "Metin Rengi")]
        public string TextColor => db.AccountInfo.Find(Id).AccountInfoType.TextColor;

        IQueryable<AccountInfo> accountInfo => db.AccountInfo.Where(x => x.IsDeleted == false && x.IsArchive == false && x.IsPassive == false && x.AccountInfoType.TransactionLimit > TransactionCount && x.AccountInfoType.AmountLimit > Balance).OrderBy(x => x.OrderNumber).Take(1);

        [Display(Name = "İşlemde Olan Hesap")]
        public int CurrentAccountInfoId => accountInfo.Select(x => x.Id).DefaultIfEmpty(0).FirstOrDefault();

        public string LastUser
        {
            get
            {
                if (db.AccountInfo.Find(Id).UpdateUserId == null)
                {
                    if (db.Users.Any(x => x.Id == db.AccountInfo.Find(Id).AddUserId))
                    {
                        return db.Users.Find(db.AccountInfo.Find(Id).AddUserId).Name;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    if (db.Users.Any(x => x.Id == db.AccountInfo.Find(Id).UpdateUserId))
                    {
                        return db.Users.Find(db.AccountInfo.Find(Id).UpdateUserId).Name;
                    } else
                    {
                        return "";
                    }
                }
            }
        }

        //This month
        [Display(Name = "Toplam Yatırım Bu Ay")]
        public decimal DepositSumMonth => db.AccountTransactions.Where(x => x.Deposit == true && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.AddDate >= StartDateMonth && x.AddDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Cash In Bu Ay")]
        public decimal CashInSumMonth => db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.AccountInfoId == Id && x.TransactionDate >= StartDateMonth && x.TransactionDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Cash Out Bu Ay")]
        public decimal CashOutSumMonth => db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == false && x.AccountInfoId == Id && x.TransactionDate >= StartDateMonth && x.TransactionDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Çekim Bu Ay")]
        public decimal DrawSumMonth => db.AccountTransactions.Where(x => x.Deposit == false && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.AddDate >= StartDateMonth && x.AddDate <= EndDateMonth && x.DrawSplit.Where(a => a.IsDeleted == false).Count() == 0).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Toplam Split Çekim Bu Ay")]
        public decimal DrawSplitSumMonth => db.DrawSplit.Where(x => x.IsDeleted == false && x.AccountInfoId == Id && x.AccountTransactions.AddDate >= StartDateMonth && x.AccountTransactions.AddDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

        [Display(Name = "Komisyon Bu Ay")]
        public decimal BankChargeMonth => (db.AccountTransactions.Where(x => x.Deposit == true && x.IsFree == false && x.AccountInfoId == Id && x.TransactionStatus == TransactionStatus.Confirm && x.AddDate >= StartDateMonth && x.AddDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum() + db.CashFlow.Where(x => x.IsDeleted == false && x.IsCashIn == true && x.IsFree == false && x.AccountInfoId == Id && x.TransactionDate >= StartDateMonth && x.TransactionDate <= EndDateMonth).Select(x => x.Amount).DefaultIfEmpty(0).Sum()) * 3.4M / 100;

        [Display(Name = "Bakiye Bu Ay")]
        public decimal BalanceMonth => DepositSumMonth + CashInSumMonth - CashOutSumMonth - DrawSumMonth - DrawSplitSumMonth - BankChargeMonth;

        public decimal BalanceOrder { get; set; }
    }
}