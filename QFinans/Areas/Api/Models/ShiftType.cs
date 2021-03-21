using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class ShiftType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ICollection<ShiftTypeAccountInfo> ShiftTypeAccountInfo { get; set; }
    }
}