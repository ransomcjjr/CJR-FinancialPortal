using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class AccountType
    {
        //Table Field Properties
        public int Id { get; set; }

        [Display(Name = "Acount Type")]
        public string Name { get; set; }
        public string Description { get; set; }

        //Constructor
        public AccountType()
        {
            this.BankAccounts = new HashSet<BankAccount>();
        }

        //Child Nav
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}