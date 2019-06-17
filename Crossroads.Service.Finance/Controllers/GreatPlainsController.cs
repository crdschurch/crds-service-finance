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

    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    [HttpGet]
    [Route("adjustjournalentries")]
    public IActionResult AdjustJournalEntries()
    {
      _logger.Info("Running adjust journal entries job...");
      return Ok();
    }
  }
}