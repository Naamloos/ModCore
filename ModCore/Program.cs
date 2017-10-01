using System.Threading.Tasks;

namespace ModCore
{
    internal class Program
    {
        private static Task Main(string[] args) =>
            new ModCore().InitializeAsync();
    }
}
