using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class AuditTransaction
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Changed { get; set; }

        public string ChangedById { get; set; }

        // virtual application user feed link
        public virtual ApplicationUser ChangedBy { get; set; }

        //Parent Relationship
        public virtual Transaction Transaction { get; set; }
    }
}