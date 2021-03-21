using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class ShiftTypeAccountInfo
    {
        public int Id { get; set; }

        [Display(Name = "Shift")]
        public int ShiftTypeId { get; set; }

        [Display(Name = "Hesap")]
        public int AccountInfoId { get; set; }

        public bool IsClosed { get; set; }

        public string ClosedUserId { get; set; }

        [Display(Name = "Bakiye")]
        public decimal? Balance { get; set; }

        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ShiftType ShiftType { get; set; }

        public virtual AccountInfo AccountInfo { get; set; }
    }
}