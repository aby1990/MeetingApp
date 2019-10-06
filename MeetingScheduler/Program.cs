using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MeetingScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                                    .AddLogging()
                                    .AddSingleton<IPlanMeetups, PlanMeetups>()
                                    .AddSingleton<IProcessMeetups, ProcessMeetups >()
                                    .BuildServiceProvider();
            serviceProvider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information);
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            
            //read file for input and keeping it in list of strings
            var inputMeetups = File.ReadAllLines("input.txt").SelectMany(line => line.Split("\n")).AsEnumerable();

            var procMeetups = serviceProvider.GetService<IProcessMeetups>();
            procMeetups.GetBookedMeetups(inputMeetups);
        }
    }
}
