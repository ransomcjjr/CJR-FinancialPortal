using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class PaymentType
    {
        //Table Field Properties
        public int Id { get; set; }
        [Display(Name = "Payment Type")]
        public string Name { get; set; }
        public string Description { get; set; }

        //Constructor
        public PaymentType()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        //Child Nav
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}