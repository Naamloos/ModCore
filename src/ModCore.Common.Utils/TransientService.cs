using Microsoft.Extensions.DependencyInjection;

namespace ModCore.Common.Utils
{
    public class TransientService<T> where T : notnull
    {
        private IServiceProvider _services;

        public TransientService(IServiceProvider services) 
        {
            _services = services;
        }

        public T GetTransient()
        {
            return _services.GetRequiredService<T>();
        }

        public static implicit operator T(TransientService<T> value) { return value.GetTransient(); }
    }
}
