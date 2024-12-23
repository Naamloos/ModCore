namespace ModCore.Services.Web.Validators
{
    public class ValidatorResult<T>
    {
        public bool Success { get; set; }
        public T Value { get; set; }
        public string? Message { get; set; }
        public ValidatorResult(bool success, T value, string? message)
        {
            Success = success;
            Value = value;
            Message = message;
        }
    }
}
