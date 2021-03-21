using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace QFinans.Models
{
    public class DailyTansactionReportViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public int Id { get; set; }

        //public int Year => db.AccountTransactions.Find(Id).AddDate.Year;

        //public int Month => db.AccountTransactions.Find(Id).AddDate.Month;

        //public int Day => db.AccountTransactions.Find(Id).AddDate.Day;

        //public DateTime Date => new DateTime(Year, Month, Day);
        //public int NewDepositCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.New)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal NewDepositSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.New)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public int ConfirmDepositCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal ConfirmDepositSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public int DenyDepositCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Deny)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal DenyDepositSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == true && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Deny)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public int NewDrawCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.New)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal NewDrawSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.New)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public int ConfirmDrawCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal ConfirmDrawSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Confirm)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public int DenyDrawCount
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Deny)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public decimal DenyDrawSum
        //{
        //    get
        //    {
        //        if (db.AccountTransactions.Find(Id).Deposit == false && db.AccountTransactions.Find(Id).TransactionStatus == Areas.Api.Models.TransactionStatus.Deny)
        //        {
        //            return db.AccountTransactions.Find(Id).Amount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}

        public int Id { get; set; }
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