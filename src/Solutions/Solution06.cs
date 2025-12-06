using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution06 : ISolution
{
    private record struct Operation(long[] Numbers, char Operand);

    public string RunPartA(string inputData)
    {
        Operation[] operations = ParseOperationsA(inputData);

        long total = operations.Aggregate(0L, (acc, v) => acc + Calculate(v));

        return total.ToString();
    }

    public string RunPartB(string inputData)
    {
        Operation[] operations = ParseOperationsB(inputData);

        long total = operations.Aggregate(0L, (acc, v) => acc + Calculate(v));

        return total.ToString();
    }

    private static long Calculate(Operation operation)
    {
        if (operation.Operand == '+')
        {
            return operation.Numbers.Sum();
        }

        return operation.Numbers.Aggregate(1L, (acc, v) => acc * v);
    }

    private static Operation[] ParseOperationsA(string inputData)
    {
        string[] lines = ParseUtils.ParseIntoLines(inputData);
        List<char> operands = [];
        List<List<long>> values = [];
        List<Operation> operations = [];

        foreach (string line in lines)
        {
            if (line.Contains('+'))
            {
                operands.AddRange(line
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => s[0]));
            }
            else
            {
                values.Add(line
                    .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToList());
            }
        }

        for (int i = 0; i < operands.Count; i++)
        {
            List<long> operationValues = [];

            for (int j = 0; j < values.Count; j++)
            {
                operationValues.Add(values[j][i]);
            }

            operations.Add(new Operation(operationValues.ToArray(), operands[i]));
        }

        return operations.ToArray();
    }

    private static Operation[] ParseOperationsB(string inputData)
    {
        string[] lines = ParseUtils.ParseIntoLines(inputData);

        List<Operation> operations = [];

        char[] operands = lines.Last().Replace(" ", string.Empty).ToCharArray();

        int lastLineIdx = inputData.LastIndexOf('\n');

        string numberLines = NormalizeLineLengths(inputData[..lastLineIdx].TrimEnd('\r'));

        char[][] numberMatrix = MatrixUtils.CreateCharMatrix(numberLines);

        IEnumerable<long> numberColumns = MatrixUtils
            .GetAllCharMatrixColumns(numberMatrix)
            .Select(c =>
            {
                string trimmedColumnString = c.AxisString.Replace(" ", string.Empty);

                if (string.IsNullOrEmpty(trimmedColumnString)) return 0L;

                return long.Parse(trimmedColumnString);
            })
            .Append(0);

        int operationCount = 0;

        List<long> currentNumbers = [];

        foreach (long number in numberColumns)
        {
            if (number == 0)
            {
                operations.Add(new Operation(currentNumbers.ToArray(), operands[operationCount]));
                currentNumbers.Clear();
                operationCount++;
            }
            else
            {
                currentNumbers.Add(number);
            }
        }

        return operations.ToArray();
    }

    private static string NormalizeLineLengths(string input)
    {
        string[] lines = input.Split('\n');

        int maxLength = lines.Max(l => l.EndsWith('\r')
            ? l.Length - 1
            : l.Length);

        return string.Join('\n',
            lines.Select(l =>
            {
                string line = l.TrimEnd('\r');
                return line.PadRight(maxLength) + (l.EndsWith('\r') ? "\r" : "");
            }));
    }
}