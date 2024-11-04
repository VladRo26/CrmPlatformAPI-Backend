namespace CrmPlatformAPI.Errors
{
    public class Exceptions(int _status, string _message, string? _details) : Exception
    {
        public int Status { get; set; } = _status;
        public string Message { get; set; } =_message;

        public string? Details { get; set; } = _details;

    }

}
