using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class ActiveAccountInfoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        public string SurName { get; set; }

        [Display(Name = "Hesap No")]
        public Int64 AccountNumber { get; set; }

        [Display(Name = "Durum")]
        public bool IsPassive { get; set; }

        [Display(Name = "Hesap Türü")]
        public string AccountInfoTypeName { get; set; }

        [Display(Name = "İşlem Limiti")]
        public int AccountInfoTypeTransactionLimit { get; set; }

        [Display(Name = "Tutar Limiti")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal AccountInfoTypeAmountLimit { get; set; }

        [Display(Name = "İşlem Sayısı")]
        public int TransactionCount { get; set; }

        [Display(Name = "Toplam Onaylı Yatırım")]
        public decimal ConfirmDepositSum { get; set; }

        [Display(Name = "Toplam Yeni Yatırım")]
        public decimal NewDepositSum { get; set; }

        [Display(Name = "Toplam Red Yatırım")]
        public decimal DenyDepositSum { get; set; }

        [Display(Name = "Toplam Onaylı Çekim")]
        public decimal ConfirmDrawSum { get; set; }

        [Display(Name = "Toplam Yeni Çekim")]
        public decimal NewDrawSum { get; set; }

        [Display(Name = "Toplam Red Çekim")]
        public decimal DenyDrawSum { get; set; }

        [Display(Name = "Sıra No")]
        public int OrderNumber { get; set; }
    }
}