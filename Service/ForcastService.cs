using Newtonsoft.Json;
using ReportingService.DTO;
using ReportingService.Interface;
using ReportingService.Model;
using System;
using System.Threading.Tasks;

namespace ReportingService.Service
{
    public class ForcastService : IForcastService
    {

        public Forcast GetForcastByDate(DateTime date)
        {
            DateTime todate = DateTime.Now;
            double daystoGo = (date - todate).TotalDays;

            double forcastIncome, forcastExp;

            Forcast forcast = new Forcast();
            ForcastDTO forcastDTO = new ForcastDTO();

            Task<ResponseWrapper> response = GetIncomeForcastAsync();
            ResponseWrapper resp = response.Result;

            if (resp.StatusCode == 200)
            {
                forcastDTO = JsonConvert.DeserializeObject<ForcastDTO>(resp.Result.ToString());
                // forcastDTO = (ForcastDTO)response.Result.Result;
                forcastIncome = daystoGo * forcastDTO.AvgAmount;
                forcast.ExpectedIncome = forcastIncome + forcastDTO.TotalAmount;
            }

            Task<ResponseWrapper> expResponse = GetExpenseForcastAsync();
            ResponseWrapper respex = expResponse.Result;

            if (respex.StatusCode == 200)
            {
                forcastDTO = JsonConvert.DeserializeObject<ForcastDTO>(respex.Result.ToString());
                forcastExp = daystoGo * forcastDTO.AvgAmount;
                forcast.ExpectedExpense = forcastExp + forcastDTO.TotalAmount;
            }

            return forcast;
        }


        public async Task<ResponseWrapper> GetIncomeForcastAsync()
        {
            ResponseWrapper rw = await TrackerService.Get<ResponseWrapper>($"/api/Income/avgIncome");
            return rw;
        }


        public async Task<ResponseWrapper> GetExpenseForcastAsync()
        {
            ResponseWrapper rw = await TrackerService.Get<ResponseWrapper>($"/api/Expense/avgExp");
            return rw;
        }

    }
}
