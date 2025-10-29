using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;

namespace MathSiteProject.Repositories;

public class DataBaseSetup
{
    public static MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
    public static IMongoDatabase database = client.GetDatabase("MathProjectDB");

    // コレクションの取得
    public static IMongoCollection<User> userCollection()
    {
        return database.GetCollection<User>("Users");
    }
    public static IMongoCollection<Problem> problemCollection()
    {
        return database.GetCollection<Problem>("Problems");
    }

    public static IMongoCollection<AnswerHistory> answerHistoryCollection()
    {
        return database.GetCollection<AnswerHistory>("AnswerHistory");
    }

    // コレクションのリスト化
    public static List<Problem> GetProblems()
    {
        var collection = problemCollection();
        return collection
        .Find(FilterDefinition<Problem>.Empty)
        .SortBy(h => h.IdNumber)
        .ToList();
    }

    public static List<User> GetUsers()
    {
        return userCollection().Find(FilterDefinition<User>.Empty).ToList();
    }

    public static List<AnswerHistory> GetAnswerHistories()
    {
        var collection = answerHistoryCollection();
        return collection
        .Find(FilterDefinition<AnswerHistory>.Empty)
        .SortByDescending(h => h.SolvedAt)
        .ToList();
    }

    public static List<AnswerHistory> GetAnswerHistoryBystudentId(string studentId)
    {
        var collection = answerHistoryCollection();
        var filter = Builders<AnswerHistory>.Filter.Eq(h => h.StudentId, studentId);
        return collection.Find(filter).ToList();
    }

    public static void ShowUsers()
    {
        List<User> users = GetUsers();

        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.UserId}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"PasswordHash: **************************");
            Console.WriteLine($"Role: {user.Role}");
            Console.WriteLine("---------------------------");
        }
    }

    public static int CountProblems()
    {
        List<Problem> problems = GetProblems();
        int res = problems.Count();
        return res;
    }

    public static string? GetProblemDifficulty(string? problemId)
    {
        if (string.IsNullOrEmpty(problemId) || !int.TryParse(problemId, out var serial))
        {
            return null;
        }

        var collection = problemCollection();
        var filter = Builders<Problem>.Filter.Eq(p => p.SerialNumber, serial);
        var problem = collection.Find(filter).FirstOrDefault();

        return problem?.difficulty;
    }

    public static int[] GetScoresbyDifficulty(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new int[] { 0, 0, 0, 0, 0 };
        }
        var answerHistories = GetAnswerHistoryBystudentId(userId);
        int[] scoresByDifficulty = new int[5]; // 難易度1から5までのスコアを格納する配列

        foreach (var history in answerHistories)
        {
            switch (history.Difficulty)
            {
                case "A":
                    scoresByDifficulty[0] += history.Score ?? 0;
                    break;
                case "B":
                    scoresByDifficulty[1] += history.Score ?? 0;
                    break;
                case "C":
                    scoresByDifficulty[2] += history.Score ?? 0;
                    break;
                case "D":
                    scoresByDifficulty[3] += history.Score ?? 0;
                    break;
                case "E":
                    scoresByDifficulty[4] += history.Score ?? 0;
                    break;
                default:
                    break;
            }
        }
        return scoresByDifficulty;
    }
    
    public static int[] GetIntsAnsweredbyDifficulty(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new int[] { 0, 0, 0, 0, 0 };
        }
        var answerHistories = GetAnswerHistoryBystudentId(userId);
        int[] countsByDifficulty = new int[5]; // 難易度1から5までのスコアを格納する配列

        foreach (var history in answerHistories)
        {
            switch (history.Difficulty)
            {
                case "A":
                    countsByDifficulty[0] += 1;
                    break;
                case "B":
                    countsByDifficulty[1] += 1;
                    break;
                case "C":
                    countsByDifficulty[2] += 1;
                    break;
                case "D":
                    countsByDifficulty[3] += 1;
                    break;
                case "E":
                    countsByDifficulty[4] += 1;
                    break;
                default:
                    break;
            }
        }
        return countsByDifficulty;
    }

    public static float[] GetAverageScoresbyDifficulty(int[] scoresByDifficulty, int[] countsByDifficulty)
    {
        float[] averageScores = new float[5];
        for (int i = 0; i < 5; i++)
        {
            if (countsByDifficulty[i] > 0)
            {
                averageScores[i] = (float)scoresByDifficulty[i] / countsByDifficulty[i];
            }
            else
            {
                averageScores[i] = 0;
            }
        }
        return averageScores;

    }

    public static void InitializePublicFields()
    {
        var collection = problemCollection();

        // is_public フィールドが存在しないドキュメントを対象に更新
        var filter = Builders<Problem>.Filter.Exists("is_public", false);

        // is_public を false に、published_at を null に設定
        var update = Builders<Problem>.Update.Combine(
            Builders<Problem>.Update.Set("is_public", false),
            Builders<Problem>.Update.Set("published_at", BsonNull.Value)
        );
        Console.WriteLine("新フィールド追加完了");

        collection.UpdateMany(filter, update);
    }
    

}