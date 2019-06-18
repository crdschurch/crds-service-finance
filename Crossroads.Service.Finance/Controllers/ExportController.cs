using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
  [Route("api/Export")]
  public class ExportController : Controller
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