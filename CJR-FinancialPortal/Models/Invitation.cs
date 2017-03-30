using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int? HouseHoldId { get; set; }
        [Display(Name = "Email")]
        public string InvEmail { get; set; }
        [Display(Name = "Confrim Email")]
        public string InvEmailConfirm { get; set; }
        [Display(Name = "Secret Code")]
        public string SecretCode { get; set; }
        public bool CodeUsed { get; set; }
        public string SentById { get; set; }
        public DateTime SentTimeStamp { get; set;}
        public bool Archive { get; set; }

        //Access User Information

        public virtual ApplicationUser SentBy { get; set; }

        //Parent Relationship
        public virtual HouseHold HouseHold { get; set; }
    }
}