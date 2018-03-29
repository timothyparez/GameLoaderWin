namespace TeamArg.GameLoader.Model
{
    public class LogEntry
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public LogEntry(string message, bool isError = false)
        {
            Message = message;
            IsError = isError;
        }
    }
}