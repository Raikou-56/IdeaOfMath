using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject.Models;
using MathSiteProject.Repositories.Data;

namespace MathSiteProject.Repositories.Interfaces;

public interface IProblemService
{
    Task<List<ProblemViewData>> GetPagedProblemsAsync(int page, int limit, string? studentId);
}

public class ProblemService : IProblemService
{
    private readonly ProblemRepository _repository;
    private readonly AnswerHistoryRepository _answerHistoryRepo;
    
    public ProblemService(ProblemRepository repository,
    AnswerHistoryRepository answerHistoryRepo)
    {
        _repository = repository;
        _answerHistoryRepo = answerHistoryRepo;
    }

    public async Task<List<ProblemViewData>> GetPagedProblemsAsync(int page, int limit, string? studentId)
    {
        try
        {
            // 並列で履歴と未採点データを取得
            var historyTask = _answerHistoryRepo.GetHistoryByStudentIdAsync(studentId ?? "");
            var unscoredTask = _answerHistoryRepo.GetUnscoredProblemIdsAsync();
            var problemsTask = _repository.GetPagedProblems(page, limit);

            await Task.WhenAll(historyTask, unscoredTask, problemsTask);

            var historyList = historyTask.Result;
            var unscoredMap = unscoredTask.Result;
            var problems = problemsTask.Result;

            var solvedIds = historyList.Select(h => h.ProblemId).ToHashSet();

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
                Scoring = unscoredMap.Contains(p.SerialNumber.ToString()),
                Is_public = p.IsPublic,
                PublishedAt = p.PublishedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ProblemServiceエラー: " + ex.Message);
            throw;
        }
    }

    
}

