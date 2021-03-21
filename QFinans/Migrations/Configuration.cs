namespace QFinans.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<QFinans.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "QFinans.Models.ApplicationDbContext";
        }

        protected override void Seed(QFinans.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //User
            if (!roleManager.RoleExists("RegisterAccount"))
                roleManager.Create(new IdentityRole("RegisterAccount"));

            //AppUserController

            if (!roleManager.RoleExists("IndexAppUser"))
                roleManager.Create(new IdentityRole("IndexAppUser"));

            if (!roleManager.RoleExists("EditAppUser"))
                roleManager.Create(new IdentityRole("EditAppUser"));

            if (!roleManager.RoleExists("DeleteAppUser"))
                roleManager.Create(new IdentityRole("DeleteAppUser"));

            //AccountAmountRedirectController
            if (!roleManager.RoleExists("IndexAccountAmountRedirect"))
                roleManager.Create(new IdentityRole("IndexAccountAmountRedirect"));

            if (!roleManager.RoleExists("DetailsAccountAmountRedirect"))
                roleManager.Create(new IdentityRole("DetailsAccountAmountRedirect"));

            if (!roleManager.RoleExists("CreateAccountAmountRedirect"))
                roleManager.Create(new IdentityRole("CreateAccountAmountRedirect"));

            if (!roleManager.RoleExists("EditAccountAmountRedirect"))
                roleManager.Create(new IdentityRole("EditAccountAmountRedirect"));

            if (!roleManager.RoleExists("DeleteAccountAmountRedirect"))
                roleManager.Create(new IdentityRole("DeleteAccountAmountRedirect"));

            //AccountInfoController
            if (!roleManager.RoleExists("IndexAccountInfo"))
                roleManager.Create(new IdentityRole("IndexAccountInfo"));

            if (!roleManager.RoleExists("ArchiveAccountAccountInfo"))
                roleManager.Create(new IdentityRole("ArchiveAccountAccountInfo"));

            if (!roleManager.RoleExists("DetailsAccountInfo"))
                roleManager.Create(new IdentityRole("DetailsAccountInfo"));

            if (!roleManager.RoleExists("CreateAccountInfo"))
                roleManager.Create(new IdentityRole("CreateAccountInfo"));

            if (!roleManager.RoleExists("EditAccountInfo"))
                roleManager.Create(new IdentityRole("EditAccountInfo"));

            if (!roleManager.RoleExists("TotalAmountAccountInfo"))
                roleManager.Create(new IdentityRole("TotalAmountAccountInfo"));

            if (!roleManager.RoleExists("EditTotalAmountAccountInfo"))
                roleManager.Create(new IdentityRole("EditTotalAmountAccountInfo"));

            if (!roleManager.RoleExists("DeleteAccountInfo"))
                roleManager.Create(new IdentityRole("DeleteAccountInfo"));

            if (!roleManager.RoleExists("SendArchieveAccountInfo"))
                roleManager.Create(new IdentityRole("SendArchieveAccountInfo"));

            if (!roleManager.RoleExists("SetPassiveAccountInfo"))
                roleManager.Create(new IdentityRole("SetPassiveAccountInfo"));

            if (!roleManager.RoleExists("SetActiveAccountInfo"))
                roleManager.Create(new IdentityRole("SetActiveAccountInfo"));

            //AccountInfoStatusController
            if (!roleManager.RoleExists("IndexAccountInfoStatus"))
                roleManager.Create(new IdentityRole("IndexAccountInfoStatus"));

            if (!roleManager.RoleExists("DetailsAccountInfoStatus"))
                roleManager.Create(new IdentityRole("DetailsAccountInfoStatus"));

            if (!roleManager.RoleExists("CreateAccountInfoStatus"))
                roleManager.Create(new IdentityRole("CreateAccountInfoStatus"));

            if (!roleManager.RoleExists("EditAccountInfoStatus"))
                roleManager.Create(new IdentityRole("EditAccountInfoStatus"));

            if (!roleManager.RoleExists("DeleteAccountInfoStatus"))
                roleManager.Create(new IdentityRole("DeleteAccountInfoStatus"));

            //AccountInfoTypeController
            if (!roleManager.RoleExists("IndexAccountInfoType"))
                roleManager.Create(new IdentityRole("IndexAccountInfoType"));

            if (!roleManager.RoleExists("DetailsAccountInfoType"))
                roleManager.Create(new IdentityRole("DetailsAccountInfoType"));

            if (!roleManager.RoleExists("CreateAccountInfoType"))
                roleManager.Create(new IdentityRole("CreateAccountInfoType"));

            if (!roleManager.RoleExists("EditAccountInfoType"))
                roleManager.Create(new IdentityRole("EditAccountInfoType"));

            if (!roleManager.RoleExists("DeleteAccountInfoType"))
                roleManager.Create(new IdentityRole("DeleteAccountInfoType"));

            //AccountTransactionsController
            if (!roleManager.RoleExists("DepositAccountTransactions"))
                roleManager.Create(new IdentityRole("DepositAccountTransactions"));

            if (!roleManager.RoleExists("DrawAccountTransactions"))
                roleManager.Create(new IdentityRole("DrawAccountTransactions"));

            if (!roleManager.RoleExists("DetailsAccountTransactions"))
                roleManager.Create(new IdentityRole("DetailsAccountTransactions"));

            if (!roleManager.RoleExists("CreateDepositAccountTransactions"))
                roleManager.Create(new IdentityRole("CreateDepositAccountTransactions"));

            if (!roleManager.RoleExists("CreateDrawAccountTransactions"))
                roleManager.Create(new IdentityRole("CreateDrawAccountTransactions"));

            if (!roleManager.RoleExists("EditAccountTransactions"))
                roleManager.Create(new IdentityRole("EditAccountTransactions"));

            if (!roleManager.RoleExists("EditCoinAccountTransactions"))
                roleManager.Create(new IdentityRole("EditCoinAccountTransactions"));

            if (!roleManager.RoleExists("ConfirmDepositAccountTransactions"))
                roleManager.Create(new IdentityRole("ConfirmDepositAccountTransactions"));

            if (!roleManager.RoleExists("ConfirmDrawAccountTransactions"))
                roleManager.Create(new IdentityRole("ConfirmDrawAccountTransactions"));

            if (!roleManager.RoleExists("DenyAccountTransactions"))
                roleManager.Create(new IdentityRole("DenyAccountTransactions"));

            if (!roleManager.RoleExists("CallBackApiAccountTransactions"))
                roleManager.Create(new IdentityRole("CallBackApiAccountTransactions"));

            //BlackListController
            if (!roleManager.RoleExists("IndexBlackList"))
                roleManager.Create(new IdentityRole("IndexBlackList"));

            if (!roleManager.RoleExists("CreateBlackList"))
                roleManager.Create(new IdentityRole("CreateBlackList"));

            if (!roleManager.RoleExists("EditBlackList"))
                roleManager.Create(new IdentityRole("EditBlackList"));

            if (!roleManager.RoleExists("DeleteBlackList"))
                roleManager.Create(new IdentityRole("DeleteBlackList"));

            //CashFlowTypeController
            if (!roleManager.RoleExists("IndexCashFlowType"))
                roleManager.Create(new IdentityRole("IndexCashFlowType"));

            if (!roleManager.RoleExists("DetailsCashFlowType"))
                roleManager.Create(new IdentityRole("DetailsCashFlowType"));

            if (!roleManager.RoleExists("CreateCashFlowType"))
                roleManager.Create(new IdentityRole("CreateCashFlowType"));

            if (!roleManager.RoleExists("EditCashFlowType"))
                roleManager.Create(new IdentityRole("EditCashFlowType"));

            if (!roleManager.RoleExists("DeleteCashFlowType"))
                roleManager.Create(new IdentityRole("DeleteCashFlowType"));

            //CashFlowController
            if (!roleManager.RoleExists("IndexCashFlow"))
                roleManager.Create(new IdentityRole("IndexCashFlow"));

            if (!roleManager.RoleExists("DetailsCashFlow"))
                roleManager.Create(new IdentityRole("DetailsCashFlow"));

            if (!roleManager.RoleExists("CreateCashFlow"))
                roleManager.Create(new IdentityRole("CreateCashFlow"));

            if (!roleManager.RoleExists("EditCashFlow"))
                roleManager.Create(new IdentityRole("EditCashFlow"));

            if (!roleManager.RoleExists("DeleteCashFlow"))
                roleManager.Create(new IdentityRole("DeleteCashFlow"));

            //DrawSplitController
            if (!roleManager.RoleExists("IndexDrawSplit"))
                roleManager.Create(new IdentityRole("IndexDrawSplit"));

            if (!roleManager.RoleExists("CreateDrawSplit"))
                roleManager.Create(new IdentityRole("CreateDrawSplit"));

            if (!roleManager.RoleExists("EditDrawSplit"))
                roleManager.Create(new IdentityRole("EditDrawSplit"));

            if (!roleManager.RoleExists("DeleteDrawSplit"))
                roleManager.Create(new IdentityRole("DeleteDrawSplit"));

            //ReportController
            if (!roleManager.RoleExists("IndexReport"))
                roleManager.Create(new IdentityRole("IndexReport"));

            if (!roleManager.RoleExists("MonthlyReport"))
                roleManager.Create(new IdentityRole("MonthlyReport"));

            if (!roleManager.RoleExists("BalanceReport"))
                roleManager.Create(new IdentityRole("BalanceReport"));

            if (!roleManager.RoleExists("CashFlowReport"))
                roleManager.Create(new IdentityRole("CashFlowReport"));

            //ShiftTypeController
            if (!roleManager.RoleExists("IndexShiftType"))
                roleManager.Create(new IdentityRole("IndexShiftType"));

            if (!roleManager.RoleExists("DetailsShiftType"))
                roleManager.Create(new IdentityRole("DetailsShiftType"));

            if (!roleManager.RoleExists("CreateShiftType"))
                roleManager.Create(new IdentityRole("CreateShiftType"));

            if (!roleManager.RoleExists("EditShiftType"))
                roleManager.Create(new IdentityRole("EditShiftType"));

            if (!roleManager.RoleExists("DeleteShiftType"))
                roleManager.Create(new IdentityRole("DeleteShiftType"));

            if (!roleManager.RoleExists("AddAccountInfoShiftType"))
                roleManager.Create(new IdentityRole("AddAccountInfoShiftType"));

            if (!roleManager.RoleExists("EditAccountInfoShiftType"))
                roleManager.Create(new IdentityRole("EditAccountInfoShiftType"));

            if (!roleManager.RoleExists("CloseAccountInfoShiftType"))
                roleManager.Create(new IdentityRole("CloseAccountInfoShiftType"));

            if (!roleManager.RoleExists("DeleteAccountInfoShiftType"))
                roleManager.Create(new IdentityRole("DeleteAccountInfoShiftType"));

            //SimLocationController
            if (!roleManager.RoleExists("IndexSimLocation"))
                roleManager.Create(new IdentityRole("IndexSimLocation"));

            if (!roleManager.RoleExists("DetailsSimLocation"))
                roleManager.Create(new IdentityRole("DetailsSimLocation"));

            if (!roleManager.RoleExists("CreateSimLocation"))
                roleManager.Create(new IdentityRole("CreateSimLocation"));

            if (!roleManager.RoleExists("EditSimLocation"))
                roleManager.Create(new IdentityRole("EditSimLocation"));

            if (!roleManager.RoleExists("DeleteSimLocation"))
                roleManager.Create(new IdentityRole("DeleteSimLocation"));

            //SystemParametersController
            if (!roleManager.RoleExists("SystemParameters"))
                roleManager.Create(new IdentityRole("SystemParameters"));

            //Coinbase
            if (!roleManager.RoleExists("IndexCoinbases"))
                roleManager.Create(new IdentityRole("IndexCoinbases"));

            if (!roleManager.RoleExists("DetailsCoinbases"))
                roleManager.Create(new IdentityRole("DetailsCoinbases"));

            //BankTypeController
            if (!roleManager.RoleExists("IndexBankType"))
                roleManager.Create(new IdentityRole("IndexBankType"));

            if (!roleManager.RoleExists("DetailsBankType"))
                roleManager.Create(new IdentityRole("DetailsBankType"));

            if (!roleManager.RoleExists("CreateBankType"))
                roleManager.Create(new IdentityRole("CreateBankType"));

            if (!roleManager.RoleExists("EditBankType"))
                roleManager.Create(new IdentityRole("EditBankType"));

            if (!roleManager.RoleExists("DeleteBankType"))
                roleManager.Create(new IdentityRole("DeleteBankType"));

            //BankInfoController
            if (!roleManager.RoleExists("IndexBankInfo"))
                roleManager.Create(new IdentityRole("IndexBankInfo"));

            if (!roleManager.RoleExists("ArchiveBankInfo"))
                roleManager.Create(new IdentityRole("ArchiveBankInfo"));

            if (!roleManager.RoleExists("DetailsBankInfo"))
                roleManager.Create(new IdentityRole("DetailsBankInfo"));

            if (!roleManager.RoleExists("CreateBankInfo"))
                roleManager.Create(new IdentityRole("CreateBankInfo"));

            if (!roleManager.RoleExists("EditBankInfo"))
                roleManager.Create(new IdentityRole("EditBankInfo"));

            if (!roleManager.RoleExists("DeleteBankInfo"))
                roleManager.Create(new IdentityRole("DeleteBankInfo"));

            //CustomerBankInfoController
            if (!roleManager.RoleExists("IndexCustomerBankInfo"))
                roleManager.Create(new IdentityRole("IndexCustomerBankInfo"));

            if (!roleManager.RoleExists("DetailsCustomerBankInfo"))
                roleManager.Create(new IdentityRole("DetailsCustomerBankInfo"));

            if (!roleManager.RoleExists("CreateCustomerBankInfo"))
                roleManager.Create(new IdentityRole("CreateCustomerBankInfo"));

            if (!roleManager.RoleExists("EditCustomerBankInfo"))
                roleManager.Create(new IdentityRole("EditCustomerBankInfo"));

            if (!roleManager.RoleExists("DeleteCustomerBankInfo"))
                roleManager.Create(new IdentityRole("DeleteCustomerBankInfo"));
        }
    }
}
