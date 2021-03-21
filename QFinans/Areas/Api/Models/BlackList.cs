using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public class BlackList
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public bool IsDeleted { get; set; }

        public string AddUserId { get; set; }

        public DateTime AddDate { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}