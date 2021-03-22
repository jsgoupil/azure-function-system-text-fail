using FunctionApp1.Application;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]

namespace FunctionApp1
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            try
            {
                // On Azure, this is the path where the app is.
                // If you use Directory.GetCurrentDirectory(), you will get D:\Program Files (x86)\SiteExtensions\Functions\3.0.14785\32bit
                // Adapted from: https://stackoverflow.com/a/60078802
                var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..");
                builder.ConfigurationBuilder
                    .SetBasePath(basePath)
                    .AttachConfiguration(new HbtiEnvironment()
                    {
                        EnvironmentName = builder.GetContext().EnvironmentName
                    });
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// NOTE! This startup does not start automatically with Microsoft.NET.Sdk.Functions 3.0.9.
        /// We tried by dropping an hardcoded file extensions.json to the output/bin directory.
        /// However, during publishing, the SDK runs the generator once more preventing us to use a BeforeTargets properly.
        /// It seems that the generator 1.2.0 work at the moment.
        /// If anything were to happen again, search in Diagnostics level build the following message: _GenerateFunctionsExtensionsMetadataPostBuild
        public override void Configure(IFunctionsHostBuilder builder)
        {
            try
            {
                string executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var compiledViewAssembly = Assembly.LoadFile(Path.Combine(executionPath, "FunctionApp1.Views.dll"));
                builder.Services
                    .AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
                    .AddSingleton<IRazorService, RazorService>()
                    .AddMvcCore()
                    .AddViews()
                    .AddRazorViewEngine()
                    .AddApplicationPart(compiledViewAssembly);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        private void HandleException(Exception ex)
        {
        }

        class HbtiEnvironment : IHostEnvironment
        {
            public string ApplicationName { get => typeof(HbtiEnvironment).AssemblyQualifiedName; set => throw new NotImplementedException(); }
            public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string EnvironmentName { get; set; }
        }
    }
}
