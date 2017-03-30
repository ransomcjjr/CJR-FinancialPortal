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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        [AuthorizeHousehold]
        public ActionResult Index()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            //var budgets = db.Budgets.Include(b => b.AddByUser).Include(b => b.BudgetCategory).Include(b => b.HouseHold);
            var budgets = db.Budgets.Where(h => h.HouseHoldId == hid);
            return View(budgets.ToList());
        }

        // GET: Budgets/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }

            ReconcileAndBalanceHelper RABH = new ReconcileAndBalanceHelper();
            ViewBag.MonthlyTotal = RABH.GetMonthlyCategoryTransactionTotal(budget.Id,DateTime.Now);
            ViewBag.RecMontlyTotal = RABH.GetMonthlyRecCategoryTransactionTotal(budget.Id,DateTime.Now);

            return View(budget);
        }

        // GET: Budgets/Create
        [AuthorizeHousehold]
        public ActionResult Create()
        {
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategories, "Id", "Name");

            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BudgetCategoryId,NickName,BudgetAmount")] Budget budget)
        {
            //HouseHoldId,AddedDate,AddByUserId,,Archive
            if (ModelState.IsValid)
            {
                int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
                budget.HouseHoldId = hid;
                budget.AddedDate = DateTime.Now;
                budget.AddByUserId = User.Identity.GetUserId();
                budget.Archive = false;
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategories, "Id", "Name", budget.BudgetCategoryId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategories, "Id", "Name", budget.BudgetCategoryId);

            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BudgetCategoryId,NickName,BudgetAmount,Archive")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                //HouseHoldId,AddedDate,AddByUserId,
                db.Budgets.Attach(budget);
                db.Entry(budget).Property("BudgetCategoryId").IsModified = true;
                db.Entry(budget).Property("NickName").IsModified = true;
                db.Entry(budget).Property("BudgetAmount").IsModified = true;
                db.Entry(budget).Property("Archive").IsModified = true;

                //db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategories, "Id", "Name", budget.BudgetCategoryId);

            return View(budget);
        }

        // GET: Budgets/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
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
