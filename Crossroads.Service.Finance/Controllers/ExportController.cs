using System;
using Crossroads.Service.Finance.Services.Exports;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class ExportController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpPost]
        [Route("journalentries/adjust")]
        public IActionResult AdjustJournalEntries()
        {
            try
            {
                _logger.Info("Running adjust journal entries job...");
                _exportService.CreateJournalEntries();
                return Ok();
            }
            catch (Exception ex)
            {
                var msg = "DonorController: GetRecurringGifts";
                _logger.Error($"Error creating journal entries: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}