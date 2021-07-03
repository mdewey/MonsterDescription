using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MonsterDescription.Services.Models;

namespace MonsterDescription.Services
{
  public interface IDescriptionRepository
  {
    Task<Description> QueryAsync(string creature);
  }

  public class DescriptionRepository : IDescriptionRepository
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
    public async Task<Description> QueryAsync(string creature)
    {

      var query = String.Join(' ', creature.Split(" ").Select(s => char.ToUpper(s[0]) + s.Substring(1)));
      var url = $"https://aonprd.com/MonsterDisplay.aspx?ItemName={query}";
      var monster = await GetMonster(url);
      monster.FullLink = url;
      monster.Name = creature;
      return monster;
    }
  }
}