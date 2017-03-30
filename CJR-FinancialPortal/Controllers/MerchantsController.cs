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
    public class MerchantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Merchants
        [AuthorizeHousehold]
        public ActionResult Index()
        {
            int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
            //var merchants = db.Merchants.Include(m => m.AddedBy);
            var merchants = db.Merchants.Where(m => m.HouseHoldId == hid);
            return View(merchants.ToList());
        }

        // GET: Merchants/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            return View(merchant);
        }

        // GET: Merchants/Create
        [AuthorizeHousehold]
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Merchants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisplayName,Name,AccountNum,Address1,Address2,City,State,ZipCode,Phone,Description,Archive")] Merchant merchant)
        {
            //AddedDate,AddedById,Id,HouseholdId
            if (ModelState.IsValid)
            {
                int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
                merchant.HouseHoldId = hid;
                merchant.AddedById = User.Identity.GetUserId();
                merchant.AddedDate = DateTime.Now;
                db.Merchants.Add(merchant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           
            return View(merchant);
        }

        // GET: Merchants/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
           
            return View(merchant);
        }

        // POST: Merchants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DisplayName,Name,AccountNum,Address1,Address2,City,State,ZipCode,Phone,Description,Archive")] Merchant merchant)
        {

            //HouseholdId,AddedDate,AddedById,
            if (ModelState.IsValid)
            {
                int hid = Convert.ToInt32(User.Identity.GetHouseholdId());
                db.Merchants.Attach(merchant);
                db.Entry(merchant).Property("DisplayName").IsModified = true;
                db.Entry(merchant).Property("Name").IsModified = true;
                db.Entry(merchant).Property("AccountNum").IsModified = true;
                db.Entry(merchant).Property("Address1").IsModified = true;
                db.Entry(merchant).Property("Address2").IsModified = true;
                db.Entry(merchant).Property("City").IsModified = true;
                db.Entry(merchant).Property("State").IsModified = true;
                db.Entry(merchant).Property("ZipCode").IsModified = true;
                db.Entry(merchant).Property("Phone").IsModified = true;
                db.Entry(merchant).Property("Description").IsModified = true;
                db.Entry(merchant).Property("Archive").IsModified = true;
                //db.Entry(merchant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(merchant);
        }

        // GET: Merchants/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            return View(merchant);
        }

        // POST: Merchants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            db.Merchants.Remove(merchant);
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
