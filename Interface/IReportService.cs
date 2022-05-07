using System;

namespace ReportingService.Interface
{
    public interface IReportService
    {
        string GenerateReport(DateTime date);

        string GetReportAsync(DateTime date);

    }
}
