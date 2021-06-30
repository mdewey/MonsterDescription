using System.Collections.Generic;

namespace MonsterDescription.Services.Models
{
  public class Description
  {
    public string Name { get; set; }

    public string FullLink { get; set; }

    public List<string> Descriptions { get; set; } = new List<string>();

  }
}