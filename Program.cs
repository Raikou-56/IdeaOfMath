using MathSiteProject;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using MathSiteProject.Repositories.Interfaces;
using MathSiteProject.Repositories.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// DataProtection → デフォルトのファイルシステム保存に戻す
var keyPath = "/opt/render/keys";
if (!Directory.Exists(keyPath))
{
    Directory.CreateDirectory(keyPath);
    // 必要なら権限設定をここで行う（環境に応じて）
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName("MathSiteProject");

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.None
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Antiforgery 設定
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "__Host-AntiForgery";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// appsettings.jsonの読み込みを無視
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<AnswerHistoryRepository>();
builder.Services.AddScoped<ProblemRepository>();
builder.Services.AddScoped<IProblemService, ProblemService>();
builder.Services.AddTransient<CloudinaryStorageService>();
builder.Services.AddScoped<UserRepository>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

var forwardOptions = new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardOptions.KnownNetworks.Clear();
forwardOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ミドルウェア
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    DataBaseSetup.ShowUsers();
}
catch (Exception ex)
{
    Console.WriteLine($"MongoDB接続エラー: {ex.Message}");
}

app.Run();
