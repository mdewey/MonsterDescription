using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonsterDescription.Controllers.Models;
using MonsterDescription.Services;
using MonsterDescription.Services.Models;
using Slack.NetStandard;
using Slack.NetStandard.Interaction;
using Slack.NetStandard.Messages.Blocks;

namespace MonsterDescription.Controllers
{
  [Route("slack/monster/description")]
  [ApiController]
  public class SlackController : ControllerBase
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

    [HttpPost]
    public async Task<ActionResult> GetBuildStatusAsync([FromForm] SlackRequest data)
    {

      try
      {
        var name = data.text;
        var url = $"https://aonprd.com/MonsterDisplay.aspx?ItemName={char.ToUpper(name[0]) + name.Substring(1)}";
        var monster = await GetMonster(url);
        monster.FullLink = url;
        monster.Name = name;
        var response = new
        {
          blocks = new List<Object>{
                new {
                    type = "header",
                    text= new {
                        type= "plain_text",
                        text= $":monster: {monster.Name} :monster:",
                        emoji= true
                    }
                },
                new {
                    type = "divider",
                },
            }
        };
        monster.Descriptions.ForEach(desc =>
        {
          response.blocks.Add(new
          {
            type = "section",
            text = new
            {
              type = "mrkdwn",
              text = desc
            }
          });
        });
        return Ok(new
        {
          blocks = response.blocks,
          response_type = "in_channel"
        });

      }
      catch (System.Exception ex)
      {
        var response = new
        {
          blocks = new List<Object>{
                new {
                    type = "header",
                    text= new {
                        type= "plain_text",
                        text= $":cowboy: Well looky here partner :clown:",
                        emoji= true
                    }
                },
                new {
                    type = "divider",
                },
                new {
                  type = "section",
                  text = new
                  {
                    type = "mrkdwn",
                    text = "Hey rodeo clown, check the logs for more"
                  }
                },
                new {
                  type = "section",
                  text = new
                  {
                    type = "mrkdwn",
                    text = ex.Message
                  }
                }
            }
        };

        return Ok(new
        {
          blocks = response.blocks,
          response_type = "in_channel"
        });

      }
    }
  }
}