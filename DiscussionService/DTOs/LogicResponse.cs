namespace DiscussionService.DTOs
{
    public class LogicResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public LogicResponseType Type { get; set; }

        public static LogicResponse<T> Ok(T data) => new LogicResponse<T> { Success = true, Data = data };
        public static LogicResponse<T> Fail(string message, LogicResponseType type) => new LogicResponse<T> { Success = false, ErrorMessage = message, Type = type };
    }

    public enum LogicResponseType
    {
        NotFound,
        Conflict,
        BadRequest,
        None
    }
}
