using System.Net;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace IPFSCluster_API.Controllers;

[ApiController]
[Route("api/ipfs/[controller]")]
public class FileStatusController : ControllerBase
{
    private static readonly HttpClient Client = new ();

    private const string IpDefault = "172.18.0.5";

    //GET
    [HttpGet]
    public async Task<ActionResult> GetFilesStatus()
    {
        var message = await Files(IpDefault);
        return Ok(message);
    }
    
    [HttpGet("{ip}")]
    public async Task<ActionResult> GetFilesStatus(string ip)
    {
        var message = await Files(ip);
        return Ok(message);
    }

    [HttpGet("file")]
    public async Task<ActionResult> GetFileStatus(string id)
    {
        var message = await Files(IpDefault, id);
        return Ok(message);
    }
    
    [HttpGet("{ip}/file")]
    public async Task<ActionResult> GetFileStatus(string ip, string id)
    {
        var message = await Files(ip, id);
        return Ok(message);
    }
    

    private async Task<string> Files(string ip, [Optional] string id)
    {
        Client.DefaultRequestHeaders.Accept.Clear();
        var httpRequestMessage = "http://" + ip + ":9094/pins";
        if (!String.IsNullOrEmpty(id))
            httpRequestMessage += "/" + id;
        return await (Client.GetStringAsync(httpRequestMessage));

    }
}