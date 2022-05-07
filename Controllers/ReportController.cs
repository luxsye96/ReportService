using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ReportingService.Interface;
using System;
using System.IO;
using System.Web.Http;

namespace ReportingService.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {


        private IReportService iReportSvc;

        public ReportController(IReportService repSvc)
        {
            iReportSvc = repSvc;

        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("report")]
        public ResponseWrapper Report()
        {

            ResponseWrapper response = new ResponseWrapper();
            DateTime todate = DateTime.Now;
            RecurringJob.AddOrUpdate(() => GenerateReport(todate), Cron.Hourly);

            response.Message = "Report Generated";
            return response;
        }

        public void GenerateReport(DateTime date)
        {
            _ = iReportSvc.GenerateReport(date);
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("{date}")]
        public ResponseWrapper GenerateReportA(DateTime date)
        {
            ResponseWrapper response = new ResponseWrapper();
            try
            {
                String file = iReportSvc.GenerateReport(date);
                response.StatusCode = 200;
                response.Message = "Generated report succesfully";
                response.Result = file;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }

            return response;
        }


        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("{date}")]
        public IActionResult GetReport(DateTime date)
        {
            string fileName = iReportSvc.GetReportAsync(date);
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string downloadFileName = date.ToString("yyyy-MM-dd") + ".pdf";
            return File(stream, "application/octet-stream", downloadFileName);
        }


    }
}
