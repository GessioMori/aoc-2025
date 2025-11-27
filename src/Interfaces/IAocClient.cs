using aoc_2025.AocClient;

namespace aoc_2025.Interfaces;

public interface IAocClient
{
    Task<ClientResponse> GetPuzzleInput(int dayNumber);
}
