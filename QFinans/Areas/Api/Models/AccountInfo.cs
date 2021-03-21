using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class AccountInfo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyisim")]
        public string SurName { get; set; }

        [Display(Name = "Hesap No")]
        public Int64 AccountNumber { get; set; }

        [Display(Name = "Durum")]
        public bool IsPassive { get; set; }

        [Display(Name = "Hesap Türü")]
        public int AccountInfoTypeId { get; set; }

        [Display(Name = "Tutar Yönlendirme Türü")]
        public int? AccountAmountRedirectId { get; set; }

        [Display(Name = "Düzeltme Tutarı")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "Sıra No")]
        public int OrderNumber { get; set; }

        [Display(Name = "Arşiv")]
        public bool IsArchive { get; set; }

        [Display(Name = "Cihaz")]
        public string Device { get; set; }

        [Display(Name = "Açılış Tarihi")]
        public DateTime? OpeningDate { get; set; }

        [Display(Name = "Sim Lokasyonu")]
        public int? SimLocationId { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Hesap Durumu")]
        public int? AccountInfoStatusId { get; set; }

        [Display(Name = "Açıklama")]
        public string Explanation { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }

        //insert data from database
        [Display(Name = "Yatırım İşlem Sayısı")]
        public int? TransactionCountDeposit { get; set; }

        [Display(Name = "Bu Ay Yatırım Yapan Farklı Kullanıcı Sayısı")]
        public int? DifferentUsersTransactionCountDeposit { get; set; }

        [Display(Name = "Çekim İşlem Sayısı")]
        public int? TransactionCountDraw { get; set; }

        [Display(Name = "Onaylı Yatırım Tutarı")]
        public decimal? ConfirmDepositSum { get; set; }

        [Display(Name = "Onaylı Ücretsiz Yatırım Tutarı")]
        public decimal? ConfirmDepositSumNotFree { get; set; }

        [Display(Name = "Bu Ayki Onaylı Yatırım Tutarı")]
        public decimal? ConfirmDepositSumCurrentMonth { get; set; }

        [Display(Name = "Bu Ayki Onaylı Ücretsiz Yatırım Tutarı")]
        public decimal? ConfirmDepositSumCurrentMonthNotFree { get; set; }

        [Display(Name = "Onaylı Çekim Tutarı")]
        public decimal? ConfirmDrawSum { get; set; }

        [Display(Name = "Bu Ayki Onaylı Çekim Tutarı")]
        public decimal? ConfirmDrawSumCurrentMonth { get; set; }

        [Display(Name = "Bölünmüş Onaylı Çekim Tutarı")]
        public decimal? DrawSplitSum { get; set; }

        [Display(Name = "Bu Ayki Bölünmüş Onaylı Çekim Tutarı")]
        public decimal? DrawSplitSumCurrentMonth { get; set; }

        [Display(Name = "Cash In Tutarı")]
        public decimal? CashInSum { get; set; }

        [Display(Name = "Ücretsiz Cash In Tutarı")]
        public decimal? CashInSumNotFree { get; set; }

        [Display(Name = "Bu Ayki Cash In Tutarı")]
        public decimal? CashInSumCurrentMonth { get; set; }

        [Display(Name = "Bu Ayki  Ücretsiz Cash In Tutarı")]
        public decimal? CashInSumCurrentMonthNotFree { get; set; }

        [Display(Name = "Cash Out Tutarı")]
        public decimal? CashOutSum { get; set; }

        [Display(Name = "Bu Ayki  Ücretsiz Cash Out Tutarı")]
        public decimal? CashOutSumCurrentMonth { get; set; }

        [Display(Name = "Bakiye")]
        public decimal? Balance { get; set; }

        [Display(Name = "Bu Ayki Bakiye")]
        public decimal? BalanceCurrentMonth { get; set; }

        [Display(Name = "Komisyon")]
        public decimal? BankCharge { get; set; }

        [Display(Name = "Bu Ayki Komisyon")]
        public decimal? BankChargeCurrentMonth { get; set; }

        [Display(Name = "Son İşlem Yapan Kullanıcı")]
        public string LastUser { get; set; }

        public virtual AccountInfoType AccountInfoType { get; set; }
        public virtual AccountAmountRedirect AccountAmountRedirect { get; set; }
        public ICollection<AccountTransactions> AccountTransactions { get; set; }
        public ICollection<CashFlow> CashFlow { get; set; }
        public ICollection<DrawSplit> DrawSplit { get; set; }
        public virtual SimLocation SimLocation { get; set; }
        public ICollection<ShiftTypeAccountInfo> ShiftTypeAccountInfo { get; set; }
        public virtual AccountInfoStatus AccountInfoStatus { get; set; }
    }
}