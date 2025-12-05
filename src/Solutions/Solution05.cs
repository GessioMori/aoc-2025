using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution05 : ISolution
{
    private record struct Range(long Min, long Max);
    private record struct Database(List<Range> Ranges, List<long> Ids);
    private record struct RangePoint(long Value, bool IsStart);

    public string RunPartA(string inputData)
    {
        Database database = ParseInput(inputData);

        long count = 0;

        foreach (long id in database.Ids)
        {
            foreach (Range range in database.Ranges)
            {
                if (id >= range.Min && id <= range.Max)
                {
                    count++;
                    break;
                }
            }
        }

        return count.ToString();
    }

    public string RunPartB(string inputData)
    {
        Database database = ParseInput(inputData);
        List<RangePoint> orderedRangePoints = CreateRangePointsList(database.Ranges);

        List<Range> nonOverlappedRanges = [];

        long currentMin = orderedRangePoints[0].Value;
        int startsCount = 1;

        for (int i = 1; i < orderedRangePoints.Count; i++)
        {
            RangePoint currentRangePoint = orderedRangePoints[i];

            if (i == orderedRangePoints.Count - 1)
            {
                nonOverlappedRanges.Add(new Range(currentMin, currentRangePoint.Value));
                continue;
            }

            if (startsCount == 0)
            {
                currentMin = currentRangePoint.Value;
                startsCount++;
                continue;
            }

            if (currentRangePoint.IsStart)
            {
                startsCount++;
            }
            else
            {
                startsCount--;

                if (startsCount == 0)
                {
                    nonOverlappedRanges.Add(new Range(currentMin, currentRangePoint.Value));
                }
            }
        }

        long count = 0;

        foreach (Range range in nonOverlappedRanges)
        {
            count += range.Max - range.Min + 1;
        }

        return count.ToString();
    }

    private static Database ParseInput(string inputData)
    {
        string[] lines = ParseUtils.ParseIntoLines(inputData);
        List<Range> ranges = [];
        List<long> ids = [];

        foreach (string line in lines)
        {
            if (line.Contains('-'))
            {
                string[] parts = line.Split('-');
                ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
            }
            else
            {
                ids.Add(long.Parse(line));
            }
        }

        return new Database(ranges, ids);
    }

    private static List<RangePoint> CreateRangePointsList(IEnumerable<Range> ranges)
    {
        List<RangePoint> rangePoints = [];

        foreach (Range range in ranges)
        {
            rangePoints.Add(new RangePoint(range.Min, true));
            rangePoints.Add(new RangePoint(range.Max, false));
        }

        return rangePoints
            .OrderBy(r => r.Value)
            .ThenByDescending(rp => rp.IsStart)
            .ToList();
    }
}