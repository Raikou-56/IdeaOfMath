using MongoDB.Driver;
using MongoDB.Bson;
using MathSiteProject;
using MathSiteProject.Repositories;
using MathSiteProject.Repositories.Data;
using MathSiteProject.Repositories.Interfaces;
using MathSiteProject.Repositories.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

var builder = WebApplication.CreateBuilder(args);

var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
var mongoDatabase = mongoClient.GetDatabase("MathProjectDB");
var keysCollection = mongoDatabase.GetCollection<BsonDocument>("DataProtectionKeys");

builder.Services.AddDataProtection()
    .SetApplicationName("MathProjectDB")
    .AddKeyManagementOptions(o =>
    {
        o.XmlRepository = new MongoXmlRepository(keysCollection);
        // 鍵のロールオーバー間隔など、必要ならここで調整可能
        // o.NewKeyLifetime = TimeSpan.FromDays(90);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// ログイン, Cookie
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// appsettings.jsonの読み込みを無視
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<AnswerHistoryRepository>();

builder.Services.AddScoped<ProblemRepository>();

builder.Services.AddScoped<IProblemService, ProblemService>();

builder.Services.AddTransient<CloudinaryStorageService>();

builder.Services.AddScoped<UserRepository>();


var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// ミドルウェア
// app.UseHttpsRedirection();
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
