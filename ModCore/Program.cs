using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ModCore.Entities;
using Newtonsoft.Json;

namespace ModCore
{
    internal class Program
    {
        private static void Main(string[] args) => new ModCore().Initialize().ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
