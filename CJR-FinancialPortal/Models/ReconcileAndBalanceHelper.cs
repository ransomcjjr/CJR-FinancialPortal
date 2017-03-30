using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class ReconcileAndBalanceHelper
    {
        ApplicationDbContext dbCalc = new ApplicationDbContext();

        //Get Total Income Budget
        public double GetBudgetIncomeTotal(int hid)
        {
            double IncomeTotal = 0;
            int countTotal = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == true).Count();

            if (countTotal > 0)
            {
                IncomeTotal = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == true).Sum(h => h.BudgetAmount);
            }
                return IncomeTotal;
        }

        //Get Total Expense Budget
        public double GetBudgetExpenseTotal(int hid)
        {
            double Expensetotal = 0;
            int countExpense = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == false).Count();

            if (countExpense > 0)
            {
                Expensetotal = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == false).Sum(h => h.BudgetAmount);
            }
                return Expensetotal;
        }

        //Get Budgeted Amount for a Budget Item Category
        public double GetBudgetCategoryTotal(int bid)
        {
            Budget budgetItem = dbCalc.Budgets.FirstOrDefault(b => b.Id == bid);
            double CatTotal = 0;

            if (budgetItem != null)
            {
                CatTotal = budgetItem.BudgetAmount;

            }
            return CatTotal;
        }

        //Get Total Monthly Amount of Income Transactions
        public double GetMonthlyIncomeTransactionTotal(int hid,DateTime dateChosen)
        {
            double IncomeTotal = 0;
            int countIncome = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == true).SelectMany(t => t.Transactions.Where(ta => ta.Archive == false && ta.DateAdded.Month == dateChosen.Month && ta.DateAdded.Year == dateChosen.Year)).Count();

            if (countIncome > 0)
            {
                IncomeTotal = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == true).SelectMany(t => t.Transactions.Where(ta => ta.Archive == false && ta.DateAdded.Month == dateChosen.Month && ta.DateAdded.Year == dateChosen.Year)).Sum(h => h.Amount);
            }
                return IncomeTotal;
        }

        //Get Total Monthly Amount of Expense Transactions 
        public double GetMonthlyExpenseTransactionTotal(int hid, DateTime dateChosen)
        {
            double Expensetotal = 0;
            int countExpense = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == false).SelectMany(t => t.Transactions.Where(ta => ta.Archive == false && ta.DateAdded.Month == dateChosen.Month && ta.DateAdded.Year == dateChosen.Year)).Count();

            if (countExpense > 0)
            {
                Expensetotal = dbCalc.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false && h.BudgetCategory.IncomeExpense == false).SelectMany(t => t.Transactions.Where(ta => ta.Archive == false && ta.DateAdded.Month == dateChosen.Month && ta.DateAdded.Year == dateChosen.Year)).Sum(h => h.Amount);
            }
                return Expensetotal;
        }

        //Returns Total Amount of Transactions Based on a Budget Category
        public double GetMonthlyCategoryTransactionTotal(int bid, DateTime dateChosen)
        {
            double MonthCattotal = 0;
            int countMonthCattotal = dbCalc.Transactions.Where(t => t.BudgetId == bid && t.Archive == false && t.DateAdded.Month == dateChosen.Month && t.DateAdded.Year == dateChosen.Year).Count();

            if (countMonthCattotal > 0)
            {
                MonthCattotal = dbCalc.Transactions.Where(t => t.BudgetId == bid && t.Archive == false).Sum(a => a.Amount);
            }

            return MonthCattotal;
        }

        //Returns Total Amount of Transactions Based on a Budget Category
        public double GetMonthlyRecCategoryTransactionTotal(int bid, DateTime dateChosen)
        {
            double MonthCattotal = 0;
            int countMonthCattotal = dbCalc.Transactions.Where(t => t.BudgetId == bid && t.Archive == false && t.Reconciled == true).Count();

            if (countMonthCattotal > 0)
            {
                MonthCattotal = dbCalc.Transactions.Where(t => t.BudgetId == bid && t.Archive == false && t.Reconciled == true).Sum(a => a.Amount);
            }

            return MonthCattotal;
        }

        //Get Household Account Balance
        public double GetAccountBalance (int AccountId)
        {
            BankAccount Account = dbCalc.BankAccounts.Find(AccountId);
            double AccountIncome = 0;
            double AccountExpense = 0;
            double startBalance = Account.StartBalance;

            //Checks First To See If There Are Any Income Transactions Associated With The Account            
            var intCount = dbCalc.Transactions.Where(b => b.BankAccountId == AccountId && b.Archive == false && b.Budget.BudgetCategory.IncomeExpense == true).Count();

            if (intCount > 0)
            {
                AccountIncome = dbCalc.Transactions.Where(b => b.BankAccountId == AccountId && b.Archive == false && b.Budget.BudgetCategory.IncomeExpense == true).Sum(t => t.Amount);
                AccountIncome = AccountIncome + startBalance;
            }
            else
            {
                AccountIncome = startBalance;
            }

            //Checks First To See If There Are Any Expense Transactions Associated With The Account 
            var countExpense = dbCalc.Transactions.Where(b => b.BankAccountId == AccountId && b.Archive == false && b.Budget.BudgetCategory.IncomeExpense == false).Count();

            if (countExpense > 0)
            {
               AccountExpense = dbCalc.Transactions.Where(b => b.BankAccountId == AccountId && b.Archive == false && b.Budget.BudgetCategory.IncomeExpense == false).Sum(t => t.Amount);
            }

            //Income Transactions Minus Expense
            double AccountBalance = AccountIncome - AccountExpense; 
            
           return AccountBalance;
        }

        //Get Account Reconciled Balance
        public double GetReconciledAmt(int accountId)
        {
            BankAccount Account = dbCalc.BankAccounts.Find(accountId);
            double AccountIncome = 0;
            double AccountExpense = 0;
            double startBalance = Account.StartBalance;

           int countIncome = dbCalc.Transactions.Where(b => b.BankAccountId == accountId && b.Archive == false && b.Reconciled == true && b.Budget.BudgetCategory.IncomeExpense == true).Count();

            if (countIncome > 0)
            {
                AccountIncome = dbCalc.Transactions.Where(b => b.BankAccountId == accountId && b.Archive == false && b.Reconciled == true && b.Budget.BudgetCategory.IncomeExpense == true).Sum(t => t.Amount);
                AccountIncome = AccountIncome + startBalance;
            }

            int countExpense = dbCalc.Transactions.Where(b => b.BankAccountId == accountId && b.Archive == false && b.Reconciled == true && b.Budget.BudgetCategory.IncomeExpense == false).Count();

            if (countExpense > 0)
            {
                AccountExpense = dbCalc.Transactions.Where(b => b.BankAccountId == accountId && b.Archive == false && b.Reconciled == true && b.Budget.BudgetCategory.IncomeExpense == false).Sum(t => t.Amount);
            }

            double ReconcileBalance = AccountIncome - AccountExpense;

            return ReconcileBalance;
        }

        //Returns Whether or Not Budget is on Target
        public double CompareBudgetToActual (double budgetAmt, double ActualAmt)
        {
            double compareAmts = budgetAmt - ActualAmt;
            return compareAmts;
        }
    }
}