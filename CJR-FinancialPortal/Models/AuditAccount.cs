using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJR_FinancialPortal.Models
{
    public class AuditAccount
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Changed { get; set; }

        public string ChangedById { get; set; }

        // virtual application user feed link
        public virtual ApplicationUser ChangedBy { get; set; }

        //Parent Relationship
        public virtual BankAccount BankAccount { get; set; }
    }
}