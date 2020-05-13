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
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class ExportController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IExportService _exportService;
        private readonly IProcessLogger _processLogger;

        public ExportController(IExportService exportService, IProcessLogger processLogger)
        {
            _exportService = exportService;
            _processLogger = processLogger;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [Route("journalentries/adjust")]
        public IActionResult AdjustJournalEntries()
        {
            try
            {
                var creatingJournalEntriesMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.createJournalEntries)
                {
                    MessageData = "Creating journal entries"
                };
                _processLogger.SaveProcessLogMessage(creatingJournalEntriesMessage);

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
                var runningExportMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.exportJournalEntries)
                {
                    MessageData = "Exporting journal entries programatically"
                };
                _processLogger.SaveProcessLogMessage(runningExportMessage);

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
                var manualJournalEntryExportMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.manualJournalEntryExport)
                {
                    MessageData = "Exporting journal entries manually"
                };
                _processLogger.SaveProcessLogMessage(manualJournalEntryExportMessage);

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
