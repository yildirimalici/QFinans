using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class Paparax
    {
        public int Id { get; set; }

        [Display(Name = "Paparax Bakiye")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Balance { get; set; }

        [Display(Name = "Toplam Kasa")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Safe { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}