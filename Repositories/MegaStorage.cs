using CG.Web.MegaApiClient;

namespace MathSiteProject.Repositories.Mega;
public class MegaUploader
{
    public void LoginToMega()
    {
        var client = new MegaApiClient();
        client.Login("mathsiteteam@gmail.com", "mathsite-mega");

        Console.WriteLine("ログイン成功！");
    }
}
