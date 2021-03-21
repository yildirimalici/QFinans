using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class CallbackUrl
    {
        public int Id { get; set; }

        public string Papara { get; set; }

        public string Coinbase { get; set; }

        public string MoneyTransfer { get; set; }
    }
}