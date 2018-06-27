using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Middleware;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Crossroads.Service.Finance.Controllers
{
    public class DonorController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDonationService _donationService;
        // private readonly IAnalyticsService _analyticsService;

        public DonorController(IDonationService donationService
                                // IAnalyticsService analyticsService
                                )
        {
            _donationService = donationService;
            // _analyticsService = analyticsService;
        }
        
        /// <summary>
        /// Retrieve list of recurring gifts for the logged-in donor.
        /// </summary>
        /// <returns>A list of RecurringGiftDto</returns>
        [ProducesResponseType(typeof(List<RecurringGiftDto>), 200)]
        [Route("donor/recurringgifts")]
        [HttpGet]
        public IActionResult GetRecurringGifts()
        {
            try
            {
                var recurringGifts = _donationService.GetRecurringGifts("token");
                if (recurringGifts == null || recurringGifts.Count == 0)
                {
                    return NotFound(new ObjectResult("No recurring gifts found"));
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
        /// Retrieve list of capital campaign pledges for the logged-in donor.
        /// </summary>
        /// <returns>A list of PledgeDto</returns>
        [ProducesResponseType(typeof(List<PledgeDto>), 200)]
        [Route("donor/pledges")]
        [HttpGet]
        public IActionResult GetPledges()
        {
            try
            {
                var pledges = _donationService.GetPledges("token");
                if (pledges == null || pledges.Count == 0)
                {
                    return NotFound(new ObjectResult("No pledges found"));
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
        /// Retrieve list of donations for the logged-in donor.
        /// </summary>
        /// <returns>A list of DonationDTOs</returns>
        [ProducesResponseType(typeof(List<DonationDto>), 200)]
        [Route("donor/donations")]
        [HttpGet]
        public IActionResult GetDonations()
        {
            try
            {
                var donations = _donationService.GetDonations("token");
                if (donations == null || donations.Count == 0)
                {
                    return NotFound(new ObjectResult("No donations found"));
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
    }
}

