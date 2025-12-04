using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution04 : ISolution
{
    public string RunPartA(string inputData)
    {
        char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);
        long count = 0;

        int rows = matrix.Length;
        int cols = matrix[0].Length;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (matrix.At(i, j) != '@') continue;

                char?[] neighbors = matrix.GetNeighbors((i, j));

                count += neighbors.Count(c => c.Equals('@')) < 4 ? 1 : 0;
            }
        }

        return count.ToString();
    }

    public string RunPartB(string inputData)
    {
        char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);
        long count = 0;

        int rows = matrix.Length;
        int cols = matrix[0].Length;
        bool canFinish = false;

        while (!canFinish)
        {
            List<(int, int)> rollsToRemove = [];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix.At(i, j) != '@') continue;

                    char?[] neighbors = matrix.GetNeighbors((i, j));

                    if (neighbors.Count(c => c.Equals('@')) < 4)
                    {
                        rollsToRemove.Add((i, j));
                    }
                }
            }

            canFinish = rollsToRemove.Count == 0;
            count += rollsToRemove.Count;

            foreach ((int rollX, int rollY) in rollsToRemove)
            {
                matrix.SetAt(rollX, rollY, '.');
            }
        }

        return count.ToString();
    }
}