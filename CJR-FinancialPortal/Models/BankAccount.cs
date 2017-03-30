using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJR_FinancialPortal.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }

        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [Display(Name = "Route#")]
        public string RouteNumber { get; set; }
        [Display(Name = "Account#")]
        public string AccountNumber { get; set; }
        [Display(Name = "Start Balance")]
        public double StartBalance { get; set; }
        [Display(Name = "Account Key")]
        public string AccountLoginKey { get; set; }
        [Display(Name = "Account Pass")]
        public string AccountPassKey { get; set; }
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string zip { get; set; }
        [Display(Name = "Primary#")]
        public string PrimaryPhone { get; set; }
        [Display(Name = "Extention")]
        public int PhoneExtention { get; set; }
        [Display(Name = "Bank Website")]
        public string BankUrl { get; set; }
        [Display(Name = "Primaryf Account Owner")]
        public string PrimaryOwnerId {get; set;}
        [Display(Name = "Secondary Account Owner")]
        public string SecondaryOwnerId { get; set; }
        [Display(Name = "Account Added By")]
        public string AddedById { get; set; }
        [Display(Name = "Date")]
        public DateTime AddedDate { get; set; }
        [Display(Name = "In-Active")]
        public bool Active { get; set; }
      

        //Ticket Constructors for Children
        public BankAccount()
        {
            this.Transactions = new HashSet<Transaction>();
            this.AuditAccounts = new HashSet<AuditAccount>();

        }

        //Child Navigation
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<AuditAccount> AuditAccounts { get; set; }


        // virtual application user feed link
        public virtual ApplicationUser PrimaryOwner { get; set; }
        public virtual ApplicationUser SecondaryOwner { get; set; }
        public virtual ApplicationUser AddedBy { get; set; }

        //Parent Relationship
        public virtual AccountType AccountType { get; set; }
        public virtual HouseHold HouseHold { get; set; }
    }
}