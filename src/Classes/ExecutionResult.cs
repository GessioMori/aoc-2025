namespace aoc_2025.Classes;

public enum ExecutionResultType
{
    Success,
    Failure,
}

public class ExecutionResult
{
    public string Result { get; set; } = string.Empty;
    public long ElapsedTimeInMs { get; set; }
    public ExecutionResultType ResultType { get; set; }
}
