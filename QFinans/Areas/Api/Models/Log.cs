using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFinans.Areas.Api.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public string UrlAccessed { get; set; }

        [AllowHtml]
        public string Data { get; set; }
        public long ExecutionMs { get; set; }
        public DateTime AddDate { get; set; }
    }
}