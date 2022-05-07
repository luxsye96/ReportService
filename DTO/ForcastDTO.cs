using System;

namespace ReportingService.DTO
{
    public class ForcastDTO
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalAmount { get; set; }

        public double AvgAmount { get; set; }

    }
}
