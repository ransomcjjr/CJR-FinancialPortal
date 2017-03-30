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
using Newtonsoft.Json;

namespace CJR_FinancialPortal.Controllers
{
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        [AuthorizeHousehold]
        public ActionResult Index()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            //var bankAccounts = db.BankAccounts.Include(b => b.AccountType).Include(b => b.AddedBy).Include(b => b.HouseHold).Include(b => b.PrimaryOwner).Include(b => b.SecondaryOwner);
            var bankAccounts = db.BankAccounts.Where(b => b.HouseHoldId == hid);
            return View(bankAccounts.ToList());
        }


        //Dashboard Budget Pie Chart Data
        public ActionResult GetPieChart()
        {
            //Current Monthly Expense Budget vs Monthly Transactions
            var MonthlyExpenseBudget = new ReconcileAndBalanceHelper();
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            double BudgetExpense = MonthlyExpenseBudget.GetBudgetExpenseTotal(hid);
            double ActualExpense = MonthlyExpenseBudget.GetMonthlyExpenseTransactionTotal(hid, DateTime.Now);

            var data = new[] { new {label = "Expense Budget", value = BudgetExpense },
                new { label = "Actual Expense", value = ActualExpense } };

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        //Dashboard 1 month Outlook
        public ActionResult GetBarChart()
        {
            var RABH = new ReconcileAndBalanceHelper();
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var budgetItems = db.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false);
            var countbudgetItems = db.Budgets.Where(h => h.HouseHoldId == hid && h.Archive == false).Count();
            double amount = 0;
            object[] data = new object[countbudgetItems];
            int counter = 0;
            foreach (var item in budgetItems)
            {
                //var data = new ChartData();
                if (countbudgetItems > 0)
                {
                    amount = RABH.GetMonthlyCategoryTransactionTotal(item.Id, DateTime.Now);
                    data[counter] = new { name = item.NickName, budget = item.BudgetAmount, actual = Math.Round(amount,2) };
                    counter++;
                }
                else
                {
                    data[counter] = new { name = item.NickName, budget = 0, actual = 0 };
                    counter++;
                }
            }
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        //Struct For GetBarChart
        public struct ChartData
        {
            public string NickName { get; set; }
            public double BudgetAmt { get; set; }
            public double CatActualAmt { get; set; }
        }

        // GET: BankAccounts/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ReconcileAndBalanceHelper RABH = new ReconcileAndBalanceHelper();

            BankAccount bankAccount = db.BankAccounts.Find(id);
            double accountBalance = RABH.GetAccountBalance(bankAccount.Id);
            double accountRecBalance = RABH.GetReconciledAmt(bankAccount.Id);

            //double accountBalance = 100;
            //double accountRecBalance = 150;


            ViewBag.AccBalance = accountBalance;
            ViewBag.RecBalance = accountRecBalance;

            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        [AuthorizeHousehold]
        public ActionResult Create()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var fullUserList = db.Users.Where(h => h.HouseHoldId == hid);

            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name");
            ViewBag.PrimaryOwnerId = new SelectList(fullUserList, "Id", "FullName");
            ViewBag.SecondaryOwnerId = new SelectList(fullUserList, "Id", "FullName");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountTypeId,BankName,RouteNumber,AccountNumber,StartBalance,AccountLoginKey,AccountPassKey,Address1,Address2,City,State,zip,PrimaryPhone,PhoneExtention,BankUrl,PrimaryOwnerId,SecondaryOwnerId,Active")] BankAccount bankAccount)
        {
            
            if (ModelState.IsValid)
            {
                int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
                var userList = db.Users.FirstOrDefault(h => h.HouseHoldId == hid); 
                bankAccount.HouseHoldId = hid;
                bankAccount.AddedDate = DateTime.Now;
                bankAccount.AddedById = User.Identity.GetUserId();
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int hid1 = Convert.ToInt32(User.Identity.GetHouseholdId());
            var fullUserList = db.Users.Where(h => h.HouseHoldId == hid1);

            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name");
            ViewBag.PrimaryOwnerId = new SelectList(fullUserList, "Id", "FullName");
            ViewBag.SecondaryOwnerId = new SelectList(fullUserList, "Id", "FullName");
            return View(bankAccount);
        }


        // GET: BankAccounts/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var fullUserList = db.Users.Where(h => h.HouseHoldId == hid);

            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name",bankAccount.AccountTypeId);
            ViewBag.PrimaryOwnerId = new SelectList(fullUserList, "Id", "FullName",bankAccount.PrimaryOwnerId);
            ViewBag.SecondaryOwnerId = new SelectList(fullUserList, "Id", "FullName",bankAccount.SecondaryOwnerId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountTypeId,BankName,RouteNumber,AccountNumber,StartBalance,AccountLoginKey,AccountPassKey,Address1,Address2,City,State,zip,PrimaryPhone,PhoneExtention,BankUrl,PrimaryOwnerId,SecondaryOwnerId,Active")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property("AccountTypeId").IsModified = true;
                db.Entry(bankAccount).Property("BankName").IsModified = true;
                db.Entry(bankAccount).Property("RouteNumber").IsModified = true;
                db.Entry(bankAccount).Property("AccountNumber").IsModified = true;
                db.Entry(bankAccount).Property("StartBalance").IsModified = true;
                db.Entry(bankAccount).Property("AccountLoginKey").IsModified = true;
                db.Entry(bankAccount).Property("AccountPassKey").IsModified = true;
                db.Entry(bankAccount).Property("Address1").IsModified = true;
                db.Entry(bankAccount).Property("Address2").IsModified = true;
                db.Entry(bankAccount).Property("City").IsModified = true;
                db.Entry(bankAccount).Property("State").IsModified = true;
                db.Entry(bankAccount).Property("zip").IsModified = true;
                db.Entry(bankAccount).Property("PrimaryPhone").IsModified = true;
                db.Entry(bankAccount).Property("PhoneExtention").IsModified = true;
                db.Entry(bankAccount).Property("BankUrl").IsModified = true;
                db.Entry(bankAccount).Property("PrimaryOwnerId").IsModified = true;
                db.Entry(bankAccount).Property("SecondaryOwnerId").IsModified = true;
                db.Entry(bankAccount).Property("Active").IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Details",new {id = bankAccount.Id });
            }
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            var fullUserList = db.Users.Where(h => h.HouseHoldId == hid);

            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name",bankAccount.AccountTypeId);
            ViewBag.PrimaryOwnerId = new SelectList(fullUserList, "Id", "FullName",bankAccount.PrimaryOwnerId);
            ViewBag.SecondaryOwnerId = new SelectList(fullUserList, "Id", "FullName",bankAccount.SecondaryOwnerId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
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
