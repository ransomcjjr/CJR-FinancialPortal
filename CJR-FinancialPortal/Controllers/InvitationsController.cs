using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJR_FinancialPortal.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace CJR_FinancialPortal.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitations
        [AuthorizeHousehold]
        public ActionResult Index()
        {
            var invitations = db.Invitations.Include(i => i.HouseHold).Include(i => i.SentBy);
            return View(invitations.ToList());
        }

        // GET: Invitations/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        [AuthorizeHousehold]
        public ActionResult Create()
        {
            //ViewBag.HouseHoldId = new SelectList(db.HouseHolds, "Id", "Name");
            //ViewBag.SentById = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "InvEmail,InvEmailConfirm")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                string strCode = Membership.GeneratePassword(12, 2);

                var U = User.Identity.GetUserId();
                var HHId = db.Users.Find(U);

                invitation.HouseHoldId = HHId.HouseHoldId;
                invitation.SecretCode = strCode;
                invitation.SentById = User.Identity.GetUserId();
                invitation.SentTimeStamp = DateTime.Now;
                invitation.Archive = false;
                invitation.CodeUsed = false;
                db.Invitations.Add(invitation);
                db.SaveChanges();

                var msg = new SendMsgsHelper();
                await msg.HouseholdInvitation(invitation.InvEmail, invitation.SentById, strCode);


                return RedirectToAction("Index","BankAccounts");
            }
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            //ViewBag.HouseHoldId = new SelectList(db.HouseHolds, "Id", "Name", invitation.HouseHoldId);
            //ViewBag.SentById = new SelectList(db.ApplicationUsers, "Id", "FirstName", invitation.SentById);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseHoldId,InvEmail,InvEmailConfirm,SecretCode,CodeUsed,SentById,SentTimeStamp,Archive")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.HouseHoldId = new SelectList(db.HouseHolds, "Id", "Name", invitation.HouseHoldId);
           // ViewBag.SentById = new SelectList(db.ApplicationUsers, "Id", "FirstName", invitation.SentById);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
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
