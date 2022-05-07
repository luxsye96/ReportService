using System.Collections.Generic;

namespace ReportingService.Model
{
    public class Report
    {

        public List<IncomeRep> IncomeReport { get; set; }

        public List<Expense> ExpenseReport { get; set; }

        public double Balance { get; set; }


    }
}
