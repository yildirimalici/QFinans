using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class TestCallBackApiViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public Int64 AccountNumber { get; set; }
        public string Note { get; set; }
    }
}