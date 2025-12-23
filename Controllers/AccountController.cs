using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Extentions;
using MathSiteProject.Models;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MathSiteProject.Controllers;

public class AccountController : Controller
{
    private readonly UserRepository _userRepository;

    public AccountController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public IActionResult NewUser()
    {
        return View();
    }

    // 会員登録
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        IMongoCollection<User> users = DataBaseSetup.userCollection();

        if (!ModelState.IsValid)
        {
            return View("NewUser", model);
        }

        var existingUser = users.Find(u => u.UserId == model.MailAddress).FirstOrDefault();
        if (existingUser != null)
        {
            ModelState.AddModelError("MailAddress", "このメールアドレスはすでに登録されています。");
            return View("NewUser", model);
        }

        if (model.Password == null)
        {
            ModelState.AddModelError("Password", "パスワードを入力してください。");
            return View("NewUser", model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "パスワードが一致しません。");
            return View("NewUser", model);
        }
        var newUser = new User
        {
            UserId = model.MailAddress,
            Username = model.Username,
            PassWordHash = SecurityHelper.HashPassword(model.Password),
            Role = model.Role,
            Grade = model.Grade
        };

        users.InsertOne(newUser);

        return RedirectToAction("Login", "Account");  // 登録後にログインページへ
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var users = DataBaseSetup.userCollection();
        var user = users.Find(u => u.UserId == model.MailAddress).FirstOrDefault();

        // ユーザー存在チェック & パスワード検証
        if (user == null || model.Password == null || user.PassWordHash != SecurityHelper.HashPassword(model.Password)) 
        { ModelState.AddModelError(string.Empty, "ユーザーIDまたはパスワードが違います"); 
            return View(model); 
        } 
        if (user == null || user.UserId == null || user.PassWordHash != SecurityHelper.HashPassword(model.Password)) 
        { ModelState.AddModelError("", "ユーザーIDまたはパスワードが違います"); 
            return View(model); 
        }

        // Role 未設定チェック
        if (string.IsNullOrEmpty(user.Role))
        {
            ModelState.AddModelError("", "登録区分が登録されていません。運営に確認してください。");
            return View(model);
        }

        // クレーム作成
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username ?? user.UserId),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.UserId ?? "")
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        // Cookie 認証でサインイン
        await HttpContext.SignInAsync("Cookies", principal);
        
        // Role に応じてリダイレクト
        return user.Role switch
        {
            "Student" => RedirectToAction("Index", "Home"),
            "Teacher" => RedirectToAction("Teacher", "Home"),
            "Admin"   => RedirectToAction("Teacher", "Home"),
            _         => RedirectToAction("Login", "Account"),
        };
    }

    [Authorize]
    public IActionResult MyPage()
    {
        IMongoCollection<User> users = DataBaseSetup.userCollection();
        var user = users.Find(u => u.UserId == User.FindFirstValue("UserId")).FirstOrDefault();
        if (user == null)
        {
            return RedirectToAction("Error", new { message = "ユーザー情報が取得できませんでした。" });
        }

        var scoresByDifficulty = DataBaseSetup.GetScoresbyDifficulty(user?.UserId);
        var countsByDifficulty = DataBaseSetup.GetIntsAnsweredbyDifficulty(user?.UserId);

        var model = new UserViewModel
        {
            UserId = user?.UserId,
            UserName = user?.Username,
            Role = user?.Role,
            Grade = user?.Grade,
            ScoresbyDifficulty = scoresByDifficulty,
            IntsAnsweredbyDifficulty = countsByDifficulty,
            AverageScoresbyDifficulty = DataBaseSetup
                .GetAverageScoresbyDifficulty(scoresByDifficulty, countsByDifficulty),
            TotalScores = ArrayExtensions.TotalScores(scoresByDifficulty),
            GradeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "中学生以下", Text = "中学生以下" },
                new SelectListItem { Value = "高校1年生", Text = "高校1年生" },
                new SelectListItem { Value = "高校2年生", Text = "高校2年生" },
                new SelectListItem { Value = "高校3年生", Text = "高校3年生" },
                new SelectListItem { Value = "浪人生", Text = "浪人生" },
                new SelectListItem { Value = "大学生以上", Text = "大学生以上" }
            }
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UserViewModel model)
    {
        var user = await _userRepository.GetByIdAsync(model.UserId ?? "");
        if (user == null) return NotFound();

        user.Username = model.UserName;
        user.Grade = model.Grade;

        await _userRepository.UpdateAsync(user);

        return RedirectToAction("MyPage");
    }

    // Admin用ユーザー編集
    [HttpPost]
    public async Task<IActionResult> EditUser(User user)
    {
        // RepositoryのUpdateAsyncを呼ぶ
        await _userRepository.UpdateAsync(user);

        // 更新後はAdminPageにリダイレクト
        return RedirectToAction("Index", "Home");
    }

    // ログアウト
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}