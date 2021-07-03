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
    private readonly IDescriptionRepository _repository;

    public DescriptionController(IDescriptionRepository repository)
    {
      this._repository = repository;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> GetBuildStatusAsync(string name)
    {
      try
      {
        var monster = await this._repository.QueryAsync(name);
        return Ok(new { monster });
      }
      catch (System.Exception ex)
      {
        return Ok(new { Status = "Failed", Message = ex.Message });
      }
    }
  }
}