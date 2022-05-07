using System.Collections.Generic;

namespace ReportingService.Model
{
    public class ReportParams<T>
    {
        public string RptFileName { get; set; }

        public string ReportTittle { get; set; }

        public List<T> DataSource { get; set; }

        public bool IsPassPramToCr { get; set; }
    }
}
