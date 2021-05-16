using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class CallbackUrl
    {
        public int Id { get; set; }

        public string PaparaDeposit { get; set; }

        public string PaparaDraw { get; set; }

        public string Coinbase { get; set; }

        public string MoneyTransfer { get; set; }
    }
}