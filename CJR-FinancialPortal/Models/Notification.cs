using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Note Sent")]
        public string NoteSent { get; set; }
        [Display(Name = "Date Sent")]
        public DateTime DateSent { get; set; }
        public bool NotificatoinRead { get; set; }


        // virtual application user feed link
        public virtual ApplicationUser User { get; set; }

        //Parent Relationship
        public virtual HouseHold HouseHold { get; set; }
    }
}