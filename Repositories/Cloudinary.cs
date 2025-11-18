using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace MathSiteProject.Repositories.Storage;

public class CloudinaryStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _rootFolder;

    public CloudinaryStorageService()
    {
        var cloudName = Environment.GetEnvironmentVariable("CLOUD_NAME");
        var apiKey = Environment.GetEnvironmentVariable("CLOUD_API_KEY");
        var apiSecret = Environment.GetEnvironmentVariable("CLOUD_API_SECRET");
        _rootFolder = Environment.GetEnvironmentVariable("CLOUD_ROOT_FOLDER") ?? "MathSite";

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary の環境変数が正しく設定されていません。");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(string localPath, string studentName, string problemId, string fileName)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var publicId = $"{_rootFolder}/{studentName}/{problemId}/{fileName}/{date}";

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(localPath),
            PublicId = publicId,
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl?.ToString() ?? string.Empty;
    }

    public string GetPublicUrl(string studentName, string problemId, string fileName)
    {
        var publicId = $"{_rootFolder}/{studentName}/{problemId}/{fileName}";
    
        // 拡張子を指定する場合（例：.jpg）
        return _cloudinary.Api.UrlImgUp
            .Secure(true)
            .Transform(new Transformation()) // 変換なしでもOK
            .BuildUrl(publicId + ".jpg"); // 拡張子を明示するのがポイント！
    }

}
