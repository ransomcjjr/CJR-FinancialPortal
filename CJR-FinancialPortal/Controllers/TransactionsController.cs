using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJR_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace CJR_FinancialPortal.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [AuthorizeHousehold]
        public ActionResult IndexOld()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var transactions = db.Transactions.Include(t => t.AddedBy).Include(t => t.BankAccount).Include(t => t.Budget).Include(t => t.Merchant).Include(t => t.PaymentType).Where(t => t.Budget.HouseHoldId == hid);
            return View(transactions.ToList());
        }

        // GET: Transactions In Household By Date Range/Amount Range

        public ActionResult Index(string query, int? searchType)
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (searchType == 1)
            {
                string[] daterange = query.Split('-');
                DateTime begindate = Convert.ToDateTime(daterange[0].Trim());
                DateTime enddate = Convert.ToDateTime(daterange[1].Trim());
                var transactions = db.Transactions.Include(t => t.AddedBy).Include(t => t.BankAccount).Include(t => t.Budget).Include(t => t.Merchant).Include(t => t.PaymentType).Where(t => t.Budget.HouseHoldId == hid && t.DateAdded >= begindate && t.DateAdded <= enddate);
                return View(transactions.ToList());
            }
            else if (searchType == 2)
            {
                string[] daterange = query.Split(',');
                double beginAmt = Convert.ToDouble(daterange[0].Trim());
                double endAmt = Convert.ToDouble(daterange[1].Trim());
                var transactions = db.Transactions.Include(t => t.AddedBy).Include(t => t.BankAccount).Include(t => t.Budget).Include(t => t.Merchant).Include(t => t.PaymentType).Where(t => t.Budget.HouseHoldId == hid && t.Amount >= beginAmt && t.Amount <= endAmt);
                return View(transactions.ToList());
            }
            else
            { 
                var transactions = db.Transactions.Include(t => t.AddedBy).Include(t => t.BankAccount).Include(t => t.Budget).Include(t => t.Merchant).Include(t => t.PaymentType).Where(t => t.Budget.HouseHoldId == hid);
                return View(transactions.ToList());
            }
        }

        // GET: Transactions/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [AuthorizeHousehold]
        public ActionResult Create()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());

            var accountList = db.BankAccounts.Where(b => b.HouseHoldId == hid);
            ViewBag.BankAccountId = new SelectList(accountList, "Id", "BankName");

            var budgetList = db.Budgets.Where(b => b.HouseHoldId == hid);
            ViewBag.BudgetId = new SelectList(budgetList, "Id", "NickName");

            var merchantList = db.Merchants.Where(m => m.HouseHoldId == hid);
            ViewBag.MerchantId = new SelectList(merchantList, "Id", "DisplayName");

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "BankAccountId,BudgetId,PaymentTypeId,MerchantId,Amount,Note,Reconciled,ReconciledAmt,Archive")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                //Create Transaction
                transaction.DateAdded = DateTime.Now;
                transaction.AddedById = User.Identity.GetUserId();
                db.Transactions.Add(transaction);
                db.SaveChanges();

                //Send Notification
                ReconcileAndBalanceHelper RABH = new ReconcileAndBalanceHelper();
                //Check for Account Overdrawn
                double AccountBal = RABH.GetAccountBalance(transaction.BankAccountId);
                var budget = db.Budgets.Find(transaction.BudgetId);
                if (budget.BudgetCategory.IncomeExpense == false)
                {
                    var bank = db.BankAccounts.Find(transaction.BankAccountId);
                    int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
                    SendMsgsHelper SMH = new SendMsgsHelper();

                    if (AccountBal < 0)
                    {
                        //Send Account Overdrawn Notification
                        await SMH.AccountOverdrawnMsg(bank.BankName, bank.AccountNumber, AccountBal,hid);
                    }

                    //Check for Budget Overage
                    double MonthTotalBudgetExpense = RABH.GetBudgetExpenseTotal(hid);
                    double MonthTotalExpense = RABH.GetMonthlyExpenseTransactionTotal(hid, DateTime.Now);

                    if (MonthTotalExpense > MonthTotalBudgetExpense)
                    {
                        //Send Over Budget Notification
                        await SMH.OverBudgetMsg(MonthTotalBudgetExpense, hid);
                    }

                    //Check for Budget Category Overage
                    int bid = Convert.ToInt32(transaction.BudgetId);
                    double CategoryMonthTrans = RABH.GetMonthlyCategoryTransactionTotal(bid, DateTime.Now);
                    double CatBudgetAmt = RABH.GetBudgetCategoryTotal(bid);
                    //CategoryMonthTrans = CategoryMonthTrans + transaction.Amount;
                    if (CategoryMonthTrans > CatBudgetAmt)
                    {
                        //Send Over Category Monthly Budget Amount Notification
                        await SMH.OverCategoryBudgetMsg(CategoryMonthTrans, hid, budget.BudgetCategory.Name);
                    }
                }

                return RedirectToAction("Index");
            }

            int hid1 = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accountList = db.BankAccounts.Where(b => b.HouseHoldId == hid1);
            ViewBag.BankAccountId = new SelectList(accountList, "Id", "BankName",transaction.BankAccountId);

            var budgetList = db.Budgets.Where(b => b.HouseHoldId == hid1);
            ViewBag.BudgetId = new SelectList(budgetList, "Id", "NickName",transaction.BudgetId);

            var merchantList = db.Merchants.Where(m => m.HouseHoldId == hid1);
            ViewBag.MerchantId = new SelectList(merchantList, "Id", "DisplayName",transaction.MerchantId);

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name",transaction.PaymentTypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accountList = db.BankAccounts.Where(b => b.HouseHoldId == hid);
            ViewBag.BankAccountId = new SelectList(accountList, "Id", "BankName",transaction.BankAccountId);

            var budgetList = db.Budgets.Where(b => b.HouseHoldId == hid);
            ViewBag.BudgetId = new SelectList(budgetList, "Id", "NickName",transaction.BudgetId);

            var merchantList = db.Merchants.Where(m => m.HouseHoldId == hid);
            ViewBag.MerchantId = new SelectList(merchantList, "Id", "DisplayName",transaction.MerchantId);

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name",transaction.PaymentTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccountId,BudgetId,PaymentTypeId,MerchantId,Amount,Note,Reconciled,ReconciledAmt,Archive")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                //DateAdded,AddedById,
                //db.Entry(transaction).State = EntityState.Modified;
                db.Transactions.Attach(transaction);
                db.Entry(transaction).Property("BankAccountId").IsModified = true;
                db.Entry(transaction).Property("BudgetId").IsModified = true;
                db.Entry(transaction).Property("PaymentTypeId").IsModified = true;
                db.Entry(transaction).Property("MerchantId").IsModified = true;
                db.Entry(transaction).Property("Amount").IsModified = true;
                db.Entry(transaction).Property("Note").IsModified = true;
                db.Entry(transaction).Property("Reconciled").IsModified = true;
                db.Entry(transaction).Property("ReconciledAmt").IsModified = true;
                db.Entry(transaction).Property("Archive").IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accountList = db.BankAccounts.Where(b => b.HouseHoldId == hid);
            ViewBag.BankAccountId = new SelectList(accountList, "Id", "BankName",transaction.BankAccountId);

            var budgetList = db.Budgets.Where(b => b.HouseHoldId == hid);
            ViewBag.BudgetId = new SelectList(budgetList, "Id", "NickName",transaction.BudgetId);

            var merchantList = db.Merchants.Where(m => m.HouseHoldId == hid);
            ViewBag.MerchantId = new SelectList(merchantList, "Id", "DisplayName",transaction.MerchantId);

            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name",transaction.PaymentTypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
