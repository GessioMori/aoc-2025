using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution07 : ISolution
{
    public string RunPartA(string inputData)
    {
        char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

        (int, int) start = matrix.FirstInstanceOf('S');

        Queue<(int, int)> queue = [];
        HashSet<(int, int)> visited = [];
        queue.Enqueue(start);

        int splitCount = 0;

        while (queue.Count > 0)
        {
            (int x, int y) currentPosition = queue.Dequeue();

            if (visited.Contains(currentPosition)) continue;

            visited.Add(currentPosition);

            if ((matrix.At(currentPosition) == '.' || matrix.At(currentPosition) == 'S') && currentPosition.y + 1 < matrix.NumOfRows)
            {
                queue.Enqueue((currentPosition.x, currentPosition.y + 1));
            }
            else if (matrix.At(currentPosition) == '^')
            {
                bool wasSplitted = false;
                if (currentPosition.x - 1 >= 0)
                {
                    queue.Enqueue((currentPosition.x - 1, currentPosition.y));
                    wasSplitted = true;
                }
                if (currentPosition.x + 1 < matrix.NumOfColumns)
                {
                    queue.Enqueue((currentPosition.x + 1, currentPosition.y));
                    wasSplitted = true;
                }
                if (wasSplitted)
                {
                    splitCount++;
                }
            }
        }

        return splitCount.ToString();
    }

    public string RunPartB(string inputData)
    {
        char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

        (int, int) start = matrix.FirstInstanceOf('S');

        Queue<Node> queue = [];
        Dictionary<(int, int), Node> visitedNodes = [];
        Node startNode = new(start, []);
        visitedNodes.Add(start, startNode);
        Dictionary<(int, int), long> memo = [];

        queue.Enqueue(startNode);

        void AddNextPosition((int, int) nextPosition, Node currentNode)
        {
            if (visitedNodes.TryGetValue(nextPosition, out Node? value))
            {
                value.Parents.Add(currentNode);
            }
            else
            {
                Node nextNode = new(nextPosition, [currentNode]);
                visitedNodes.Add(nextPosition, nextNode);
                queue.Enqueue(nextNode);
            }
        }

        long GetNumOfParents(Node node)
        {
            if (node.Parents.Count == 0) return 1;

            if (memo.TryGetValue(node.Position, out long value))
            {
                return value;
            }

            long count = 0;

            foreach (Node parent in node.Parents)
            {
                count += GetNumOfParents(parent);
            }

            memo.Add(node.Position, count);

            return count;
        }

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            (int x, int y) currentPosition = currentNode.Position;

            if ((matrix.At(currentPosition) == '.' || matrix.At(currentPosition) == 'S') && currentPosition.y + 1 < matrix.NumOfRows)
            {
                (int, int) nextPosition = (currentPosition.x, currentPosition.y + 1);
                AddNextPosition(nextPosition, currentNode);
            }
            else if (matrix.At(currentPosition) == '^')
            {
                if (currentPosition.x - 1 >= 0)
                {
                    (int, int) nextPosition = (currentPosition.x - 1, currentPosition.y);
                    AddNextPosition(nextPosition, currentNode);
                }
                if (currentPosition.x + 1 < matrix.NumOfColumns)
                {
                    (int, int) nextPosition = (currentPosition.x + 1, currentPosition.y);
                    AddNextPosition(nextPosition, currentNode);
                }
            }
        }

        return visitedNodes
            .Where(v => v.Key.Item2 + 1 == matrix.NumOfRows)
            .Select(v => GetNumOfParents(v.Value))
            .Sum()
            .ToString();
    }

    private class Node((int, int) position, HashSet<Node> parents)
    {
        public HashSet<Node> Parents = parents;
        public (int, int) Position = position;
    }
}
