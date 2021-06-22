using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class MoneyTransfer
    {
        public int Id { get; set; }

        [Display(Name = "Kullanıcı")]
        public string UserName { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "İkinci İsim")]
        public string MiddleName { get; set; }

        [Display(Name = "Soyisim")]
        public string SurName { get; set; }

        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        public bool Deposit { get; set; }

        [Display(Name = "Hesap")]
        public int? BankInfoId { get; set; }

        [Display(Name = "Müşteri Bankası")]
        public int? CustomerBankInfoId { get; set; }

        [Display(Name = "Müşteri Iban")]
        public string CustomerIban { get; set; }

        [Display(Name = "Önerilen Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? OldAmount { get; set; }

        [Display(Name = "Status")]
        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus? TransactionStatus { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ResponseType { get; set; }

        public string ResponseMessage { get; set; }

        public string Location { get; set; }

        public DateTime? NotificationDate { get; set; }

        public string Reference { get; set; }

        public int BrandId { get; set; }

        public virtual BankInfo BankInfo { get; set; }
        //public virtual MoneyTransferType MoneyTransferType { get; set; }
        public virtual CustomerBankInfo CustomerBankInfo { get; set; }
    }
}