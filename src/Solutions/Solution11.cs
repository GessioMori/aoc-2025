using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution11 : ISolution
{
    public string RunPartA(string inputData)
    {
        Dictionary<string, Node> nodes = ParseInput(inputData);
        Dictionary<string, Dictionary<string, long>> memo = [];

        Dictionary<string, long> numOfPathsToOut = GetNumOfPathsToNode(nodes["out"], memo);

        return numOfPathsToOut["you"].ToString();
    }

    public string RunPartB(string inputData)
    {
        Dictionary<string, Node> nodes = ParseInput(inputData);
        Dictionary<string, Dictionary<string, long>> memo = [];

        long numofPaths = 0;

        // svr -> fft -> dac -> out
        long svrToFFt = GetNumOfPathsToNode(nodes["fft"], memo).TryGetValue("svr", out long v) ? v : 0;
        long fftToDac = GetNumOfPathsToNode(nodes["dac"], memo).TryGetValue("fft", out v) ? v : 0;
        long dacToOut = GetNumOfPathsToNode(nodes["out"], memo).TryGetValue("dac", out v) ? v : 0;

        numofPaths += svrToFFt * fftToDac * dacToOut;

        // svr -> dac -> fft -> out
        long svrToDac = GetNumOfPathsToNode(nodes["dac"], memo).TryGetValue("svr", out v) ? v : 0;
        long dacToFft = GetNumOfPathsToNode(nodes["fft"], memo).TryGetValue("dac", out v) ? v : 0;
        long fftToOut = GetNumOfPathsToNode(nodes["out"], memo).TryGetValue("fft", out v) ? v : 0;

        numofPaths += svrToDac * dacToFft * fftToOut;

        return numofPaths.ToString();
    }

    private static Dictionary<string, long> GetNumOfPathsToNode(Node node, Dictionary<string, Dictionary<string, long>> memo)
    {
        if (memo.TryGetValue(node.Label, out Dictionary<string, long>? value))
        {
            return value;
        }

        List<(string, Dictionary<string, long>)> parentsDictsList = [];

        foreach (Node parent in node.Parents)
        {
            parentsDictsList.Add((parent.Label, GetNumOfPathsToNode(parent, memo)));
        }

        Dictionary<string, long> result = MergeNumOfPathsDictionaries(parentsDictsList);
        memo.Add(node.Label, result);
        return result;
    }

    private static Dictionary<string, long> MergeNumOfPathsDictionaries(List<(string, Dictionary<string, long>)> parentsDictsList)
    {
        Dictionary<string, long> result = [];

        foreach ((string, Dictionary<string, long>) parentDict in parentsDictsList)
        {
            if (result.ContainsKey(parentDict.Item1))
            {
                result[parentDict.Item1] += 1;
            }
            else
            {
                result.Add(parentDict.Item1, 1);
            }

            foreach (KeyValuePair<string, long> ancestorsDict in parentDict.Item2)
            {
                if (result.ContainsKey(ancestorsDict.Key))
                {
                    result[ancestorsDict.Key] += ancestorsDict.Value;
                }
                else
                {
                    result.Add(ancestorsDict.Key, ancestorsDict.Value);
                }
            }
        }

        return result;
    }

    private static Dictionary<string, Node> ParseInput(string inputData)
    {
        Dictionary<string, Node> nodes = [];
        string[] lines = ParseUtils
            .ParseIntoLines(inputData)
            .ToList()
            .Append("out:")
            .ToArray();

        foreach (string line in lines)
        {
            string nodeLabel = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
            nodes.Add(nodeLabel, new Node() { Label = nodeLabel });
        }

        foreach (string line in lines)
        {
            string[] parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[] nodeNeighbors = parts.Length > 1 ?
                parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : [];

            foreach (string neighbors in nodeNeighbors)
            {
                nodes[neighbors].Parents.Add(nodes[parts[0]]);
            }
        }

        return nodes;
    }

    private class Node()
    {
        public string Label = string.Empty;
        public List<Node> Parents = [];
    }
}