using FuzzySharp;

namespace Shared.Infrastructure.Rest;

public static class FuzzySearch
{
    public static bool IsMatch(string first, string second, int threshold = 85)
    {
        return Fuzz.Ratio(first.ToLower(), second.ToLower()) > threshold;
    }

    public static bool Contains(string first, string second, int threshold = 85)
    {
        return Fuzz.PartialRatio(first.ToLower(), second.ToLower()) > threshold;
    }
}