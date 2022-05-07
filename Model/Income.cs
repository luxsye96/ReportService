using System;
using System.ComponentModel.DataAnnotations;

namespace ReportingService.Model
{
    public class Income
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int IncomeType { get; set; }
        public string IncomeName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }




    }
}