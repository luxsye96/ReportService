using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using ReportingService.Interface;
using ReportingService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingService.Service
{
    public class ReportService : IReportService
    {

        public double balanceAmount = 0;

        public string GenerateReport(DateTime date)
        {


            DateTime startTime = DateTimeUtility.StartOfWeek(date, DayOfWeek.Sunday);
            string sTime = startTime.ToString("yyyy-MM-dd");
            string endTime = date.ToString("yyyy-MM-dd");

            PdfPTable incomeTable = new PdfPTable(3);
            PdfPTable expTable = new PdfPTable(3);

            List<IncomeRep> incomeReportElements = GetIncomeReportElements(sTime, endTime);

            if (incomeReportElements.Count != 0)
            {
                incomeTable = GetIncomeTable(incomeReportElements);
            }

            List<ExpRep> expReportElements = GetExpenseReportElements(sTime, endTime);

            if (expReportElements.Count != 0)
            {
                expTable = GetExpenseTable(expReportElements);
            }


            string reportFile = GeneratePDFReport(sTime, endTime, incomeTable, expTable, balanceAmount);
            return reportFile;

        }

        public List<IncomeRep> GetIncomeReportElements(string sTime, string endTime)
        {
            List<Income> incomeList = new List<Income>();
            List<IncomeRep> incomeRepList = new List<IncomeRep>();

            Task<ResponseWrapper> response = GetIncomeByDateRangeAsync(sTime, endTime);
            ResponseWrapper responseWrapper = response.Result;
            if (responseWrapper.StatusCode == 200)
            {
                incomeList = JsonConvert.DeserializeObject<List<Income>>(responseWrapper.Result.ToString());

                foreach (Income i in incomeList)
                {
                    IncomeRep incomeRep = new IncomeRep();
                    incomeRep.Date = i.Date;
                    incomeRep.Income = i.IncomeName;
                    incomeRep.Amount = i.Amount;
                    incomeRepList.Add(incomeRep);
                }
            }

            return incomeRepList;
        }


        public async Task<ResponseWrapper> GetIncomeByDateRangeAsync(string sDate, string eDate)
        {
            string url = string.Format("/api/Income/{0}/{1}", sDate, eDate);
            ResponseWrapper rw = await TrackerService.Get<ResponseWrapper>(url);
            return rw;
        }


        public List<ExpRep> GetExpenseReportElements(string sTime, string endTime)
        {
            List<Expense> expenseList = new List<Expense>();
            List<ExpRep> expRepList = new List<ExpRep>();

            Task<ResponseWrapper> response = GetExpenseByDateRangeAsync(sTime, endTime);
            ResponseWrapper responseWrapper = response.Result;
            if (responseWrapper.StatusCode == 200)
            {
                expenseList = JsonConvert.DeserializeObject<List<Expense>>(responseWrapper.Result.ToString());

                foreach (Expense i in expenseList)
                {
                    ExpRep expRep = new ExpRep();
                    expRep.Date = i.Date;
                    expRep.Expense = i.ExpenseName;
                    expRep.Amount = i.Amount;
                    expRepList.Add(expRep);
                }
            }

            return expRepList;
        }


        public async Task<ResponseWrapper> GetExpenseByDateRangeAsync(string sDate, string eDate)
        {
            string url = string.Format("/api/Expense/{0}/{1}", sDate, eDate);
            ResponseWrapper rw = await TrackerService.Get<ResponseWrapper>(url);
            return rw;
        }

        public PdfPTable GetIncomeTable(List<IncomeRep> incomeReport)
        {
            double totalAmount = 0;

            PdfPTable table = new PdfPTable(3);

            table.AddCell("Date");
            table.AddCell("Income");
            table.AddCell("Amount");

            foreach (IncomeRep incomerep in incomeReport)
            {
                table.AddCell(incomerep.Date.ToString("yyyy-MM-dd"));
                table.AddCell(incomerep.Income);

                PdfPCell amountCell = new PdfPCell(new Phrase(incomerep.Amount.ToString("0.##")));
                amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(amountCell);
                totalAmount = totalAmount + incomerep.Amount;
            }

            PdfPCell cell = new PdfPCell(new Phrase("Total Income"));
            cell.Rowspan = 1;
            cell.Colspan = 2;
            table.AddCell(cell);

            PdfPCell tAmountCell = new PdfPCell(new Phrase(totalAmount.ToString("0.##")));
            tAmountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(tAmountCell);

            balanceAmount = balanceAmount + totalAmount;

            return table;

        }

        public PdfPTable GetExpenseTable(List<ExpRep> expReport)
        {
            double totalAmount = 0;

            PdfPTable table = new PdfPTable(3);
            table.AddCell("Date");
            table.AddCell("Expense");
            table.AddCell("Amount");

            foreach (ExpRep exp in expReport)
            {
                table.AddCell(exp.Date.ToString("yyyy-MM-dd"));
                table.AddCell(exp.Expense);

                PdfPCell amountCell = new PdfPCell(new Phrase(exp.Amount.ToString("0.##")));
                amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(amountCell);

                totalAmount = totalAmount + exp.Amount;
            }

            PdfPCell cell = new PdfPCell(new Phrase("Total Income"));
            cell.Rowspan = 1;
            cell.Colspan = 2;
            table.AddCell(cell);

            PdfPCell tAmountCell = new PdfPCell(new Phrase(totalAmount.ToString("0.##")));
            tAmountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(tAmountCell);


            balanceAmount = balanceAmount + totalAmount;
            return table;

        }

        public string GeneratePDFReport(string sDate, string date, PdfPTable incomeTable, PdfPTable expTable, double balance)
        {
            string fileName = Environment.CurrentDirectory + "\\" + date + ".pdf";

            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            iTextSharp.text.Font myFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 255));
            string header = String.Format("Income Expense Report \n\n From {0} to {1} \n\n \n\n", sDate, date);
            //string header = " <b> Income Expense Report </b> \n\n";

            Paragraph heading = new Paragraph(header, myFont);
            heading.Alignment = Element.ALIGN_CENTER;

            doc.Add(heading);
            doc.Add(new Paragraph("Income  \n\n"));

            doc.Add(incomeTable);

            doc.Add(new Paragraph("Expense \n\n"));
            doc.Add(expTable);


            doc.Add(GetBalanceTable(balance));
            doc.Close();

            return fileName;
        }

        public PdfPTable GetBalanceTable(double balance)
        {


            PdfPTable table = new PdfPTable(3);
            table.SpacingBefore = 20;
            PdfPCell cell = new PdfPCell(new Phrase("Balance Saving"));
            cell.Rowspan = 1;
            cell.Colspan = 2;
            table.AddCell(cell);

            PdfPCell amountCell = new PdfPCell(new Phrase(balance.ToString("0.##")));
            amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(amountCell);

            return table;

        }

        public string GetReportAsync(DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd");
            string fileName = Environment.CurrentDirectory + "\\" + dateString + ".pdf";

            if (!File.Exists(fileName))
            {
                fileName = GenerateReport(date);

            }

            return fileName;
        }



    }
}
