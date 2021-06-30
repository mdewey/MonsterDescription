using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MonsterDescription.Services.Models;

namespace MonsterDescription.Services
{
  public class DescriptionParser
  {
    public HtmlDocument htmlDoc { get; set; } = new HtmlDocument();
    public List<HtmlNode> data { get; set; } = new List<HtmlNode>();
    public DescriptionParser(string html)
    {
      htmlDoc.LoadHtml(html);
      data = htmlDoc
        .DocumentNode
        .SelectSingleNode("//span[@id='ctl00_MainContent_DataListFeats_ctl00_Label1']")
        .ChildNodes
        .Where(w =>
        {
          return !String.IsNullOrEmpty(w.InnerHtml.Trim());
        }).Select(s => s).ToList();
    }

    public Description Parse()
    {
      var rv = new Description();
      var start = findStartIndex("Description");
      if (start.HasValue)
      {
        start++;
        while (data[start.GetValueOrDefault()].Name == "#text" || data[start.GetValueOrDefault()].Name == "i")
        {
          rv.Descriptions.Add(data[start.GetValueOrDefault()].InnerHtml);
          start++;
        }
      }
      else
      {
        rv.Descriptions.Add("No description found :-( ");
      }

      return rv;
    }

    public DescriptionParser ShowData()
    {
      var i = 0;
      foreach (var item in data)
      {
        Console.WriteLine($"{i}=>{item.InnerText} | name => {item.Name} | nodeType => {item.NodeType}");
        Console.WriteLine("-------");
        i++;
      }
      return this;
    }

    private int parseInt(string raw)
    {
      var rv = 0;
      var str = raw.Replace(",", "").Replace("+", "").Replace(";", "").Trim();
      Int32.TryParse(str, out rv);
      return rv;
    }

    private int? findStartIndex(string needle) => data
          .Select((item, index) => new { index, item, text = item.InnerHtml })
          .Where(w => w.text == needle).FirstOrDefault()?.index;
  }
}