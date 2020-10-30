using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProcessLogging.Models;
using Crossroads.Web.Common.Auth.Helpers;

namespace Crossroads.Service.Finance.Controllers
{
    [RequiresAuthorization]
    [Route("api/[controller]")]
    public class PushpayController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly INewPushpayService _pushpayService;
        private readonly IRecurringService _recurringService;
        private readonly IDonationService _donationService;

        public PushpayController(INewPushpayService pushpayService, IRecurringService recurringService)
        {
            _pushpayService = pushpayService;
            _recurringService = recurringService;
        }

        [HttpPost("updaterecurringgifts")]
        public async Task<IActionResult> UpdateRecurringGiftsAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.Info($"UpdateRecurringGiftsAsync is starting.  Start Date: {startDate}, End Date: {endDate}");
                await _pushpayService.PullRecurringGiftsAsync(startDate, endDate);
                _logger.Info("All update jsons are saved to the DB");
                _logger.Info("Starting to process the updates");
                await _recurringService.SyncRecurringSchedules();
                _logger.Info("UpdateRecurringGiftsAsync is complete.");
                return Ok();
            }

            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in PushpayController.UpdateRecurringGiftsAsync: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("updatedonations")]
        public async Task<IActionResult> UpdateDonationsAsync()
        {
	        try
	        {
		        var lastSuccessfulRunTime = string.Empty;

		        using (var reader = new StreamReader(Request.Body))
		        {
			        var body = await reader.ReadToEndAsync();

			        lastSuccessfulRunTime = JObject.Parse(body)["lastSuccessfulRunTime"].ToString();
		        }

		        if (string.IsNullOrEmpty(lastSuccessfulRunTime))
		        {
			        throw new Exception("Donation sync missing lastSuccessfulRunTime value");
		        }

		        // save raw schedules to the db
		        await _pushpayService.PollDonationsAsync(lastSuccessfulRunTime);

                // process raw schedules
                await _pushpayService.ProcessRawDonations();

                return NoContent();
	        }
	        catch (Exception ex)
	        {
		        var error = $"Got error getting updates for donations from PushPay: {ex.Message}";
		        _logger.Error(error);
		        return StatusCode(500);
	        }
        }
    }
}
