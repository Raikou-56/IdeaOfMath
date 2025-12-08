using System.Security.Cryptography;

namespace MathSiteProject.Extentions;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}

public static class ArrayExtensions
{
    public static int TotalScores(int[] scoresByDifficulty)
    {
        return scoresByDifficulty.Sum();
    }
}

public static class ScoreExtensions
{
    public static string? GetBadgeImage(int score)
    {
        if (score >= 2000) return "/img/badges/rank5.png";
        if (score >= 1000) return "/img/badges/rank4.png";
        if (score >= 500) return "/img/badges/rank3.png";
        if (score >= 200) return "/img/badges/rank2.png";
        return null;
    }

    public static string GetScoreColorClass(int score)
    {
        if (score >= 2000) return "score-gold";
        if (score >= 1000) return "score-purple";
        if (score >= 500)  return "score-blue";
        if (score >= 200)  return "score-green";
        return "score-silver";
    }
}