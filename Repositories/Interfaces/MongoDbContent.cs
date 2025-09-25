using MongoDB.Driver;
using MathSiteProject.Models;

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
        var filter = Builders<AnswerHistory>.Filter.Eq(h => h.StudentId, studentId);
        var result = await _collection.Find(filter).ToListAsync();
        return result;
    }

    public async Task InsertAsync(AnswerHistory history)
    {
        await _collection.InsertOneAsync(history);
    }

}
