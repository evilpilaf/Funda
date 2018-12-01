using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Funda.Core.Entities;
using Funda.Core.Usecases;

using FundaWebApiServiceAdapter;

using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;

namespace FundaConsoleHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var log = new LoggerConfiguration().WriteTo.ColoredConsole(LogEventLevel.Debug).CreateLogger();

            ServiceProvider serviceProvider = new ServiceCollection()
                                              .AddLogging(logginBuilder => logginBuilder.AddSerilog(log))
                                              .AddScoped<GetTop10MakelaarsInAmsterdamUseCase>()
                                              .RegisterWebApiServiceAdapter()
                                              .BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var useCase = scope.ServiceProvider.GetService<GetTop10MakelaarsInAmsterdamUseCase>();

                IEnumerable<Makelaar> result = await useCase.Execute();

                foreach (Makelaar m in result)
                {
                    Console.WriteLine($"{m.Id} - {m.Name}");
                }
            }

            Console.ReadLine();
        }
    }
}