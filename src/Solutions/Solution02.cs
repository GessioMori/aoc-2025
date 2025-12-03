using aoc_2025.Interfaces;

namespace aoc_2025.Solutions;

public class Solution02 : ISolution
{
    private readonly record struct Range(long Min, long Max);

    public string RunPartA(string inputData)
    {
        IEnumerable<Range> ranges = ParseInput(inputData)
            .SelectMany(range => SplitRangeInEqualLengths(GetNextMinEvenLengthNumber(range.Min), GetPreviousMaxEvenLengthNumber(range.Max)));

        HashSet<long> invalidIds = [];

        foreach (Range range in ranges)
        {
            IEnumerable<long> rangeInvalidIds = GetInvalidIdsInRange(range);

            foreach (long invalidId in rangeInvalidIds) { invalidIds.Add(invalidId); }
        }

        return invalidIds.Sum().ToString();
    }

    public string RunPartB(string inputData)
    {
        throw new NotImplementedException();
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

    private static long GetNextMinEvenLengthNumber(long number)
    {
        string stringNum = number.ToString();

        if (stringNum.Length % 2 == 0) return number;

        return long.Parse("1".PadRight(stringNum.Length + 1, '0'));
    }

    private static long GetPreviousMaxEvenLengthNumber(long number)
    {
        string stringNum = number.ToString();

        if (stringNum.Length % 2 == 0) return number;
        if (stringNum.Length == 1) return -1;

        return long.Parse("9".PadRight(stringNum.Length - 1, '9'));
    }

    private static List<Range> SplitRangeInEqualLengths(long min, long max)
    {
        List<Range> ranges = [];

        if (max == -1) return ranges;

        if (min.ToString().Length == max.ToString().Length)
        {
            ranges.Add(new Range(min, max));
            return ranges;
        }

        int currentLength = min.ToString().Length;
        long currentMin = min;

        while (currentLength <= max.ToString().Length)
        {
            ranges.Add(new Range(currentMin, Math.Min(long.Parse("9".PadRight(currentLength - 1, '9')), max)));
            currentLength += 2;
            currentMin = long.Parse("1".PadRight(currentLength, '0'));
        }

        return ranges;
    }

    private static List<long> GetInvalidIdsInRange(Range range)
    {
        List<long> invalidIds = [];
        string sMin = range.Min.ToString();
        string sMax = range.Max.ToString();
        int halfLenght = sMin.Length / 2;

        long halfMin = long.Parse(sMin[..halfLenght]);
        long halfMax = long.Parse(sMax[..halfLenght]);

        for (long i = halfMin; i <= halfMax; i++)
        {
            long value = long.Parse(i.ToString() + i.ToString());
            if (value >= range.Min && value <= range.Max)
            {
                invalidIds.Add(value);
            }
        }

        return invalidIds;
    }
}