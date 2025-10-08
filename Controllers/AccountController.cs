using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Extentions;
using MathSiteProject.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using MathSiteProject.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MathSiteProject.Controllers;

public class AccountController : Controller
{
    public IActionResult NewUser()
    {
        return View();
    }
    
    // 会員登録
    [HttpPost]
    public IActionResult Register(string mailaddress, string username, string password, string role)
    {
        IMongoCollection<User> users = DataBaseSetup.userCollection();

        var newUser = new User
        {
            UserId = mailaddress,
            Username = username,
            PassWordHash = SecurityHelper.HashPassword(password),
            Role = role
        };

        users.InsertOne(newUser);

        return RedirectToAction("Login", "Account");  // 登録後にログインページへ
    }

    // ログイン
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        IMongoCollection<User> users = DataBaseSetup.userCollection();

        var user = users.Find(u => u.UserId == model.MailAddress).FirstOrDefault();

        if (user == null || model.Password == null || user.PassWordHash != SecurityHelper.HashPassword(model.Password))
        {
            ModelState.AddModelError(string.Empty, "ユーザーIDまたはパスワードが違います");
            return View(model);
        }

        if (user == null || user.UserId == null || user.PassWordHash != SecurityHelper.HashPassword(model.Password))
        {
            ModelState.AddModelError("", "ユーザーIDまたはパスワードが違います");
            return View(model);
        }

        if (user.Role == null)
        {
            ModelState.AddModelError("", "登録区分が登録されてません運営に確認してください。");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username ?? user.UserId),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("StudentId", user.UserId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return user.Role switch
        {
            "Student" => RedirectToAction("Index", "Home"),
            "Teacher" => RedirectToAction("Teacher", "Home"),
            "Admin" => RedirectToAction("Teacher", "Home"),
            _         => RedirectToAction("Login", "Account"),
        };
    }

    [Authorize]
    public IActionResult MyPage()
    {
        IMongoCollection<User> users = DataBaseSetup.userCollection();
        var user = users.Find(u => u.UserId == User.FindFirstValue("StudentId")).FirstOrDefault();

        var model = new UserViewModel
        {
            UserId = user?.UserId,
            UserName = user?.Username,
            Role = user?.Role,
            Grade = user?.Grade,
            TotalScores = DataBaseSetup.GetTotalScores(user?.UserId)
        };
        return View(model);
    }

    // ログアウト
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}