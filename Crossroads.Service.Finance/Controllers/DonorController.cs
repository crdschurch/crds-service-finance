using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DonorController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDonationService _donationService;

        public DonorController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        /// <summary>
        /// Get recurring gifts for a user
        /// </summary>
        /// <returns></returns>
        [HttpGet("recurring-gifts")]
        [ProducesResponseType(typeof(List<RecurringGiftDto>), 200)]
        [ProducesResponseType(204)]
        public IActionResult GetRecurringGifts()
        {
            try
            {
                //TODO remove hardcoded "token" value and add authentication
                var recurringGifts = _donationService.GetRecurringGifts("token");
                if (recurringGifts == null || recurringGifts.Count == 0)
                {
                    return NoContent();
                }
                return Ok(recurringGifts);
            }
            catch (Exception ex)
            {
                var msg = "DonorController: GetRecurringGifts";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get pledges for a user
        /// </summary>
        /// <returns></returns>
        [HttpGet("pledges")]
        [ProducesResponseType(typeof(List<PledgeDto>), 200)]
        [ProducesResponseType(204)]
        public IActionResult GetMyPledges()
        {
            try
            {
                //TODO remove hardcoded "token" value and add authentication
                var pledges = _donationService.GetPledges("token");
                if (pledges == null || pledges.Count == 0)
                {
                    return NoContent();
                }

                return Ok(pledges);
            }
            catch (Exception ex)
            {
                var msg = "DonorController: GetPledges";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get donations (donation history) for a user
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [HttpGet("donations")]
        [ProducesResponseType(typeof(List<DonationDto>), 200)]
        [ProducesResponseType(204)]
        public IActionResult GetDonations()
        {
            try
            {
                //TODO remove hardcoded "token" value and add authentication
                var donations = _donationService.GetDonations("token");
                if (donations == null || donations.Count == 0)
                {
                    return NoContent();
                }

                return Ok(donations);
            }
            catch (Exception ex)
            {
                var msg = "DonationController: GetDonations";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get donations (donation history) for a user
        /// </summary>
        /// <returns></returns>
        [HttpGet("{contactId}/donations")]
        [ProducesResponseType(typeof(List<DonationDto>), 200)]
        [ProducesResponseType(204)]
        public IActionResult GetDonationHistory(int contactId)
        {
            try
            {
                //TODO remove hardcoded "token" value and add authentication
                var donations = _donationService.GetDonationHistoryByContactId(contactId);// ("token");
                if (donations == null || donations.Count == 0)
                {
                    return NoContent();
                }

                return Ok(donations);
            }
            catch (Exception ex)
            {
                var msg = "DonationController: GetDonationHistory";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}

