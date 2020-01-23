using Crossroads.Service.Finance.Services.Exports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                _exportService.CreateJournalEntriesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating journal entries: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("journalentries/hello")]
        public async Task<IActionResult> ExportHello()
        {
            try
            {
                _logger.Info("Running hello world...");
                await _exportService.HelloWorld();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error running hello world: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("journalentries/export")]
        public IActionResult ExportProgramatically()
        {
            try
            {
                _logger.Info("Running export...");
                _exportService.ExportJournalEntries();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("journalentries/export/manual/{update}")]
        public async Task<ActionResult<string>> ExportManually(bool update = true)
        {
            string result = String.Empty; 

            try
            {
                _logger.Info("Running export...");
                 var resultTask = _exportService.ExportJournalEntriesManually(update);
                 result = resultTask.Result;
                 return Ok(result);
            }
            catch (Exception ex)
            {
                var x = 1;
                return BadRequest(ex.Message);
            }

            //return Ok(result); 
        }
    }
}
