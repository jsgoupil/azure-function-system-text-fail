using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace FunctionApp1
{
    public static class Extensions
    {

        public static IConfigurationBuilder AttachConfiguration(this IConfigurationBuilder builder, IHostEnvironment environment)
        {
            builder.AddJsonFile("appsettings-core.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings-core.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            // We go get the JSON setting in the bin folder, this is because we use a link.
            if (environment.IsDevelopment())
            {
                var location = Assembly.GetEntryAssembly().Location;
                var directory = Path.GetDirectoryName(location);
                builder.AddJsonFile(Path.Combine(directory, "appsettings-core.json"), optional: true, reloadOnChange: true)
                    .AddJsonFile(Path.Combine(directory, $"appsettings-core.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
            }

            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (environment.IsDevelopment())
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
                    if (assembly != null)
                    {
                        builder.AddUserSecrets(assembly, optional: true);
                    }
                }
                catch (Exception)
                {
                }
            }

            builder.AddEnvironmentVariables();

            return builder;
        }

    }
}
