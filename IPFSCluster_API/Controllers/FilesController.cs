using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace IPFSCluster_API.Controllers;

[ApiController]
[Route("api/ipfs/[controller]")]
public class FilesController : ControllerBase
{
    private static readonly HttpClient Client = new ();
    private const string IpDefault = "172.18.0.5";
    
    //GET
    [HttpGet("view")]
    public async Task<ActionResult> ViewFile(string id)
    {
        string nodeIp = GetNodeIp(IpDefault).Result.ToString();
        var message = await File(nodeIp, id);
        return Ok(message);
    }
    [HttpGet("view/{ip}")]
    public async Task<ActionResult> ViewFile(string ip, string id)
    {
        string nodeIp = GetNodeIp(ip).Result.ToString();
        var message = await File(nodeIp, id);
        return Ok(message);
    }

    [HttpPost("add")]
    public async Task<ActionResult> PostFile(string path, int replicaMin = -1, int replicaMax = -1)
    {
        
        var message = await AddFile(IpDefault,path,replicaMin,replicaMax);
        return Ok(message);
    }
    
    [HttpPost("add/{ip}")]
    public async Task<ActionResult> PostFile(string ip, string path,int replicaMin = -1, int replicaMax = -1)
    {
        var message = await AddFile(ip,path,replicaMin,replicaMax);
        return Ok(message);
    }

    [HttpPost("get")]
    public async Task<ActionResult> GetFile(string id, string path)
    {
        string nodeIp = GetNodeIp(IpDefault).Result.ToString();
        var message = await DownloadFile(nodeIp, id, path);
        return Ok(message);
    }
    
    [HttpPost("get/{ip}")]
    public async Task<ActionResult> GetFile(string ip, string id, string path)
    {
        string nodeIp = GetNodeIp(ip).Result.ToString();
        var message = await DownloadFile(nodeIp, id, path);
        return Ok(message);
    }

    /*[HttpGet("find")]
    public async Task<ActionResult> GetId()
    {
        var message = await GetNodeIp(IpDefault);
        return Ok(message.ToString());
    }
    
    [HttpGet("find/{ip}")]
    public async Task<ActionResult> GetId(string ip)
    {
        var message = await GetNodeIp(ip);
        return Ok(message);
    }
    */

    private async Task<string> File(string ip, string id)
    {
        Client.DefaultRequestHeaders.Accept.Clear();
        var result = await Client.PostAsync("http://" + ip + ":5001/api/v0/cat?arg=" + id, null);
        Console.Write(result.Headers);
        return await result.Content.ReadAsStringAsync();
    }

    private async Task<string> AddFile(string ip, string path,int replicaMin, int replicaMax)
    {
        var fileStreamContent = new StreamContent(System.IO.File.OpenRead(path));
        var content = new MultipartFormDataContent();
        content.Add(fileStreamContent);
        var result = await Client.PostAsync("http://" + ip + ":9094/add?replication-min=" + replicaMin + "&replication-max=" + replicaMax, content);
        return await result.Content.ReadAsStringAsync();
    }

    private async Task<string> DownloadFile(string ip, string id, string path)
    {
        var result = await Client.PostAsync("http://" + ip + ":5001/api/v0/get?arg=" + id, null);
        await using (var fs = new FileStream(@"" + path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await result.Content.CopyToAsync(fs);
        }
        return "File saved in path: " + path;
    }
    
    private async Task<Object> GetNodeIp(string ip)
    {
        IpfsClientController ipfs = new IpfsClientController();
        var data = (OkObjectResult)ipfs.GetId().Result;
        dynamic result = JsonConvert.DeserializeObject(data.Value.ToString());
        string addrs = result["ipfs"]["addresses"][1].ToString();
        var addr = addrs.Split("/");
        return (addr[2]);
    }

    /*
    private File ExtractTar(Stream stream)
    {
        var buffer = new byte[100];
        while (true)
        {
            stream.Read(buffer, 0, 100);
            var name = Encoding.ASCII.GetString(buffer).Trim('\0');
            if (String.IsNullOrWhiteSpace(name))
                break;
            stream.Seek(24, SeekOrigin.Current);
            stream.Read(buffer, 0, 12);
            var size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, 12).Trim(), 8);

            stream.Seek(376L, SeekOrigin.Current);

            //var output = Path.Combine(outputDir, name);
            if (!Directory.Exists(Path.GetDirectoryName(output)))
                Directory.CreateDirectory(Path.GetDirectoryName(output));
            using (var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var buf = new byte[size];
                stream.Read(buf, 0, buf.Length);
                str.Write(buf, 0, buf.Length);
            }

            var pos = stream.Position;

            var offset = 512 - (pos  % 512);
            if (offset == 512)
                offset = 0;

            stream.Seek(offset, SeekOrigin.Current);
        }
    }
    */
}