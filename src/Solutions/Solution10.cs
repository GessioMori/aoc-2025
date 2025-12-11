using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using Google.OrTools.LinearSolver;
using System.Numerics;
using System.Text.RegularExpressions;

namespace aoc_2025.Solutions;

public partial class Solution10 : ISolution
{
    private readonly record struct Machine(bool[] TargetLights, List<bool[]> Buttons, List<int> Joltage);

    public string RunPartA(string inputData)
    {
        int total = 0;

        List<Machine> machines = ParseInput(inputData);

        foreach (Machine machine in machines)
        {
            total += GetMinPressesForMachine(machine);
        }

        return total.ToString();
    }

    public string RunPartB(string inputData)
    {
        int total = 0;

        List<Machine> machines = ParseInput(inputData);

        foreach (Machine machine in machines)
        {
            total += SolveLinearExpression(machine);
        }

        return total.ToString();
    }

    private static int SolveLinearExpression(Machine machine)
    {
        Solver solver = Solver.CreateSolver("SCIP");

        int numOfVariables = machine.Buttons.Count;
        int numOfEquations = machine.Joltage.Count;

        Variable[] variables = Enumerable.Range(0, numOfVariables)
                                  .Select(i => solver.MakeIntVar(0, int.MaxValue, $"x{i}"))
                                  .ToArray();

        for (int j = 0; j < numOfEquations; j++)
        {
            LinearExpr expr = new();
            for (int i = 0; i < numOfVariables; i++)
            {
                if (machine.Buttons[i][j])
                {
                    expr += variables[i];
                }
            }
            solver.Add(expr == machine.Joltage[j]);
        }

        LinearExpr objective = new();
        for (int i = 0; i < numOfVariables; i++)
        {
            objective += variables[i];
        }

        solver.Minimize(objective);

        solver.Solve();

        return (int)solver.Objective().Value();
    }

    private static int GetMinPressesForMachine(Machine machine)
    {
        foreach (bool[] combination in GenerateBoolCombinations(machine.Buttons.Count))
        {
            bool isCombinationValid = true;

            for (int i = 0; i < machine.TargetLights.Length; i++)
            {
                bool isLightOn = false;

                for (int j = 0; j < machine.Buttons.Count; j++)
                {
                    if (combination[j] && machine.Buttons[j][i])
                    {
                        isLightOn = !isLightOn;
                    }
                }

                if (isLightOn != machine.TargetLights[i])
                {
                    isCombinationValid = false;
                    break;
                }
            }

            if (isCombinationValid)
            {
                return combination.Sum(c => c ? 1 : 0);
            }
        }

        throw new Exception("Invalid combinations");
    }

    private static List<Machine> ParseInput(string inputData)
    {
        return ParseUtils.ParseIntoLines(inputData)
            .Select(ParseLine)
            .ToList();
    }

    private static IEnumerable<bool[]> GenerateBoolCombinations(int n)
    {
        for (int k = 0; k <= n; k++)
        {
            for (int j = 0; j < (1 << n); j++)
            {
                if (BitOperations.PopCount((uint)j) != k)
                {
                    continue;
                }

                bool[] bits = new bool[n];
                for (int i = 0; i < n; i++)
                {
                    bits[n - 1 - i] = ((j >> i) & 1) == 1;
                }

                yield return bits;
            }
        }
    }

    private static Machine ParseLine(string line)
    {
        Match match = MachineRegex().Match(line);
        if (!match.Success)
        {
            throw new FormatException("Invalid line format.");
        }

        string targetsRaw = match.Groups["targets"].Value;
        bool[] targetLights = targetsRaw.Select(c => c == '#').ToArray();

        MatchCollection buttonMatches = ButtonRegex().Matches(match.Groups["buttons"].Value);
        List<bool[]> buttons = new(buttonMatches.Count);

        foreach (Match bm in buttonMatches)
        {
            bool[] button = new bool[targetLights.Length];
            foreach (int index in bm.Groups[1].Value.Split(',').Select(int.Parse))
            {
                button[index] = true;
            }
            buttons.Add(button);
        }

        List<int> joltage = match.Groups["joltage"]
            .Value
            .Split(',')
            .Select(int.Parse)
            .ToList();

        return new Machine(targetLights, buttons, joltage);
    }

    [GeneratedRegex(@"^\[(?<targets>[.#]+)\]\s+(?<buttons>(\([0-9,]+\)\s*)+)\{(?<joltage>[0-9,]+)\}$", RegexOptions.Compiled)]
    private static partial Regex MachineRegex();

    [GeneratedRegex(@"\(([0-9,]+)\)")]
    private static partial Regex ButtonRegex();
}