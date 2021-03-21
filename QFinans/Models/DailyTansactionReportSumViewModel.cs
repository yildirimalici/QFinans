using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class DailyTansactionReportSumViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int NewDepositCount { get; set; }
        public decimal NewDepositSum { get; set; }
        public int ConfirmDepositCount { get; set; }
        public decimal ConfirmDepositSum { get; set; }
        public int DenyDepositCount { get; set; }
        public decimal DenyDepositSum { get; set; }
        public int NewDrawCount { get; set; }
        public decimal NewDrawSum { get; set; }
        public int ConfirmDrawCount { get; set; }
        public decimal ConfirmDrawSum { get; set; }
        public int DenyDrawCount { get; set; }
        public decimal DenyDrawSum { get; set; }
    }
}