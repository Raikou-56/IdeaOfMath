using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;

namespace MathSiteProject.Repositories.Data;

public class UserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoDbContext database)
    {
        _collection = database.GetCollection<User>("Users");
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq("_id", user.UserId);
        await _collection.ReplaceOneAsync(filter, user);
    }

    // ユーザー名だけ更新したい場合
    public async Task UpdateUserNameAsync(string id, string newName)
    {
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
        var update = Builders<User>.Update.Set(u => u.Username, newName);
        await _collection.UpdateOneAsync(filter, update);
    }

    // 学年だけ更新したい場合
    public async Task UpdateGradeAsync(string id, string newGrade)
    {
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
        var update = Builders<User>.Update.Set(u => u.Grade, newGrade);
        await _collection.UpdateOneAsync(filter, update);
    }
}
