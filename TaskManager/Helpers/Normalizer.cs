namespace TaskManager.Helpers;

public class NameNormalizer
{
    public static string Normalize(string name) =>
        name?.Trim().ToLowerInvariant() ?? string.Empty;
}