using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution11 : ISolution
{
    public string RunPartA(string inputData)
    {
        Dictionary<string, Node> nodes = ParseInput(inputData);
        Dictionary<Node, long> memo = [];

        return GetNumOfPathsFromTo(nodes["out"], nodes["you"], memo).ToString();
    }

    public string RunPartB(string inputData)
    {
        Dictionary<string, Node> nodes = ParseInput(inputData);

        long numofPaths = 0;

        // svr -> fft -> dac -> out
        long svrToFFt = GetNumOfPathsFromTo(nodes["fft"], nodes["svr"], []);
        long fftToDac = GetNumOfPathsFromTo(nodes["dac"], nodes["fft"], []);
        long dacToOut = GetNumOfPathsFromTo(nodes["out"], nodes["dac"], []);

        numofPaths += svrToFFt * fftToDac * dacToOut;

        // svr -> dac -> fft -> out
        long svrToDac = GetNumOfPathsFromTo(nodes["dac"], nodes["svr"], []);
        long dacToFft = GetNumOfPathsFromTo(nodes["fft"], nodes["dac"], []);
        long fftToOut = GetNumOfPathsFromTo(nodes["out"], nodes["fft"], []);

        numofPaths += svrToDac * dacToFft * fftToOut;

        return numofPaths.ToString();
    }

    private static long GetNumOfPathsFromTo(Node origin, Node target, Dictionary<Node, long> memo)
    {
        if (origin == target)
        {
            return 1;
        }

        if (memo.TryGetValue(origin, out long value))
        {
            return value;
        }

        long totalPathsFromOrigin = 0;

        foreach (Node parent in origin.Parents)
        {
            totalPathsFromOrigin += GetNumOfPathsFromTo(parent, target, memo);
        }

        memo.Add(origin, totalPathsFromOrigin);
        return totalPathsFromOrigin;
    }

    private static Dictionary<string, Node> ParseInput(string inputData)
    {
        Dictionary<string, Node> nodes = new(StringComparer.Ordinal);

        string[] lines = ParseUtils
            .ParseIntoLines(inputData)
            .Append("out:")
            .ToArray();

        foreach (string? line in lines)
        {
            string label = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
            nodes[label] = new Node(label);
        }

        foreach (string? line in lines)
        {
            string[] parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string label = parts[0];

            string[] neighbors = parts.Length > 1
                ? parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : [];

            foreach (string neighbor in neighbors)
            {
                nodes[neighbor].Parents.Add(nodes[label]);
            }
        }

        return nodes;
    }

    private class Node(string label)
    {
        public string Label { get; } = label;
        public List<Node> Parents { get; } = [];
    }
}