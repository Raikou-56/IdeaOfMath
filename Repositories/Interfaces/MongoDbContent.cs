using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;

// データベース接続と操作のためのクラス 主にデータ編集
namespace MathSiteProject.Repositories.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGODB_URI")))
        {
            throw new InvalidOperationException("MONGODB_URI is not set.");
        }
        var client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        _database = client.GetDatabase("MathProjectDB");
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    public IMongoCollection<Problem> Problems => _database.GetCollection<Problem>("Problems");
}

public class AnswerHistoryRepository
{
    private readonly IMongoCollection<AnswerHistory> _collection;

    public AnswerHistoryRepository(MongoDbContext database)
    {
        _collection = database.GetCollection<AnswerHistory>("AnswerHistory");
    }

    public async Task<List<AnswerHistory>> GetHistoryByStudentIdAsync(string studentId)
    {
        if (string.IsNullOrWhiteSpace(studentId)) return new List<AnswerHistory>();
        var filter = Builders<AnswerHistory>.Filter.Eq(h => h.StudentId, studentId);
        var result = await _collection.Find(filter).ToListAsync();
        return result;
    }

    public async Task<List<AnswerHistory>> GetHistoryByStudentAndProblemAsync(string problemId, string studentId)
    {
        var filter = Builders<AnswerHistory>.Filter.And(
            Builders<AnswerHistory>.Filter.Eq(h => h.StudentId, studentId),
            Builders<AnswerHistory>.Filter.Eq(h => h.ProblemId, problemId)
        );

        var result = await _collection.Find(filter).SortByDescending(h => h.SolvedAt).ToListAsync();
        return result;
    }

    public async Task InsertAsync(AnswerHistory history)
    {
        await _collection.InsertOneAsync(history);
    }

    public async Task<AnswerHistory?> GetByIdAsync(string id)
    {
        var filter = Builders<AnswerHistory>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(AnswerHistory history)
    {
        var objectId = new ObjectId(history.Id);
        var filter = Builders<AnswerHistory>.Filter.Eq("_id", objectId);
        await _collection.ReplaceOneAsync(filter, history);
    }

    public async Task<HashSet<string>> GetUnscoredProblemIdsAsync()
    {
        var filter = Builders<AnswerHistory>.Filter.Eq(h => h.Scoring, false);
        var projection = Builders<AnswerHistory>.Projection.Include(h => h.ProblemId);
        var result = await _collection.Find(filter).Project<AnswerHistory>(projection).ToListAsync();
        return result.Select(h => h.ProblemId).OfType<string>().ToHashSet();
    }
}

public class ProblemRepository
{
    private readonly IMongoCollection<Problem> _collection;

    public ProblemRepository(MongoDbContext database)
    {
        _collection = database.Problems;
    }

    public async Task<List<Problem>> GetPagedProblems(int page, int limit)
    {
        return await _collection.Find(_ => true)
            .SortBy(p => p.SerialNumber)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
    }

    // 必要ならフィルター付きの取得も追加できるよ
}


