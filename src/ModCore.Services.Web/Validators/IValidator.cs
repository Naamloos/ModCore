namespace ModCore.Services.Web.Validators
{
    public interface IValidator<T> : IValidator
    {
        Task<ValidatorResult<T>> ValidateAsync(HttpContext httpContext, T obj);
    }

    public interface IValidator
    {
        public static Task<ValidatorResult<T>> ValidateTypeAsync<T>(HttpContext httpContext, T value)
        {
            // find validator with reflection
            var validator = typeof(IValidator<>).Assembly.DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(IValidator<T>)))
                .FirstOrDefault();

            if (validator == null)
            {
                throw new InvalidOperationException($"No validator found for type {typeof(T).Name}");
            }

            var validatorInstance = (IValidator<T>)Activator.CreateInstance(validator)!;
            return validatorInstance.ValidateAsync(httpContext, value);
        }
    }
}
