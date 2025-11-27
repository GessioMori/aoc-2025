using aoc_2025.Classes;

namespace aoc_2025.Interfaces;

public interface IRunner
{
    ExecutionResult Run(ISolution solution, int dayNumber, Part part, string input);
}
