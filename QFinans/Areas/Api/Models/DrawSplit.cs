using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class DrawSplit
    {
        public int Id { get; set; }

        [Display(Name = "İşlem")]
        public int AccountTransactionsId { get; set; }

        [Display(Name = "Hesap")]
        public int AccountInfoId { get; set; }

        [Display(Name = "Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public virtual AccountTransactions AccountTransactions { get; set; }
        public virtual AccountInfo AccountInfo { get; set; }
    }
}