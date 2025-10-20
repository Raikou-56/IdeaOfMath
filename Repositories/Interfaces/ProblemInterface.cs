using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;
using MathSiteProject.Repositories.Data;

namespace MathSiteProject.Repositories.Interfaces;

public interface IProblemService
{
    Task<List<ProblemViewData>> GetPagedProblems(int page, int limit, string? studentId);
}

public class ProblemService : IProblemService
{
    private readonly ProblemRepository _repository;
    private readonly AnswerHistoryRepository _answerHistoryRepo;

    public ProblemService(ProblemRepository repository, AnswerHistoryRepository answerHistoryRepo)
    {
        _repository = repository;
        _answerHistoryRepo = answerHistoryRepo;
    }

    public async Task<List<ProblemViewData>> GetPagedProblems(int page, int limit, string? studentId)
    {
        var AnswerHistories = DataBaseSetup.GetAnswerHistories();
        var historyList = await _answerHistoryRepo.GetHistoryByStudentIdAsync(studentId ?? "");
        var solvedIds = historyList.Select(h => h.ProblemId).ToHashSet();
        var unscoredMap = AnswerHistories.Where(h => !h.Scoring).Select(h => h.ProblemId).ToHashSet();
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
                UserData = solvedIds.Contains(p.SerialNumber.ToString()),
                Score = historyList
                    .Where(h => h.ProblemId == p.SerialNumber.ToString() && h.Scoring)
                    .OrderByDescending(h => h.Score)
                    .FirstOrDefault()?.Score?.ToString() ?? "未採点",
                Scoring = unscoredMap.Contains(p.SerialNumber.ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ProblemServiceエラー: " + ex.Message);
            throw;
        }
    }
}

