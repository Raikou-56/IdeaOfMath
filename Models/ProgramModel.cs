using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MathSiteProject.Models;

public class Problem
{
    [BsonId]
    public int SerialNumber { get; set; }

    [BsonElement("idNumber")]
    public string? IdNumber { get; set; }

    [BsonElement("difficulty")]
    public string? difficulty { get; set; }

    [BsonElement("category")]
    public string? category { get; set; }

    [BsonElement("problem")]
    public string? ProblemLatex { get; set; }

    [BsonElement("answer")]
    public string? AnswerLatex { get; set; }

    [BsonElement("teacher")]
    public string? Teacher { get; set; }

    [BsonElement("published_at")]
    public DateTime? PublishedAt { get; set; }

    [BsonElement("is_public")]
    public bool IsPublic { get; set; }
}

public class User
{
    [BsonId]
    public string? UserId { get; set; }

    [BsonElement("userName")]
    public string? Username { get; set; }

    [BsonElement("grade")]
    public string? Grade { get; set; }

    [BsonElement("passwordHash")]
    public string? PassWordHash { get; set; }

    [BsonElement("Roles")]
    public string? Role { get; set; }
}

public class ProblemViewData
{
    public int SerialNumber { get; set; }
    public string? IdNumber { get; set; }

    public string? difficulty { get; set; }

    public string? category { get; set; }

    public string? LatexSrc { get; set; }

    public string? Score { get; set; }

    public bool UserData { get; set; }

    public string? Teacher { get; set; }

    public bool Scoring { get; set; }

    public bool Is_public { get; set; } = false;

    public DateTime? PublishedAt { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string? MailAddress { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string? MailAddress { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "パスワードは8文字以上である必要があります。")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "英字と数字の両方を含めてください。")]
    public string? Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "パスワードが一致しません。")]
    public string? ConfirmPassword { get; set; }

    [Required]
    public string? Role { get; set; }

    [Required]
    public string? Grade { get; set; }
}


public class TeacherViewModel
{
    public int[]? ProblemsCount { get; set; }
    public Dictionary<string, int>? FieldCounts { get; set; }
}

public class AnswerViewModel
{
    public int SerialNumber { get; set; }
    public string? Problem { get; set; }
    public string? Answer { get; set; }
    public string? Teacher { get; set; }
}

public class AnswerHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("studentId")]
    public string? StudentId { get; set; }

    [BsonElement("problemId")]
    public string? ProblemId { get; set; }

    [BsonElement("difficulty")]
    public string? Difficulty { get; set; }

    [BsonElement("solvedAt")]
    public DateTime SolvedAt { get; set; }

    [BsonElement("answer")]
    public List<string>? Answers { get; set; }

    [BsonElement("cloudinaryUrls")]
    public List<string>? CloudinaryUrls { get; set; }

    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; }

    [BsonElement("Scoring")]
    public bool Scoring { get; set; }

    [BsonElement("score")]
    public int? Score { get; set; }
}

public class UserViewModel
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
    public string? Grade { get; set; }
    public int[]? ScoresbyDifficulty { get; set; }
    public int[]? IntsAnsweredbyDifficulty { get; set; }
    public float[]? AverageScoresbyDifficulty { get; set; }
    public int TotalScores { get; set; }
}
