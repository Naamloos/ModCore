using Microsoft.Extensions.Configuration;
using System.IO;

namespace ModCore.Common.Configuration
{
    public class EnvFileConfigurationProvider : FileConfigurationProvider
    {
        public EnvFileConfigurationProvider(FileConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            foreach (var item in EnvFileReader.Load(stream))
            {
                Data[item.Key] = item.Value;
            }
        }
    }
}
