using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Extentions;
using MathSiteProject.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using Microsoft.AspNetCore.Authorization;


namespace MathSiteProject.Controllers;

public class AnswerController : Controller
{
    private readonly AnswerHistoryRepository _answerHistoryRepo;

    public AnswerController(AnswerHistoryRepository answerHistoryRepo)
    {
        _answerHistoryRepo = answerHistoryRepo;
    }

    [HttpPost]
    public IActionResult SendAnswer(string serial)
    {
        ViewBag.Message = serial;
        return View();
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

        return RedirectToAction("Index", "Home"); // 採点後の遷移先
    }

    [HttpPost]
    public IActionResult CheckAnswer(string serial, string studentId)
    {
        var history = DataBaseSetup
        .GetAnswerHistories()
        .Where(h => h.ProblemId == serial && h.StudentId == studentId)
        .OrderByDescending(h => h.SolvedAt) // 時間のプロパティ名に合わせてね
        .FirstOrDefault();
        if (history == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(history);
    }
}