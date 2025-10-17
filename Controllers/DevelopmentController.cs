using System.Diagnostics;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Models;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using MathSiteProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MathSiteProject.Controllers;

public class DevelopmentController : Controller
{
    private readonly IProblemService _problemService;

    public DevelopmentController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    public IActionResult TestPage()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetProblems(int page = 1, int limit = 5)
    {
        try
        {
            var problems = _problemService.GetPagedProblems(page, limit);
            return Json(problems);
        }
        catch (Exception ex)
        {
            return Content($"GetProblemsでエラー発生: {ex.Message}");
        }
    }


}