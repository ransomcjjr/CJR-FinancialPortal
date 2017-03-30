using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public class BudgetCategory
    {
        //Table Field Properties
        public int Id { get; set; }
        [Display(Name = "Budget Category")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Income/Expense")]
        public bool IncomeExpense { get; set; }

        //Constructor
        public BudgetCategory()
        {
            this.Budgets = new HashSet<Budget>();
        }

        //Child Nav
        public virtual ICollection<Budget> Budgets { get; set; }
    }
}