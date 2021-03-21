using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class AccountInfoType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "İşlem Limiti")]
        public int TransactionLimit { get; set; }

        [Required]
        [Display(Name = "Tutar Limiti")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal AmountLimit { get; set; }

        [Display(Name = "Nakit Yatırma Limiti")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal CashInAmountLimit { get; set; }

        [Display(Name = "Dolgu Rengi")]
        public string BgColor { get; set; }

        [Display(Name = "Metin Rengi")]
        public string TextColor { get; set; }

        //[Phone]
        //[Display(Name = "Telefon")]
        //public string PhoneNumber { get; set; }

        //public string SimLocation { get; set; }

        //public string PaparaLocation { get; set; }

        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<AccountInfo> AccountInfo { get; set; }
    }
}