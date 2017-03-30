using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class Merchant
    {
        //Table Field Properties
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        [Display(Name = "Merchant")]
        public string DisplayName { get; set; }
        [Display(Name = "Merchant Name")]
        public string Name { get; set; }
        [Display(Name = "Account#")]
        public string AccountNum { get; set; }
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip")]
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        [Display(Name = "Date")]
        public DateTime AddedDate { get; set; }
        [Display(Name = "Added By")]
        public string AddedById { get; set; }
        [Display(Name = "In-Active")]
        public bool Archive { get; set; }

        //Constructor
        public Merchant()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        //Child Nav
        public virtual ICollection<Transaction> Transactions { get; set; }

        public virtual ApplicationUser AddedBy { get; set; }
    }
}