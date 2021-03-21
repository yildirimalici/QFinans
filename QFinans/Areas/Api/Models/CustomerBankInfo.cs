using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class CustomerBankInfo
    {
        public int Id { get; set; }

        [Display(Name = "Banka")]
        public int BankTypeId { get; set; }

        [Display(Name = "Aktif mi")]
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<AccountTransactions> AccountTransactions { get; set; }
        public virtual BankType BankType { get; set; }
    }
}