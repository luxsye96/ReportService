using Microsoft.AspNetCore.Mvc;
using ReportingService.Interface;
using ReportingService.Model;
using System;

namespace ReportingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForcastController : Controller
    {

        private IForcastService forCastService;

        public ForcastController(IForcastService forCastSvc)
        {
            forCastService = forCastSvc;

        }


        [HttpGet("{date}")]
        public ResponseWrapper GetForCast(DateTime date)
        {
            ResponseWrapper response = new ResponseWrapper();
            try
            {
                Forcast forcast = forCastService.GetForcastByDate(date);
                response.Result = forcast;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
