using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class CashFlow
    {
        public int Id { get; set; }

        [Display(Name = "Hesap")]
        public int AccountInfoId { get; set; }

        [Display(Name = "İşlem Türü")]
        public int? CashFlowTypeId { get; set; }

        [Display(Name = "İşlem Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [Display(Name = "Hareket Türü")]
        public bool IsCashIn { get; set; }

        [Display(Name = "Açıklama")]
        public string Explanation { get; set; }

        [Display(Name = "İşlem Durumu")]
        public bool IsFree { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Hesaplar arası virman")]
        public bool IsTransfer { get; set; }

        public virtual AccountInfo AccountInfo { get; set; }
        public virtual CashFlowType CashFlowType { get; set; }
    }
}