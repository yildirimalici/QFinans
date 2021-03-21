using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class BankType
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<BankInfo> BankInfo { get; set; }
        public ICollection<CustomerBankInfo> CustomerBankInfo { get; set; }
    }
}