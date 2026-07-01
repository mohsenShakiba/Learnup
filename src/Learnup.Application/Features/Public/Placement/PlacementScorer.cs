namespace Learnup.Application.Features.Public.Placement;

public static class PlacementScorer
{
    private const int RequiredCorrectPerBand = 3;

    /// <summary>
    /// Canonical CEFR bands in ascending order. Single source of truth for scoring and import
    /// validation.
    /// </summary>
    public static readonly string[] Bands = ["A1", "A2", "B1", "B2", "C1", "C2"];

    /// <summary>
    /// Placed level = the highest band that, together with every lower band, is passed
    /// (>= 3 of 4 correct). Stops at the first unpassed band. Defaults to A1.
    /// </summary>
    public static string Score(IReadOnlyDictionary<string, int> correctByBand)
    {
        var placed = Bands[0];

        foreach (var band in Bands)
        {
            var correct = correctByBand.TryGetValue(band, out var count) ? count : 0;

            if (correct >= RequiredCorrectPerBand)
            {
                placed = band;
            }
            else
            {
                break;
            }
        }

        return placed;
    }
}
