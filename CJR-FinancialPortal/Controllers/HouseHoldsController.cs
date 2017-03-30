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
using System.Web.Security;
using System.Security.Claims;

namespace CJR_FinancialPortal.Controllers
{
    public class HouseHoldsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseHolds
        [AuthorizeHousehold]
        public ActionResult Index()
        {
            return View(db.HouseHolds.ToList());
        }

        // GET: HouseHolds/Details/5
        [AuthorizeHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseHold houseHold = db.HouseHolds.Find(id);
            if (houseHold == null)
            {
                return HttpNotFound();
            }
            return View(houseHold);
        }

        // GET: HouseHolds/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: HouseHolds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] HouseHold houseHold)
        {
            if (ModelState.IsValid)
            {
                string currUser = User.Identity.GetUserId();
                houseHold.Created = DateTime.Now;
                houseHold.CreatedById = currUser;
                db.HouseHolds.Add(houseHold);
                db.SaveChanges();

                ApplicationUser user = db.Users.Find(currUser);
                HouseHold retHousehold = db.HouseHolds.FirstOrDefault(r => r.CreatedById == currUser);
                user.HouseHoldId = retHousehold.Id;
                db.Users.Attach(user);
                db.Entry(user).Property("HouseHoldId").IsModified = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(houseHold);
        }


        // GET: HouseHolds/Join
        [Authorize]
        public ActionResult Join()
        {
            return View();
        }

        // POST: HouseHolds/Join
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join(string InvEmail, string InvEmailConfirmed, string SecretCode)
        {
            if (ModelState.IsValid)
            {
                
                Invitation invite = db.Invitations.FirstOrDefault(i => i.SecretCode == SecretCode && i.InvEmail == InvEmail && i.CodeUsed == false);

                if (invite != null)
                {
                    //Update User Table to Household Id
                    string currentUser = User.Identity.GetUserId();
                    ApplicationUser user = db.Users.Find(currentUser);
                    user.HouseHoldId = invite.HouseHoldId;
                    db.Users.Attach(user);
                    db.Entry(user).Property("HouseHoldId").IsModified = true;
                    db.SaveChanges();

                    //Update Invitation Table
                    invite.CodeUsed = true;
                    db.Invitations.Attach(invite);
                    db.Entry(invite).Property("CodeUsed").IsModified = true;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("JoinErrorMsg","HouseHolds");
            }

            return RedirectToAction("JoinErrorMsg", "HouseHolds");
        }

        // GET: HouseHolds/LeaveHousehold
        [AuthorizeHousehold]
        public ActionResult LeaveHousehold()
        {
            int id = Convert.ToInt32(User.Identity.GetHouseholdId());
            List<UserInHouseholdVeiwModel> userList = new List<UserInHouseholdVeiwModel>();
           var houseHoldUsers = db.Users.FirstOrDefault(a => a.HouseHoldId == id);

            foreach (var users in db.Users.ToList().Where(h => h.HouseHoldId == id))
            {
                var userCollection = new UserInHouseholdVeiwModel();
                userCollection.User = users;
                userList.Add(userCollection);
            }
            return View(userList);
        }

        // POST: HouseHolds/LeaveHousehold
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> LeaveHousehold(string memberId)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(memberId);
                HouseHold houseHold = db.HouseHolds.Find(user.HouseHoldId);
                user.HouseHoldId = null;
                db.Users.Attach(user);
                db.Entry(user).Property("HouseHoldId").IsModified = true;
                db.SaveChanges();

                var userid = User.Identity.GetUserId();

                if (memberId == userid)
                {
                    await ControllerContext.HttpContext.RefreshAuthentication(user);
                    return RedirectToAction("ChooseHouseHold", "Account");
                }

                return View(houseHold);

            }
            return RedirectToAction("Index", "HouseHold");
        }

        //Dashboard Notifications List
        public PartialViewResult _DashboardUserNotifications()
        {
            var uid = User.Identity.GetUserId();
            var note = db.Notifications.Where(n => n.UserId == uid).OrderByDescending(d => d.DateSent).Take(5).ToList();
            return PartialView(note);
        }

        // GET: HouseHolds/Edit/5
        [AuthorizeHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseHold houseHold = db.HouseHolds.Find(id);
            if (houseHold == null)
            {
                return HttpNotFound();
            }
            return View(houseHold);
        }

        // POST: HouseHolds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created,CreatedById")] HouseHold houseHold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(houseHold).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(houseHold);
        }

        // GET: HouseHolds/Delete/5
        [AuthorizeHousehold]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser RemoveUser = db.Users.Find(id);
            if (RemoveUser == null)
            {
                return HttpNotFound();
            }
            return View(RemoveUser);
        }

        // POST: HouseHolds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser RemoveUser = db.Users.Find(id);
            RemoveUser.HouseHoldId = 0;
            RemoveUser.HHAuthorized = false;
            db.Users.Attach(RemoveUser);
            db.Entry(RemoveUser).Property("HouseHoldId").IsModified = true;
            db.Entry(RemoveUser).Property("HHAuthorized").IsModified = true;
            db.SaveChanges();

            //TODO: Add code if current user is leaving to log them out of the current session and direct them back to the choose household page.
            return RedirectToAction("LeaveHouseHold");
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
