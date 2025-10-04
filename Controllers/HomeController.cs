using System.Diagnostics;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Models;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using MathSiteProject.Repositories.Storage;
using Microsoft.AspNetCore.Authorization;


namespace MathSiteProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AnswerHistoryRepository _answerHistoryRepo;

    public HomeController(ILogger<HomeController> logger, AnswerHistoryRepository answerHistoryRepo)
    {
        _logger = logger;
        _answerHistoryRepo = answerHistoryRepo;
    }

    public async Task<IActionResult> Index()
    {
        List<Problem> problemsData = DataBaseSetup.GetProblems();

        var problems = new List<ProblemViewData> { };
        var studentId = User.FindFirst("StudentId")?.Value;
        if (string.IsNullOrEmpty(studentId))
        {
            // ログインしてない or Claimがない場合の処理
            foreach (var problem in problemsData)
            {
                var problemData = new ProblemViewData
                {
                    SerialNumber = problem.SerialNumber,
                    IdNumber = problem.IdNumber,
                    difficulty = problem.difficulty,
                    category = problem.category,
                    LatexSrc = problem.ProblemLatex,
                    UserData = false,
                    Teacher = problem.Teacher
                };
                problems.Add(problemData);
            }
            return View(problems);
        }
        var AnswerHistories = DataBaseSetup.answerHistoryCollection();
        var historyList = await _answerHistoryRepo.GetHistoryByStudentIdAsync(studentId);
        var solvedIds = historyList.Select(h => h.ProblemId).ToHashSet();

        foreach (var problem in problemsData)
        {
            var problemData = new ProblemViewData
            {
                SerialNumber = problem.SerialNumber,
                IdNumber = problem.IdNumber,
                difficulty = problem.difficulty,
                category = problem.category,
                LatexSrc = problem.ProblemLatex,
                UserData = solvedIds.Contains(problem.SerialNumber.ToString()),
                Score = historyList
                            .Where(h => h.ProblemId == problem.SerialNumber.ToString() && h.Scoring)
                            .OrderByDescending(h => h.Score)
                            .FirstOrDefault()?.Score.ToString() ?? "未採点",
                Teacher = problem.Teacher,
                Scoring = HasUnscoredAnswers(problem.SerialNumber.ToString())
            };
            problems.Add(problemData);
        }
        return View(problems);
    }

    [Authorize(Roles = "Teacher")]
    public IActionResult Teacher()
    {
        var Tmodel = new TeacherViewModel
        {
            ProblemsCount = DataBaseSetup.CountProblems()
        };
        return View(Tmodel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAnswer(string problemId, IFormFile[] imageAnswers)
    {
        var studentId = User.FindFirst("StudentId")?.Value;
        var studentName = User.Identity?.Name ?? "UnknownStudent";
        if (string.IsNullOrEmpty(studentId)) return RedirectToAction("Login", "Account");

        var cloudinaryService = new CloudinaryStorageService(
            Environment.GetEnvironmentVariable("CLOUD_NAME"),
            Environment.GetEnvironmentVariable("CLOUD_API_KEY"),
            Environment.GetEnvironmentVariable("CLOUD_API_SECRET"),
            "MathSite"
        );

        int counter = 0;
        List<string> imageUrls = new List<string>();
        List<string> fileNames = new List<string>();

        foreach (var imageAnswer in imageAnswers)
        {
            var fileName = $"{problemId}_{counter}";

            // 一時保存（Renderのサーバー内）
            var tempPath = Path.Combine(Path.GetTempPath(), fileName + ".tmp");
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await imageAnswer.CopyToAsync(stream);
                }

                var imageUrl = await cloudinaryService.UploadFileAsync(tempPath, studentName, problemId, fileName);
                imageUrls.Add(imageUrl);
                fileNames.Add(fileName);

                counter += 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"画像アップロード失敗: {fileName}");
            }

            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath); // 一時ファイルを削除
        }

        var history = new AnswerHistory
        {
            StudentId = studentId,
            ProblemId = problemId,
            Answers = fileNames,
            CloudinaryUrls = imageUrls, // MegaNodeIds の代わりに Cloudinary の URL を保存
            IsCorrect = false,
            SolvedAt = DateTime.Now,
            Score = 0
        };

        await _answerHistoryRepo.InsertAsync(history);

        return RedirectToAction("Index");
    }


    
    
    public static bool HasUnscoredAnswers(string problemId)
    {
        var collection = DataBaseSetup.answerHistoryCollection();
        var relatedHistory = collection
            .AsQueryable()
            .Where(h => h.ProblemId == problemId);

        return relatedHistory.Any(h => !h.Scoring);
    }
}
