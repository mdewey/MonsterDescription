using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonsterDescription.Services;
using MonsterDescription.Services.Models;

namespace MonsterDescription.Controllers
{
  [Route("api/[controller]")]
  [ApiController]


  public class DescriptionController : ControllerBase
  {
    static readonly HttpClient client = new HttpClient();

    private async Task<Description> GetMonster(string url)
    {
      Console.WriteLine(url);
      var response = await client.GetAsync(url);
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"The data source return a {response.StatusCode} for {url}");
      }
      string responseBody = await response.Content.ReadAsStringAsync();
      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(responseBody);

      var description = new DescriptionParser(responseBody).ShowData().Parse();

      return description;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> GetBuildStatusAsync(string name)
    {
      try
      {
        var query = String.Join(' ', name.Split(" ").Select(s => char.ToUpper(s[0]) + s.Substring(1)));
        var url = $"https://aonprd.com/MonsterDisplay.aspx?ItemName={query}";
        var monster = await GetMonster(url);
        monster.FullLink = url;
        monster.Name = name;
        return Ok(new { monster });
      }
      catch (System.Exception ex)
      {
        return Ok(new { Status = "Failed", Message = ex.Message });
      }
    }
  }
}