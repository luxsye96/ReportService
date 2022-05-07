using System;
using System.ComponentModel.DataAnnotations;

namespace ReportingService.Model
{
    public class Expense
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int ExpenseType { get; set; }
        public string ExpenseName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
    }
}
