using aoc_2025.AocClient;
using aoc_2025.Classes;
using aoc_2025.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace aoc_2025
{
    public class Program
    {
        static void Main()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<ILogger, ConsoleLogger>()
                .AddSingleton<ISolutionManager, SolutionManager>()
                .AddSingleton<ISolutionChecker, SolutionChecker>()
                .AddSingleton<ILastExecutionManager, LastExecutionManager>()
                .AddSingleton<IAocClient, AocHttpClient>()
                .AddSingleton<IRunner, ConsoleRunner>()
                .AddSingleton<ITestManager, TestManager>()
                .AddSingleton<IController, ConsoleController>()
                .BuildServiceProvider();

            IController inputController = serviceProvider.GetRequiredService<IController>();
            inputController.Run();
        }
    }
}
