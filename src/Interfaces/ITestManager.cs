using aoc_2025.Classes;

namespace aoc_2025.Interfaces;

public interface ITestManager
{
    List<TestCase> Parse(int dayNumber);
    int[] GetAvailableTests();
}
