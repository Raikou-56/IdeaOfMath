using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;
using MathSiteProject.Repositories.Data;

namespace MathSiteProject.Repositories.Interfaces;

public interface IProblemService
{
    List<ProblemViewData> GetPagedProblems(int page, int limit);
}

public class ProblemService : IProblemService
{
    private readonly ProblemRepository _repository;

    public ProblemService(ProblemRepository repository)
    {
        _repository = repository;
    }

    public List<ProblemViewData> GetPagedProblems(int page, int limit)
    {
        try
        {
            var problems = _repository.GetPagedProblems(page, limit);
            return problems.Select(p => new ProblemViewData
            {
                SerialNumber = p.SerialNumber,
                IdNumber = p.IdNumber,
                category = p.category,
                difficulty = p.difficulty,
                LatexSrc = p.ProblemLatex,
                UserData = false,
                Score = "0",
                Scoring = false
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ProblemServiceエラー: " + ex.Message);
            throw;
        }
    }
}

