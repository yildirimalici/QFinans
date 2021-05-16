using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class DrawViewModelCoin
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        //public string MiddleName { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Reference { get; set; }

        [Required]
        public string UnitSymbol { get; set; }

        [Required]
        public string CustomerWalletAddress { get; set; }

        public string CustomerDestinationTag { get; set; }
    }
}