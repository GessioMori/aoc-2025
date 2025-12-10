using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution09 : ISolution
{
    private readonly record struct Point(int PosX, int PosY);
    private readonly record struct RowRange(int Row, int StartCol, int EndCol);

    public string RunPartA(string inputData)
    {
        List<Point> points = ParseInput(inputData);

        long maxArea = long.MinValue;

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                long dx = Math.Abs(points[i].PosX - points[j].PosX) + 1;
                long dy = Math.Abs(points[i].PosY - points[j].PosY) + 1;
                if (dx * dy > maxArea)
                {
                    maxArea = dx * dy;
                }
            }
        }

        return maxArea.ToString();
    }

    public string RunPartB(string inputData)
    {
        List<Point> points = ParseInput(inputData);

        long maxX = long.MinValue;
        long maxY = long.MinValue;

        foreach (Point point in points)
        {
            maxX = Math.Max(maxX, point.PosX);
            maxY = Math.Max(maxY, point.PosY);
        }

        long width = maxX + 2;
        long height = maxY + 2;

        char[][] matrix = new char[height][];
        for (int y = 0; y < height; y++)
        {
            matrix[y] = [.. Enumerable.Repeat('.', (int)width)];
        }

        for (int i = 0; i < points.Count; i++)
        {
            Point a = points[i];
            Point b = (i == points.Count - 1) ? points[0] : points[i + 1];

            matrix.SetAt(a.PosX, a.PosY, 'X');

            if (a.PosX == b.PosX)
            {
                int from = Math.Min(a.PosY, b.PosY);
                int to = Math.Max(a.PosY, b.PosY);

                for (int y = from + 1; y < to; y++)
                {
                    matrix.SetAt(a.PosX, y, 'X');
                }
            }
            else if (a.PosY == b.PosY)
            {
                int from = Math.Min(a.PosX, b.PosX);
                int to = Math.Max(a.PosX, b.PosX);

                for (int x = from + 1; x < to; x++)
                {
                    matrix.SetAt(x, a.PosY, 'X');
                }
            }
        }

        PriorityQueue<(Point, Point, long), long> pq = new();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                long dx = Math.Abs(points[i].PosX - points[j].PosX) + 1;
                long dy = Math.Abs(points[i].PosY - points[j].PosY) + 1;
                long area = dx * dy;

                pq.Enqueue((points[i], points[j], area), -area);
            }
        }

        List<(Point, Point, long)> orderedRectangles = [];
        while (pq.Count > 0)
        {
            orderedRectangles.Add(pq.Dequeue());
        }

        List<RowRange> outsideRanges = GetOutsideRanges(matrix, '.');

        foreach ((Point a, Point b, long area) in orderedRectangles)
        {
            if (IsRectangleFullyInsidePolygon(a, b, outsideRanges))
            {
                return area.ToString();
            }
        }

        throw new Exception("Area not found");
    }

    private static List<RowRange> GetOutsideRanges(char[][] matrix, char outsideChar)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;

        Queue<(int r, int c)> queue = new();

        void EnqueueIfOutside(int r, int c)
        {
            if (matrix[r][c] == outsideChar)
            {
                matrix[r][c] = 'O';
                queue.Enqueue((r, c));
            }
        }

        for (int c = 0; c < matrix.NumOfColumns; c++)
        {
            EnqueueIfOutside(0, c);
            EnqueueIfOutside(matrix.NumOfRows - 1, c);
        }

        for (int r = 0; r < matrix.NumOfRows; r++)
        {
            EnqueueIfOutside(r, 0);
            EnqueueIfOutside(r, matrix.NumOfColumns - 1);
        }

        while (queue.Count > 0)
        {
            (int r, int c) = queue.Dequeue();

            if (r > 0) EnqueueIfOutside(r - 1, c);
            if (r + 1 < rows) EnqueueIfOutside(r + 1, c);
            if (c > 0) EnqueueIfOutside(r, c - 1);
            if (c + 1 < cols) EnqueueIfOutside(r, c + 1);
        }

        List<RowRange> ranges = [];

        for (int r = 0; r < rows; r++)
        {
            int start = -1;

            for (int c = 0; c < cols; c++)
            {
                if (matrix[r][c] == 'O')
                {
                    if (start < 0)
                    {
                        start = c;
                    }
                }
                else if (start >= 0)
                {
                    ranges.Add(new RowRange(r, start, c - 1));
                    start = -1;
                }
            }

            if (start >= 0)
            {
                ranges.Add(new RowRange(r, start, cols - 1));
            }
        }

        return ranges;
    }

    private static List<Point> ParseInput(string inputData)
    {
        return ParseUtils.ParseIntoLines(inputData)
            .Select(line =>
            {
                string[] parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
            })
            .ToList();
    }

    private static bool IsRectangleFullyInsidePolygon(Point pointA, Point pointB, List<RowRange> outsideRanges)
    {
        int minY = Math.Min(pointA.PosY, pointB.PosY);
        int maxY = Math.Max(pointA.PosY, pointB.PosY);
        int minX = Math.Min(pointA.PosX, pointB.PosX);
        int maxX = Math.Max(pointA.PosX, pointB.PosX);

        foreach (RowRange outsideRange in outsideRanges)
        {
            if (outsideRange.Row >= minY && outsideRange.Row <= maxY &&
                Math.Max(outsideRange.StartCol, minX) <= Math.Min(outsideRange.EndCol, maxX))
            {
                return false;
            }
        }

        return true;
    }
}