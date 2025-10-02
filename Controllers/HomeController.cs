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
                Scoring = historyList
                            .Where(h => h.ProblemId == problem.SerialNumber.ToString())
                            .All(h => !h.Scoring)
            };
            Console.WriteLine(problemData.Scoring);
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

    [HttpPost]
    public IActionResult LookAnswer(string serial)
    {
        ViewBag.Message = serial;
        List<Problem> problemsData = DataBaseSetup.GetProblems();

        if (int.TryParse(serial.ToString(), out int number))
        {
            var match = problemsData.FirstOrDefault(p => p.SerialNumber == number);
            if (match != null)
            {
                var AnswerInf = new AnswerViewModel
                {
                    SerialNumber = number,
                    Problem = match.ProblemLatex,
                    Answer = match.AnswerLatex,
                    Teacher = match.Teacher
                };
                return View(AnswerInf);
            }
            else
            {
                var AnswerInf = new AnswerViewModel
                {
                    SerialNumber = number,
                    Problem = "問題が存在しません",
                    Answer = "解答が存在しません"
                };
                return View(AnswerInf);
            }
        }
        else
        {
            Console.WriteLine("通し番号の変換に失敗しました");
            return View("Home", "Index");
        }
    }

    [HttpPost]
    public IActionResult SendAnswer(string serial)
    {
        ViewBag.Message = serial;
        return View();
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


    [HttpPost]
    public async Task<IActionResult> ScoreAnswer(string historyId, int Score)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("モデルが無効です");
        }
        var history = await _answerHistoryRepo.GetByIdAsync(historyId);
        if (history == null) return NotFound($"ID {historyId} の履歴が見つかりません");

        Console.WriteLine(Score);

        // スコアの保存（ここは設計次第で柔軟に）
        history.Scoring = true; 
        history.Score = Score; // 合計でも平均でもOK
        history.IsCorrect = Score == 50; // 仮の判定ロジック
    
        await _answerHistoryRepo.UpdateAsync(history);
    
        return RedirectToAction("Index"); // 採点後の遷移先
    }


    [Authorize]
    public IActionResult MyPage()
    {
        return View();
    }

}
