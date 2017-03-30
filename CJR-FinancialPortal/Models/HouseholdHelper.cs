using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public static class HouseholdHelper
    {
        public static string GetHouseholdId(this IIdentity user)
            {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseHoldId");

            if (HouseholdClaim != null)
            {
                return HouseholdClaim.Value;
            }
            else
            {
                return null;
            }

            }
        public static bool IsInHousehold(this IIdentity user)
        {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "HouseHoldId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }
    }
}