using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution03 : ISolution
{
    public string RunPartA(string inputData)
    {
        int[][] banks = ParseInput(inputData);

        long totalSum = 0;

        foreach (int[] bank in banks)
        {
            int[] values = new int[2];
            FillNextPositions(-1, 2, bank, values);
            totalSum += int.Parse(string.Concat(values));
        }

        return totalSum.ToString();
    }

    public string RunPartB(string inputData)
    {
        int[][] banks = ParseInput(inputData);

        long totalSum = 0;

        foreach (int[] bank in banks)
        {
            int[] values = new int[12];
            FillNextPositions(-1, 12, bank, values);
            totalSum += long.Parse(string.Concat(values));
        }

        return totalSum.ToString();
    }

    private static int[][] ParseInput(string inputData)
    {
        return ParseUtils
            .ParseIntoLines(inputData)
            .Select(x => x.Select(c => int.Parse(c.ToString())).ToArray())
            .ToArray();
    }

    private static void FillNextPositions(int lastUsedPosition, int remainingBatteriesToFill, int[] bank, int[] values)
    {
        if (remainingBatteriesToFill == 0) return;

        for (int i = 9; i >= 0; i--)
        {
            for (int j = lastUsedPosition + 1; j < bank.Length - remainingBatteriesToFill + 1; j++)
            {
                if (bank[j] == i)
                {
                    values[^remainingBatteriesToFill] = i;
                    FillNextPositions(j, remainingBatteriesToFill - 1, bank, values);
                    return;
                }
            }
        }
    }
}