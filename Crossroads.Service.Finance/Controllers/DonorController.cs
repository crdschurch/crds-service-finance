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

        public DonorController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpGet]
        [Route("recurring-gifts")]
        public IActionResult GetRecurringGifts()
        {
            try
            {
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
        
        //[HttpGet("pledges")]
        public IActionResult Pledges()
        {
            try
            {
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

        [HttpGet]
        [Route("donations")]
        public IActionResult GetDonations()
        {
            try
            {
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
    }
}

