using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;

namespace MathSiteProject.Repositories.Interfaces;

public interface IProblemService
{
    List<ProblemViewData> GetPagedProblems(int page, int limit);
}

public class ProblemService : IProblemService
{
    private readonly IMongoCollection<ProblemViewData>? _collection;

    public List<ProblemViewData> GetPagedProblems(int page, int limit)
    {
        return _collection.Find(_ => true)
        .Skip((page - 1) * limit)
        .Limit(limit)
        .ToList();
    }
}

