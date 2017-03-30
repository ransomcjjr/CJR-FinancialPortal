using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class Transaction
    {

        //Ticket Properties (Same as table)
        public int Id { get; set; }
        public int BankAccountId { get; set; }

        //BudgetId Nullable
        public int? BudgetId { get; set; }
        public int PaymentTypeId { get; set; }
        public int MerchantId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
        public bool Reconciled { get; set; }
        [Display(Name = "Reconciled Amt")]
        public double? ReconciledAmt { get; set; }
        [Display(Name = "Date")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Added By")]
        public string AddedById { get; set; }

        [Display(Name = "Voided")]
        public bool Archive { get; set; }


        //Ticket Constructors for Children
        public Transaction()
        {
            this.AuditTransactions = new HashSet<AuditTransaction>();
        }

        //Child Navigation
        public virtual ICollection<AuditTransaction> AuditTransactions { get; set; }

        // virtual application user feed link
        public virtual ApplicationUser AddedBy { get; set; }

        //Parent Relationship
        public virtual Budget Budget { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Merchant Merchant { get; set; }

    }
}