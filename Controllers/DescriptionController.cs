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
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(responseBody);

      var description = new DescriptionParser(responseBody).ShowData().Parse();

      return description;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> GetBuildStatusAsync(string name)
    {
      var url = $"https://aonprd.com/MonsterDisplay.aspx?ItemName={char.ToUpper(name[0]) + name.Substring(1)}";
      var monster = await GetMonster(url);
      monster.FullLink = url;
      monster.Name = name;
      return Ok(new { monster });
    }
  }
}