using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class HouseHold
    {
        //Table Field Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string CreatedById { get; set; } 

        //Child Construtor
        public HouseHold()
        {
            this.Budgets = new HashSet<Budget>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Notifications = new HashSet<Notification>();
            this.CreatedBy = new HashSet<ApplicationUser>();
            this.Invitations = new HashSet<Invitation>();

        }

        //Child Navigation
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Notification>Notifications { get; set; }
        public virtual ICollection<Invitation>Invitations { get; set; }

        //Many to Many Example

        public virtual ICollection<ApplicationUser> CreatedBy { get; set; }

    }
}