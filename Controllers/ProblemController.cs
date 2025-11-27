using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Models;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;

namespace MathSiteProject.Controllers;

public class ProblemController : Controller
{
    private readonly IMongoCollection<Problem> _problemCollection;

    public ProblemController(MongoDbContext context)
    {
        _problemCollection = context.Problems;
    }
    public IActionResult SaveProblem()
    {
        ViewBag.CategoryList = new SelectList(new List<string>
        {
            // 数IA
            "場合の数",
            "確率",
            "整数",
            "集合と論理",

            // 数IIB
            "微積",
            "数列",
            "図形と方程式",
            "ベクトル",

            // 数IIIC
            "極限",
            "微積",
            "複素数平面",
            "行列"
        });
        return View();
    }

    public IActionResult EditProblem(string serial)
    {
        List<Problem> problemsData = DataBaseSetup.GetProblems();
        Console.WriteLine(problemsData.Count);

        if (int.TryParse(serial.ToString(), out int number))
        {
            var match = problemsData.FirstOrDefault(p => p.SerialNumber == number);
            if (match != null)
            {
                ViewBag.CategoryList = new SelectList(new List<string>
                {
                    // 数IA
                    "場合の数",
                    "確率",
                    "整数",
                    "集合と論理",

                    // 数IIB
                    "微積",
                    "数列",
                    "図形と方程式",
                    "ベクトル",

                    // 数IIIC
                    "極限",
                    "微積",
                    "複素数平面",
                    "行列"
                }, match.category);
                return View(match);
            }
            else
            {
                return View("Index", "home");
            }
        }
        else
        {
            Console.WriteLine("通し番号の変換に失敗しました");
            return View("Index", "home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Problem updatedItem)
    {
        var filter = Builders<Problem>.Filter.Eq(p => p.SerialNumber, updatedItem.SerialNumber);
        var existing = await _problemCollection.Find(filter).FirstOrDefaultAsync();
    
        if (existing == null) return NotFound();
    
        // 基本情報の更新
        existing.IdNumber = updatedItem.IdNumber;
        existing.difficulty = updatedItem.difficulty;
        existing.category = updatedItem.category;
        existing.ProblemLatex = updatedItem.ProblemLatex;
        existing.AnswerLatex = updatedItem.AnswerLatex;
        existing.Teacher = updatedItem.Teacher;
    
        // 公開状態の更新
        bool wasPublic = existing.IsPublic;
        existing.IsPublic = updatedItem.IsPublic;
    
        // 初めて公開された場合、公開日時を記録
        if (!wasPublic && updatedItem.IsPublic && existing.PublishedAt == null)
        {
            existing.PublishedAt = DateTime.UtcNow;
        }
    
        await _problemCollection.ReplaceOneAsync(filter, existing);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> AutoSave([FromBody] Problem updatedItem)
    {
        var filter = Builders<Problem>.Filter.Eq(p => p.SerialNumber, updatedItem.SerialNumber);
        var existing = await _problemCollection.Find(filter).FirstOrDefaultAsync();
    
        if (existing == null) return NotFound();
    
        // 必要な項目だけ更新（必要に応じて絞ってもOK）
        existing.IdNumber = updatedItem.IdNumber;
        existing.difficulty = updatedItem.difficulty;
        existing.category = updatedItem.category;
        existing.ProblemLatex = updatedItem.ProblemLatex;
        existing.AnswerLatex = updatedItem.AnswerLatex;
        existing.Teacher = updatedItem.Teacher;
    
        await _problemCollection.ReplaceOneAsync(filter, existing);
    
        return Ok(new { message = "AutoSave successful" });
    }

    [HttpPost]
    public IActionResult SaveProb(Problem saveItem)
    {
        IMongoCollection<Problem> problemCollection = DataBaseSetup.problemCollection();

        var newProblem = new Problem
        {
            SerialNumber = DataBaseSetup.CountProblems() + 1,
            IdNumber = saveItem.IdNumber,
            difficulty = saveItem.difficulty,
            category = saveItem.category,
            ProblemLatex = saveItem.ProblemLatex,
            AnswerLatex = saveItem.AnswerLatex,
            Teacher = saveItem.Teacher
        };

        problemCollection.InsertOne(newProblem);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult ScoringPage(string serial)
    {
        List<AnswerHistory> allHistories = DataBaseSetup.GetAnswerHistories();
        ViewBag.Message = serial;

        var filteredHistories = allHistories
        .Where(h => h.ProblemId != null && h.ProblemId.ToString() == serial)
        .ToList();

        return View(filteredHistories);
    }

    [HttpPost]
    public IActionResult ScoringAnswer(string answerId)
    {
        var history = DataBaseSetup
        .GetAnswerHistories()
        .FirstOrDefault(x => x.Id == answerId);
        if (history == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(history);
    }

    
}
