using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CJR_FinancialPortal.Models;
using System.Data.Entity;

namespace CJR_FinancialPortal.Models
{
    public class SendMsgsHelper
    {
        private EmailService es = new EmailService();
        private IdentityMessage mm = new IdentityMessage();
        private ApplicationDbContext dbEmail = new ApplicationDbContext();

        public async Task HouseholdInvitation(string ToEmail,string strUser,string strSecretCode)
        {
            var curruser = dbEmail.Users.Find(strUser);
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("You have been invited by " + curruser.FirstName + " " + curruser.LastName + "to join their financial budget household</ br>");
            strBuild.Append("In order to join you must use the one time code listed below along with the email address this invitation was sent to.</ br>");
            strBuild.Append("Click on the link below to register and join the group.</ br></ br>");
            strBuild.Append("Required Email: " + ToEmail + "</ br>");
            strBuild.Append("Secret Code: " + strSecretCode + "</ br></ br>");
            strBuild.Append("<a herf='https://CJR-FinancialPortal/Registration.com'");

            mm.Subject = "Invitation To Join Household Budget Software";
            mm.Body = strBuild.ToString();
            mm.Destination = ToEmail;

            await es.SendAsync(mm);

        }

        public async Task AccountOverdrawnMsg(string AccountName, string AccountNumber,double AccountBalance, int hid)
        {
            var householdUsers = dbEmail.Users.Where(u => u.HouseHoldId == hid).ToList();

            foreach (var user in householdUsers)
            {
                StringBuilder strBuild = new StringBuilder();
                strBuild.Append("Dear " + user.FullName + " your " + AccountName + " is overdrawn! The current balance is " + AccountBalance);
                mm.Subject = "Financial Portal Alert: Account Overdrawn";
                mm.Body = strBuild.ToString();
                mm.Destination = user.Email;

                await es.SendAsync(mm);

                AddNotification(hid, user.Id, strBuild.ToString());
            }  
        }

        public async Task OverBudgetMsg(double OverBudgetAmt, int hid)
        {
            var householdUsers = dbEmail.Users.Where(u => u.HouseHoldId == hid).ToList();

            foreach (var user in householdUsers)
            {
                StringBuilder strBuild = new StringBuilder();
                strBuild.Append("Dear " + user.FullName + " your expenses have exceeded your monthly budgeted amount! Your current monthly expense amount is " + OverBudgetAmt);
                mm.Subject = "Financial Portal Alert: Expenses Exceeded Budget";
                mm.Body = strBuild.ToString();
                mm.Destination = user.Email;

                await es.SendAsync(mm);
                AddNotification(hid, user.Id, strBuild.ToString());
            }
        }

        public async Task OverCategoryBudgetMsg(double OverBudgetAmt, int hid, string catname)
        {
            var householdUsers = dbEmail.Users.Where(u => u.HouseHoldId == hid).ToList();

            foreach (var user in householdUsers)
            {
                StringBuilder strBuild = new StringBuilder();
                strBuild.Append("Dear " + user.FullName + " your expenses have exceeded your monthly " + catname + " category budgeted amount! Your current category monthly expense amount is " + OverBudgetAmt);
                mm.Subject = "Financial Portal Alert: Expenses Exceeded Budget";
                mm.Body = strBuild.ToString();
                mm.Destination = user.Email;

                await es.SendAsync(mm);
                AddNotification(hid, user.Id, strBuild.ToString());
            }
        }

        protected void AddNotification(int hid,string uid, string notesent)
        {
            Notification notification = new Notification();
            notification.HouseHoldId = hid;
            notification.UserId = uid;
            notification.NoteSent = notesent;
            notification.DateSent = DateTime.Now;
            notification.NotificatoinRead = false;
            dbEmail.Notifications.Add(notification);
            dbEmail.SaveChanges();
        }
    }
}