using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class MoneyTransferDepositViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int CustomerBankInfoId { get; set; }

        public string Reference { get; set; }
    }
}