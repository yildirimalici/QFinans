using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class TotalAmountViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public Int64 AccountNumber { get; set; }

        [Display(Name = "İşlem Sayısı")]
        public int TransactionCount { get; set; }

        [Display(Name = "Düzeltme Tutarı")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Son Bakiye")]
        public decimal LastAmount { get; set; }

        [Display(Name = "Durum")]
        public bool IsPassive { get; set; }

        [Display(Name = "Hesap Türü")]
        public string AccountInfoTypeName { get; set; }

        [Display(Name = "Cihaz")]
        public string Device { get; set; }
    }
}