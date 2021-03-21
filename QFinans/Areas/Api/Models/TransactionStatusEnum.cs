using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFinans.Areas.Api.Models
{
    public enum TransactionStatus
    {
        New = 1,
        Confirm = 2,
        Deny = 3
    }
}