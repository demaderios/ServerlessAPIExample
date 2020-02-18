using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ServerlessMusic.API.Helpers;

[assembly: WebJobsStartup(typeof(Startup))]
namespace ServerlessMusic.API.Helpers
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddLogging(loggingbuilder => { loggingbuilder.AddFilter(level => true); });

            var config = (IConfiguration) builder.Services.First(d => d.ServiceType == typeof(IConfiguration))
                .ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(System.Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));

                return client;
            });
        }
    }
}
