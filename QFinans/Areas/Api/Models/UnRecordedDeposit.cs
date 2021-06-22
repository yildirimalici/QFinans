using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class UnRecordedDeposit
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Name { get; set; }

        public string MiddleName { get; set; }

        public string SurName { get; set; }

        public decimal Amount { get; set; }

        public int? CustomerBankInfoId { get; set; }

        public string Reference { get; set; }

        public bool Papara { get; set; }

        public bool MoneyTransfer { get; set; }

        public DateTime AddDate { get; set; }

        public string AddUserId { get; set; }

        public int BrandId { get; set; }
    }
}