using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class BankInfo
    {
        public int Id { get; set; }

        [Display(Name = "Banka")]
        public int BankTypeId { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        public string Surname { get; set; }

        [Display(Name = "Şube Kodu")]
        public string BranchCode { get; set; }

        [Display(Name = "Hesap Numarası")]
        public string AccountNumber { get; set; }

        public string Iban { get; set; }

        [Display(Name = "Pasif mi")]
        public bool IsPassive { get; set; }

        [Display(Name = "Arşiv mi")]
        public bool IsArchive { get; set; }

        [Display(Name = "Sıra No")]
        public int OrderNumber { get; set; }

        [Display(Name = "Minumum Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MinAmount { get; set; }

        [Display(Name = "Yatırım İşlem Sayısı")]
        public int? TransactionCountDeposit { get; set; }

        [Display(Name = "Çekim İşlem Sayısı")]
        public int? TransactionCountDraw { get; set; }

        [Display(Name = "Bakiye")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? Balance { get; set; }

        [Display(Name = "Komisyon")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? BankCharge { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<MoneyTransfer> MoneyTransfer { get; set; }
        public virtual BankType BankType { get; set; }
    }
}