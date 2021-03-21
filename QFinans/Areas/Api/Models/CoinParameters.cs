using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class CoinParameters
    {
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string ContentType { get; set; }
        public string Version { get; set; }
        public decimal MinAmount { get; set; }
        public int WaitingTime { get; set; }
    }
}