using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using Crossroads.Service.Finance.Security;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using System.Linq;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DonorController : MpAuthBaseController
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDonationService _donationService;
        private readonly IContactService _contactService;

        public DonorController(IAuthTokenExpiryService authTokenExpiryService,
            IAuthenticationRepository authenticationRepository,
            IDonationService donationService, 
            IContactService contactService)
            : base(authenticationRepository, authTokenExpiryService)
        {
            _donationService = donationService;
            _contactService = contactService;
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
            return Authorized(token =>
            {
                try
                {
                    var recurringGifts = _donationService.GetRecurringGifts(token);
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
            });
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
            return Authorized(token =>
            {
                try
                {
                    var pledges = _donationService.GetPledges(token);
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
            });
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
            return Authorized(token =>
            {
                try
                {
                    var donations = _donationService.GetDonations(token);
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
            });
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
            return Authorized(token =>
            {
                try
                {
                    var donations = _donationService.GetDonationHistoryByContactId(contactId, token);
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
            });
        }

        [HttpGet("cogivers")]
        [ProducesResponseType(typeof(List<ContactDto>), 200)]
        [ProducesResponseType(204)]
        public IActionResult GetCogivers()
        {
            return Authorized(token =>
            {
                try
                {
                    var userContactId = _contactService.GetContactIdBySessionId(token);
                    var cogivers = _contactService.GetCogiversByContactId(userContactId);

                    return Ok(cogivers.OrderBy(c => c.FirstName));
                }
                catch (Exception ex)
                {
                    var msg = "DonationController: GetCogivers";
                    _logger.Error(msg, ex);
                    return BadRequest(ex.Message);
                }
            });
        }
    }
}

