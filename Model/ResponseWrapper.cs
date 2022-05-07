namespace ReportingService
{
    public class ResponseWrapper
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }


    }
}