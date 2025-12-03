using aoc_2025.Interfaces;

namespace aoc_2025.Solutions;

public class Solution02 : ISolution
{
    private readonly record struct Range(long Min, long Max);

    public string RunPartA(string inputData)
    {
        return ParseInput(inputData)
            .SelectMany(range => GetInvalidIdsInRange(range, true))
            .Sum()
            .ToString();
    }

    public string RunPartB(string inputData)
    {
        return ParseInput(inputData)
            .SelectMany(range => GetInvalidIdsInRange(range, false))
            .Sum()
            .ToString();
    }

    private static IEnumerable<Range> ParseInput(string inputData)
    {
        return inputData
            .Split(',')
            .Select(s =>
            {
                string[] parts = s.Split('-');
                return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
            });
    }

    private static List<long> GetInvalidIdsInRange(Range range, bool onlyTwoParts)
    {
        List<long> invalidIds = [];

        for (long i = range.Min; i <= range.Max; i++)
        {
            if (onlyTwoParts && CanBeSplittedInEqualParts(i, 2))
            {
                invalidIds.Add(i);
            }
            else if(!onlyTwoParts)
            {
                for (int j = 2; j <= i.ToString().Length; j++)
                {
                    if (CanBeSplittedInEqualParts(i, j))
                    {
                        invalidIds.Add(i);
                        break;
                    }
                }
            }
        }

        return invalidIds;
    }

    private static bool CanBeSplittedInEqualParts(long number, int partsCount)
    {
        string sNum = number.ToString();

        if (partsCount <= 0 || sNum.Length % partsCount != 0)
        {
            return false;
        }

        int partLength = sNum.Length / partsCount;

        string first = sNum[..partLength];

        for (int i = 1; i < partsCount; i++)
        {
            if (!string.Equals(
                    sNum.Substring(i * partLength, partLength),
                    first,
                    StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}