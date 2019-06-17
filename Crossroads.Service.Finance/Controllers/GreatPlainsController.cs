using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
  [Route("api/GreatPlains")]
  public class GreatPlainsController : Controller
  {
    [HttpGet]
    [Route("adjustjournalentries")]
    public IActionResult AdjustJournalEntries()
    {
      return Ok();
    }
  }
}