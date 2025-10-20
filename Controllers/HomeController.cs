using System.Diagnostics;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Models;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using MathSiteProject.Repositories.Storage;
using MathSiteProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace MathSiteProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AnswerHistoryRepository _answerHistoryRepo;
    private readonly IProblemService _problemService;

    public HomeController(ILogger<HomeController> logger,
                        AnswerHistoryRepository answerHistoryRepo,
                        IProblemService problemService)
    {
        _logger = logger;
        _answerHistoryRepo = answerHistoryRepo;
        _problemService = problemService;
    }

    public IActionResult Index()
    {
        return View();
    }

    // index.cshtmlの問題読み込みメソッド
    [HttpGet]
    public async Task<IActionResult> GetProblems(int page = 1, int limit = 3, string? studentId = null)
    {
        try
        {
            var problems = _problemService.GetPagedProblems(page, limit, studentId);
            return Json(problems);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ProblemServiceエラー: " + ex.Message);
            return StatusCode(500, $"GetProblemsでエラー発生: {ex.Message}");
        }
    }

    [Authorize(Roles = "Teacher,Admin")]
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
        var difficulty = DataBaseSetup.GetProblemDifficulty(problemId);
        var studentName = User.Identity?.Name ?? "UnknownStudent";
        if (string.IsNullOrEmpty(studentId)) return RedirectToAction("Login", "Account");

        var cloudName = Environment.GetEnvironmentVariable("CLOUD_NAME");
        var apiKey = Environment.GetEnvironmentVariable("CLOUD_API_KEY");
        var apiSecret = Environment.GetEnvironmentVariable("CLOUD_API_SECRET");
        
        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary の環境変数が正しく設定されていません。");
        }
        
        var cloudinaryService = new CloudinaryStorageService(
            cloudName,
            apiKey,
            apiSecret,
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
            Difficulty = difficulty,
            CloudinaryUrls = imageUrls, // MegaNodeIds の代わりに Cloudinary の URL を保存
            IsCorrect = false,
            SolvedAt = DateTime.Now,
            Score = 0
        };

        await _answerHistoryRepo.InsertAsync(history);

        return RedirectToAction("Index");
    }


    
    
    
}
