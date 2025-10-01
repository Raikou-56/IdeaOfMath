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

    public async Task<INode> GetOrCreateRootFolderAsync(string rootName = "MathSite")
    {
        var nodes = _client.GetNodes();
        var root = nodes.Single(n => n.Type == NodeType.Root);

        var existing = nodes.FirstOrDefault(n =>
            n.Type == NodeType.Directory &&
            n.ParentId == root.Id &&
            n.Name == rootName);

        if (existing != null)
            return existing;

        var newFolder = await _client.CreateFolderAsync(rootName, root);
        return newFolder;
    }

    public async Task<INode> GetOrCreateAnswerFolderAsync(string studentName, string problemId)
    {
        var mathSiteFolder = await GetOrCreateRootFolderAsync();

        // 生徒フォルダの取得 or 作成
        var studentFolder = await StudentFolderMethodAsync(studentName);

        var nodes = _client.GetNodes();
        var existing = nodes.FirstOrDefault(n =>
            n.Type == NodeType.Directory &&
            n.ParentId == studentFolder.Id &&
            n.Name == problemId);

        if (existing != null)
            return existing;

        var newFolder = await _client.CreateFolderAsync(problemId, studentFolder);
        return newFolder;
    }


    public async Task<INode> StudentFolderMethodAsync(string studentName)
    {
        var mathSiteFolder = await GetOrCreateRootFolderAsync();

        var nodes = _client.GetNodes();
        var existing = nodes.FirstOrDefault(n =>
            n.Type == NodeType.Directory &&
            n.ParentId == mathSiteFolder.Id &&
            n.Name == studentName);

        if (existing != null)
            return existing;

        var newFolder = await _client.CreateFolderAsync(studentName, mathSiteFolder);
        return newFolder;
    }

    public async Task<INode> AnswerFolderMethodAsync(string studentName, string problemId)
    {
        var mathSiteFolder = await GetOrCreateRootFolderAsync();
    
        // 生徒フォルダの取得 or 作成
        var studentFolder = await StudentFolderMethodAsync(studentName);
    
        var nodes = _client.GetNodes();
        var existing = nodes.FirstOrDefault(n =>
            n.Type == NodeType.Directory &&
            n.ParentId == studentFolder.Id &&
            n.Name == problemId);
    
        if (existing != null)
            return existing;
    
        var newFolder = await _client.CreateFolderAsync(problemId, studentFolder);
        return newFolder;
    }



    public void Logout()
    {
        _client.Logout();
    }
}