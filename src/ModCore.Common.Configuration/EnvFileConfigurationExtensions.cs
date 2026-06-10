using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace ModCore.Common.Configuration
{
    public static class EnvFileConfigurationExtensions
    {
        public static IConfigurationBuilder AddEnvFile(
            this IConfigurationBuilder builder,
            string path = ".env",
            bool optional = false,
            bool reloadOnChange = true)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Provided path does not contain an env file!", nameof(path));

            string providerRoot;
            string fileName;

            if (Path.IsPathRooted(path))
            {
                providerRoot = Path.GetDirectoryName(path)
                    ?? throw new ArgumentException("Could not determine env file directory.", nameof(path));

                fileName = Path.GetFileName(path);
            }
            else
            {
                providerRoot = AppContext.BaseDirectory;
                fileName = path;
            }

            var fileProvider = new PhysicalFileProvider(
                providerRoot,
                ExclusionFilters.None);

            return builder.AddEnvFile(
                provider: fileProvider,
                path: fileName,
                optional: optional,
                reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddEnvFile(
            this IConfigurationBuilder builder,
            IFileProvider provider,
            string path,
            bool optional,
            bool reloadOnChange)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Provided path does not contain an env file!", nameof(path));

            return builder.AddEnvFile(source =>
            {
                source.FileProvider = provider;
                source.Path = path;
                source.Optional = optional;
                source.ReloadOnChange = reloadOnChange;
            });
        }

        public static IConfigurationBuilder AddEnvFile(
            this IConfigurationBuilder builder,
            Action<EnvFileConfigurationSource> configureSource)
        {
            return builder.Add(configureSource);
        }
    }
}