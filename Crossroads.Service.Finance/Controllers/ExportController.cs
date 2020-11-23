using Crossroads.Service.Finance.Services.Exports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using ProcessLogging.Models;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [Route("journalentries/adjust")]
        public IActionResult AdjustJournalEntries()
        {
            try
            {
                _logger.Info("Creating journal entries");
                _exportService.CreateJournalEntriesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportController.AdjustJournalEntries: {ex.Message}");
                _logger.Error(ex, $"Error in ExportController.AdjustJournalEntries: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [Route("journalentries/hello")]
        public async Task<IActionResult> ExportHello()
        {
            try
            {
                await _exportService.HelloWorld();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportController.ExportHello: {ex.Message}");
                _logger.Error(ex, $"Error in ExportController.ExportHello: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [Route("journalentries/export")]
        public IActionResult ExportProgramatically()
        {
            try
            {
                _logger.Info("Exporting journal entries programatically");
                _exportService.ExportJournalEntries();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportController.ExportProgramatically: {ex.Message}");
                _logger.Error(ex, $"Error in ExportController.ExportProgramatically: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("journalentries/export/manual/{update}/{key}")]
        public async Task<ActionResult<string>> ExportManually(string key, bool update = true)
        {
            string result = String.Empty;
            var manualTestKey = Environment.GetEnvironmentVariable("MANUAL_TEST_KEY");

            if (manualTestKey != key)
            {
                return StatusCode(401);
            }

            try
            {
                _logger.Info("Exporting journal entries manually");
                var resultTask = _exportService.ExportJournalEntriesManually(update);
                 result = resultTask.Result;
                 return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportController.ExportManually: {ex.Message}");
                _logger.Error(ex, $"Error in ExportController.ExportManually: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
