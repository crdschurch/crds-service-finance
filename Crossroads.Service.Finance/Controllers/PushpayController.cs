using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Crossroads.Web.Auth.Models;
using Crossroads.Web.Common.Auth.Helpers;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> UpdateRecurringGiftsAsync()
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];
            try
            {
                await _pushpayService.PullRecurringGiftsAsync();
                _logger.Info("All update jsons are saved to the DB");
                _logger.Info("Starting to process the updates");
                await _recurringService.SyncRecurringSchedules();
                _logger.Info("UpdateRecurringGiftsAsync is complete.");
                return Ok();
            }

            catch (Exception ex)
            {
                _logger.Error("Error in PushpayController.UpdateRecurringGiftsAsync run by {user} with exception {message}", authDto.UserInfo.Mp.UserId, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("updatedonations")]
        public async Task<IActionResult> UpdateDonationsAsync()
        {
	        try
	        {
		        // save raw schedules to the db
		        await _pushpayService.PollDonationsAsync();

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
