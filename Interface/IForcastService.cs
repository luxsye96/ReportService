using ReportingService.Model;
using System;

namespace ReportingService.Interface
{
    public interface IForcastService
    {
        Forcast GetForcastByDate(DateTime date);
    }
}
