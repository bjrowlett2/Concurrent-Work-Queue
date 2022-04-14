using System;

using Concurrent_Work_Queue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Concurrent_Work_Queue_Demo
{
    public partial class Program
    {
        public class Startup
        {
            #region Methods

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddControllers();

                // In this demo the work queue just contains ints!
                serviceCollection.AddSingleton<ConcurrentWorkQueue<Int64>>();
            }

            public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment)
            {
                if (webHostEnvironment.IsDevelopment())
                {
                    applicationBuilder.UseDeveloperExceptionPage();
                }

                applicationBuilder.UseRouting();

                applicationBuilder.UseEndpoints(this.ConfigureEndpoints);
            }

            private void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
            {
                endpointRouteBuilder.MapControllers();
            }

            #endregion
        }
    }
}
