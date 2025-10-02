using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace MathSiteProject.Repositories.Storage;

public class CloudinaryStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _rootFolder;

    public CloudinaryStorageService(string cloudName, string apiKey, string apiSecret, string rootFolder = "MathSite")
    {
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _rootFolder = rootFolder;
    }

    public async Task<string> UploadFileAsync(string localPath, string studentName, string problemId, string fileName)
    {
        var publicId = $"{_rootFolder}/{studentName}/{problemId}/{fileName}";

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
