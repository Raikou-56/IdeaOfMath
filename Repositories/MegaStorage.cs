using CG.Web.MegaApiClient;

namespace MathSiteProject.Repositories.Mega;
public class MegaStorageService
{
    private readonly MegaApiClient _client;

    public MegaStorageService(string? email, string? password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Email and password must be provided.");
        }
        _client = new MegaApiClient();
        _client.Login(email, password);
    }

    public async Task<string> UploadFileAsync(string localPath, string fileName, INode destinationFolder)
    {
        using (var stream = File.OpenRead(localPath))
        {
            var node = await _client.UploadAsync(stream, fileName, destinationFolder);
            return node.Id; // または node.Name, node.Type など
        }
    }


    public async Task<INode> FolderMethodAsync(string studentName)
    {
        var nodes = _client.GetNodes();
        var root = nodes.Single(n => n.Type == NodeType.Root);

        // すでに存在するか確認
        var existing = nodes.FirstOrDefault(n => n.Type == NodeType.Directory && n.ParentId == root.Id && n.Name == studentName);
        if (existing != null)
        {
            return existing;
        }

        // なければ作成
        var newFolder = await _client.CreateFolderAsync(studentName, root);
        return newFolder;
    }


    public void Logout()
    {
        _client.Logout();
    }
}