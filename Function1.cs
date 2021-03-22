using FunctionApp1.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly IServiceProvider serviceProvider;

        public Function1(
            IServiceProvider serviceProvider
        )
        {
            this.serviceProvider = serviceProvider;
        }

        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 30 9 * * *",
        #if DEBUG
            RunOnStartup= true
        #endif
        )]TimerInfo myTimer, ILogger log)
        { 
            using var scope = serviceProvider.CreateScope();
            try
            {
                string requestBody = "{}";
                var razorService = serviceProvider.GetRequiredService<IRazorService>();

                string html = null;
                var container = JsonConvert.DeserializeObject<APageModel>(requestBody);

                var model = new APageModel
                {
                };
                html = await razorService.RenderViewToStringAsync(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

