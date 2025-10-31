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
    public async Task<IActionResult> UploadImage(IFormFile file, string studentName, string problemId, string fileName)
    {
        var tempPath = Path.GetTempFileName();
        using (var stream = System.IO.File.Create(tempPath))
        {
            await file.CopyToAsync(stream);
        }

        var url = await _storage.UploadFileAsync(tempPath, studentName, problemId, fileName);
        System.IO.File.Delete(tempPath);
        return Content(url);
    }
}
