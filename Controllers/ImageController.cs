using Microsoft.AspNetCore.Mvc;
using MathSiteProject.Repositories.Storage;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly CloudinaryStorageService _storage;

    public ImageController(CloudinaryStorageService storage)
    {
        _storage = storage;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(
        [FromForm] IFormFile file,
        [FromForm] string teacherName,
        [FromForm] string problemId,
        [FromForm] string fileName)
    {
        try
        {
            Console.WriteLine(file);
            if (file == null)
            {
                return BadRequest("ファイルがありません");
            }
    
            var tempPath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(tempPath))
            {
                await file.CopyToAsync(stream);
            }
    
            var url = await _storage.UploadFileAsync(tempPath, teacherName, problemId, fileName);
            System.IO.File.Delete(tempPath);
            return Content(url);
        }
        catch (Exception ex)
        {
            // ログに出力（必要なら Console.WriteLine や ILogger を使って）
            return StatusCode(500, $"アップロード中にエラーが発生しました: {ex.Message}");
        }
    }

}
