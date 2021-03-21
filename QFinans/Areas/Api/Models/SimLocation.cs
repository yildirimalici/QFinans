using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class SimLocation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Display(Name = "Dolgu Rengi")]
        public string BgColor { get; set; }

        [Display(Name = "Metin Rengi")]
        public string TextColor { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<AccountInfo> AccountInfo { get; set; }
    }
}