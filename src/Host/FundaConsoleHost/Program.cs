﻿using System;
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
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var log = new LoggerConfiguration().WriteTo.ColoredConsole(LogEventLevel.Warning).CreateLogger();

            ServiceProvider serviceProvider = new ServiceCollection()
                                              .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(log))
                                              .AddScoped<GetTop10MakelaarsInAmsterdamUseCase>()
                                              .RegisterWebApiServiceAdapter()
                                              .BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var useCase = scope.ServiceProvider.GetService<GetTop10MakelaarsInAmsterdamUseCase>();

                await GetTopAmsterdamMakelaars(useCase);
            }

            Console.ReadLine();
        }

        private static async Task GetTopAmsterdamMakelaars(GetTop10MakelaarsInAmsterdamUseCase useCase)
        {
            IEnumerable<Tuple<Makelaar, int>> results = await useCase.Execute();
            PrintResults(results);
        }

        private static void PrintResults(IEnumerable<Tuple<Makelaar,int>> results)
        {
            PrintLine();
            PrintRow("Makelaar name", "Total listings");

            foreach (var result in results)
            {
                PrintLine();
                PrintRow(result.Item1.Name, result.Item2.ToString());
            }
                
            PrintLine();
        }

        private const int _tableWidth = 77;

        private static void PrintLine()
        {
            Console.WriteLine(new string('-', _tableWidth));
        }

        private static void PrintRow(params string[] columns)
        {
            int width = (_tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}