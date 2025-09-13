using System;

public static class Utility
{
    private static Random _random = new Random();

    public static bool FiftyFifty() => _random.Next(0, 2) == 0;

    /// <summary>
    /// Generates a random outcome based on a probability.
    /// </summary>
    /// <param name="probability">
    /// A value between 0.0 and 1.0 representing the likelihood of returning true.
    /// Values outside this range will be clamped to 0.0 or 1.0.
    /// </param>
    /// <returns>True with the specified probability; otherwise, false.</returns>
    public static bool RandomChance(double probability)
    {
        // Clamp the probability between 0.0 and 1.0
        probability = Math.Clamp(probability, 0.0, 1.0);

        // Generate a random value and compare it
        return _random.NextDouble() < probability;
    }
}