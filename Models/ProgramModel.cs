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
}

public class User
{
    [BsonId]
    public string? UserId { get; set; }

    [BsonElement("userName")]
    public string? Username { get; set; }

    [BsonElement("passwordHash")]
    public string? PassWordHash { get; set; }

    [BsonElement("SolvedList")]
    public List<bool>? SolvedList { get; set; }

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

    public bool UserData { get; set; }

    public string? Teacher { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string? MailAddress { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }
}

public class TeacherViewModel
{
    public int ProblemsCount { get; set; }
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

    [BsonElement("problemId")] // フォルダ名は問題IDに対応
    public string? ProblemId { get; set; }

    [BsonElement("solvedAt")]
    public DateTime SolvedAt { get; set; }

    [BsonElement("answer")]
    public string? Answer { get; set; }

    [BsonElement("megaNodeId")]
    public string? MegaNodeId { get; set; }

    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; }

    [BsonElement("Scoring")]
    public bool Scoring { get; set; }

    [BsonElement("score")]
    public int? Score { get; set; }
}
