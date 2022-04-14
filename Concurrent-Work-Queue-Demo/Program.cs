using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Concurrent_Work_Queue_Demo
{
    public partial class Program
    {
        #region Methods

        public static void Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(Program.ConfigureWebHost);

            using (var host = hostBuilder.Build())
            {
                host.Run();
            }
        }

        private static void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseStartup<Startup>();
        }

        #endregion
    }
}
