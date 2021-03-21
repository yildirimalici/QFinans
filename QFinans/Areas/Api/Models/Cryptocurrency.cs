using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class Cryptocurrency
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UnitSymbol { get; set; }

        public decimal MinAmount { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}