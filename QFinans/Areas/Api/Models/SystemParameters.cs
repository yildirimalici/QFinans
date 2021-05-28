using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class SystemParameters
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Komisyon İşlem Sayısı Limiti")]
        public int BankChargeTransactionLimit { get; set; }
        [Required]
        [Display(Name = "Komisyon İşlem Tutarı Limiti")]
        public decimal BankChargeAmountLimit { get; set; }

        [Display(Name = "Anlık Bakiye Düzenleme Tutarı")]
        public decimal? BalanceEditAmount { get; set; }

        [Display(Name = "Anlık Bakiye Başlangıç Tarihi")]
        public DateTime? BalanceStartDate { get; set; }

        [Display(Name = "Anlık Başlangıç Bakiyesi")]
        public decimal? InitialBalance { get; set; }

        [Display(Name = "Anlık Bakiye Komisyon Oranı")]
        public decimal? BankCharge { get; set; }

        [Display(Name = "İlk İşlem Tutar Limiti")]
        public decimal? InitialAmountLimit { get; set; }

        [Display(Name = "Yatırım Sayfası Gösterilecek Kayıt Sayısı")]
        public int? DepositPageSize { get; set; }

        [Display(Name = "Çekim Sayfası Gösterilecek Kayıt Sayısı")]
        public int? DrawPageSize { get; set; }

        [Display(Name = "Hesap Bilgileri Sayfası Gösterilecek Kayıt Sayısı")]
        public int? AccountInfoPageSize { get; set; }

        [Display(Name = "Ücretsiz İlk İşlem Sayısı")]
        public int? FreeTransactionNumber { get; set; }

        [Display(Name = "Acil Durum Hesap Id'si")]
        public int? EmergencyAccountId { get; set; }

        [Display(Name = "Ücretsiz İlk Farklı Kullanıcı Sayısı")]
        public int? FreeFirstDifferentUserNumber { get; set; }

        [Display(Name = "EFT Başlangıç Saati")]
        public TimeSpan EftStartTime { get; set; }

        [Display(Name = "EFT Bitiş Saati")]
        public TimeSpan EftEndTime { get; set; }

        [Display(Name = "Min Fast Limit")]
        public decimal MinFastLimit { get; set; }

        [Display(Name = "Max Fast Limit")]
        public decimal MaxFastLimit { get; set; }
    }
}