using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    //TODO: ADD STATE Table and Month Table
    public class Budget
    {
        public int Id { get; set; }
        public int? HouseHoldId { get; set; }
        [Display(Name = "Budget Category")]
        public int BudgetCategoryId {get; set;}
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }
        [Display(Name = "Budget Amount")]
        public double BudgetAmount { get; set; }
        [Display(Name = "Date")]
        public DateTime AddedDate { get; set; }
        [Display(Name = "Added By")]
        public string AddByUserId { get; set; }
        [Display(Name = "In-Active")]
        public bool Archive { get; set; }


        //Ticket Constructors for Children
        public Budget()
        {
            this.Transactions = new HashSet<Transaction>();
            this.AuditBudgets = new HashSet<AuditBudget>();
        }

        //Child Navigation
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<AuditBudget> AuditBudgets { get; set; }

        // virtual application user feed link
        public virtual ApplicationUser AddByUser { get; set; }

        //Parent Relationship
        public virtual HouseHold HouseHold { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}