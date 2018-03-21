using System.Threading.Tasks;

namespace ModCore
{
    internal static class Program
    {
        private static Task Main(string[] args) => (ModCore = new ModCore()).InitializeAsync(args);

        public static ModCore ModCore { get; private set; }
    }
}
