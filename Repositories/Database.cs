using MongoDB.Driver;
using MathSiteProject.Models;

namespace MathSiteProject.Repositories;

public class DataBaseSetup
{
    public static MongoClient client = new MongoClient("mongodb+srv://raikoukit777_db_user:mathSite-at@mathuserdataonc.si9g8gl.mongodb.net/MathProjectDB?retryWrites=true&w=majority&appName=MathUserDataonC");
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
        return problemCollection().Find(FilterDefinition<Problem>.Empty).ToList();
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


}