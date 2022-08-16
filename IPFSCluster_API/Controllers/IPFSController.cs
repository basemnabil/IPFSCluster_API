using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPFSCluster_API.Controllers;

[ApiController]
[Route("api/ipfs/[controller]")]
public class IpfsClientController : ControllerBase
{
    private static readonly HttpClient Client = new ();

    private const string IpDefault = "172.18.0.5";
    private static string _httpResponseMessage; 
    
    // GET
    [HttpGet]
    public async Task<ActionResult> GetId()
    {
        await Id(IpDefault);
        return Ok(_httpResponseMessage);
    }
    
    [HttpGet("{ip}")]
    public async Task<ActionResult> GetId(string ip)
    {
        await Id(ip);
        return Ok(_httpResponseMessage);
    }

    private static async Task Id(string ip)
    {
        Client.DefaultRequestHeaders.Accept.Clear();
        var httpRequestMessage = "http://" + ip + ":9094/id";
        var stringTask = Client.GetStringAsync(httpRequestMessage);
        _httpResponseMessage = await stringTask;
    }
    
}