using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class BalanceViewModel
    {
        public int NewDepositCount { get; set; }
        public int ConfirmDepositCount { get; set; }
        public int DenyDepositCount { get; set; }
        public decimal NewDepositSum { get; set; }
        public decimal ConfirmDepositSum { get; set; }
        public decimal DenyDepositSum { get; set; }

        public int NewDrawCount { get; set; }
        public int ConfirmDrawCount { get; set; }
        public int DenyDrawCount { get; set; }
        public decimal NewDrawSum { get; set; }
        public decimal ConfirmDrawSum { get; set; }
        public decimal DenyDrawSum { get; set; }

        public decimal? InitialBalance { get; set; }
        public decimal? BalanceEditAmount { get; set; }
        public decimal? BankCharge { get; set; }

        public decimal CashInSum { get; set; }
        public decimal CashOutSum { get; set; }
        public decimal DepositFreeSum { get; set; }
        public decimal CashInFreeSum { get; set; }

        public decimal Safe { get; set; }
    }
}