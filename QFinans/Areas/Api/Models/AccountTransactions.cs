using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class AccountTransactions
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyisim")]
        public string SurName { get; set; }

        [Display(Name = "Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [Display(Name = "Önerilen Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? OldAmount { get; set; }

        [Display(Name = "Komisyon")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? BankCharge { get; set; }

        [Display(Name = "Papara Hesap")]
        public int? AccountInfoId { get; set; }

        [Display(Name = "Durum")]
        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus? TransactionStatus { get; set; }

        public bool Deposit { get; set; }

        [Display(Name = "Müşteri Hesap No")]
        public Int64? CustomerAccountNumber { get; set; }

        [Display(Name = "Açıklama")]
        public string Note { get; set; }

        [Display(Name = "Ücretsiz mi?")]
        public bool IsFree { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ResponseType { get; set; }

        public string ResponseMessage { get; set; }

        public string Location { get; set; }

        public DateTime? NotificationDate { get; set; }

        public bool FirstDifferentTransaction { get; set; }

        public string Reference { get; set; }

        public bool IsCoin { get; set; }

        public string ReferenceCoin { get; set; }

        public DateTime? ResponseDateCoin { get; set; }

        public string UrlCoin { get; set; }

        public string UrlCoinApi { get; set; }

        public string ResponseMessageCoin { get; set; }

        public string UnitSymbol { get; set; }

        public string CoinConvertTimestamp { get; set; }

        public decimal? ExchangeUnitValue { get; set; }

        public double? ExchangeValue { get; set; }

        public string CustomerWalletAddress { get; set; }

        public string DestinationTag { get; set; }

        public string PaymentLocalCurrency { get; set; }

        public double? PaymentLocalAmount { get; set; }

        public string PaymentCriytoCurrency { get; set; }

        public double? PaymentCriytoAmount { get; set; }

        public bool IsMoneyTransfer { get; set; }

        [Display(Name = "Bank")]
        public int? BankInfoId { get; set; }

        [Display(Name = "Müsterinin Bankası")]
        public int? CustomerBankInfoId { get; set; }

        [Display(Name = "Müşrteri Iban")]
        public string CustomerIban { get; set; }

        public bool IsBotCheck { get; set; }

        public virtual AccountInfo AccountInfo { get; set; }

        public ICollection<DrawSplit> DrawSplit { get; set; }

        public virtual BankInfo BankInfo { get; set; }
        //public virtual MoneyTransferType MoneyTransferType { get; set; }
        public virtual CustomerBankInfo CustomerBankInfo { get; set; }
    }
}