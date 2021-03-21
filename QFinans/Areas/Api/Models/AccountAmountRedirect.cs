using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class AccountAmountRedirect
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Display(Name = "Minimum Tutar")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal MinAmount { get; set; }

        [Display(Name = "Maksimum Tutar")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal MaxAmount { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<AccountInfo> AccountInfo { get; set; }
    }
}