using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution01 : ISolution
{
    private readonly record struct Command(int Direction, int Steps);
    private const int DialSize = 100;
    private const int StartDial = 50;

    public string RunPartA(string inputData)
    {
        int currentDial = StartDial;
        long countZeros = 0;

        IEnumerable<Command> commands = ParseCommands(inputData);

        foreach ((int direction, int value) in commands)
        {
            currentDial = (currentDial + direction * value) % DialSize;
            if (currentDial == 0)
            {
                countZeros++;
            }
        }

        return countZeros.ToString();
    }

    public string RunPartB(string inputData)
    {
        int currentDial = StartDial;
        long countZeros = 0;

        IEnumerable<Command> commands = ParseCommands(inputData);

        foreach ((int direction, int value) in commands)
        {
            int turns = 0;

            if (value > 99)
            {
                turns = value / DialSize;
            }

            int nextDial = currentDial + (value * direction % DialSize);

            if (currentDial != 0 && currentDial != DialSize && (nextDial <= 0 || nextDial >= DialSize))
            {
                turns++;
            }

            countZeros += turns;
            currentDial = nextDial % DialSize;

            if (currentDial < 0)
            {
                currentDial = DialSize + currentDial;
            }
        }

        return countZeros.ToString();
    }

    private static IEnumerable<Command> ParseCommands(string input)
    {
        return ParseUtils
        .ParseIntoLines(input)
        .Select(line => new Command(
            line[0] == 'R' ? 1 : -1,
            int.Parse(line[1..])
        ));
    }

}