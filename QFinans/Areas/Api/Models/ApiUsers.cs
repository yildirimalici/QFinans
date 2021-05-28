using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class ApiUsers
    {
        [Key]
        public Guid Key { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }

        public string WhiteIsTip { get; set; }

        public string Job { get; set; }

        public string Organization { get; set; }

        public bool Papara { get; set; }

        public bool Coinbase { get; set; }

        public bool MoneyTransfer { get; set; }
    }
}