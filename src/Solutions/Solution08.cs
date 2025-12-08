using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using Spectre.Console;

namespace aoc_2025.Solutions;

public class Solution08 : ISolution
{
    private record struct Junction(int PosX, int PosY, int PosZ);

    private readonly int numOfConnections = 1000;
    private readonly int numOfLargestCircuits = 3;

    public string RunPartA(string inputData)
    {
        List<Junction> junctions = GetJunctionsList(inputData);

        (List<HashSet<Junction>> clusters, Dictionary<Junction, int> junctionClusterIndexes) = InitClusters(junctions);

        List<(Junction A, Junction B)> orderedJunctionPairsByDistance = GetDistances(junctions)
            .OrderBy(pairDistance => pairDistance.D)
            .Select(pairDistance => (pairDistance.A, pairDistance.B))
            .Take(this.numOfConnections)
            .ToList();

        for (int i = 0; i < this.numOfConnections; i++)
        {
            (Junction junctionA, Junction junctionB) = orderedJunctionPairsByDistance[i];

            int junctionAClusterIndex = junctionClusterIndexes[junctionA];
            int junctionBClusterIndex = junctionClusterIndexes[junctionB];

            if (junctionAClusterIndex == junctionBClusterIndex)
            {
                continue;
            }

            MergeClusters(junctionClusterIndexes, clusters, junctionAClusterIndex, junctionBClusterIndex);
        }

        return clusters
            .OrderByDescending(c => c.Count)
            .Take(this.numOfLargestCircuits)
            .Aggregate(1, (acc, c) => acc * c.Count)
            .ToString();
    }

    public string RunPartB(string inputData)
    {
        List<Junction> junctions = GetJunctionsList(inputData);

        (List<HashSet<Junction>> clusters, Dictionary<Junction, int> map) = InitClusters(junctions);

        List<(Junction A, Junction B)> orderedJunctionPairsByDistance = GetDistances(junctions)
            .OrderBy(pairDistance => pairDistance.D)
            .Select(pairDistance => (pairDistance.A, pairDistance.B))
            .ToList();

        foreach ((Junction junctionA, Junction junctionB) in orderedJunctionPairsByDistance)
        {
            int junctionAClusterIndex = map[junctionA];
            int junctionBClusterIndex = map[junctionB];

            if (junctionAClusterIndex == junctionBClusterIndex)
            {
                continue;
            }

            MergeClusters(map, clusters, junctionAClusterIndex, junctionBClusterIndex);

            if ((clusters[junctionAClusterIndex].Count == junctions.Count) || (clusters[junctionBClusterIndex].Count == junctions.Count))
            {
                long posXProduct = junctionA.PosX * junctionB.PosX;
                return posXProduct.ToString();
            }
        }

        throw new InvalidOperationException();
    }

    private static void MergeClusters(Dictionary<Junction, int> junctionClusterIndexes, List<HashSet<Junction>> clusters, int from, int into)
    {
        foreach (Junction jb in clusters[from])
        {
            junctionClusterIndexes[jb] = into;
        }

        clusters[into].UnionWith(clusters[from]);
        clusters[from].Clear();
    }

    static (List<HashSet<Junction>>, Dictionary<Junction, int>) InitClusters(List<Junction> junctions)
    {
        List<HashSet<Junction>> clusters = [];
        Dictionary<Junction, int> map = [];

        for (int i = 0; i < junctions.Count; i++)
        {
            clusters.Add([junctions[i]]);
            map[junctions[i]] = i;
        }

        return (clusters, map);
    }

    private static List<Junction> GetJunctionsList(string inputData)
    {
        return ParseUtils.ParseIntoLines(inputData)
            .Select(line =>
            {
                string[] parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return new Junction(
                    int.Parse(parts[0]),
                    int.Parse(parts[1]),
                    int.Parse(parts[2]));
            })
            .ToList();
    }

    private static IEnumerable<(Junction A, Junction B, double D)> GetDistances(List<Junction> junctionBoxes)
    {
        for (int i = 0; i < junctionBoxes.Count; i++)
        {
            for (int j = i + 1; j < junctionBoxes.Count; j++)
            {
                double dx = junctionBoxes[i].PosX - junctionBoxes[j].PosX;
                double dy = junctionBoxes[i].PosY - junctionBoxes[j].PosY;
                double dz = junctionBoxes[i].PosZ - junctionBoxes[j].PosZ;
                yield return (junctionBoxes[i], junctionBoxes[j], Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz)));
            }
        }
    }
}