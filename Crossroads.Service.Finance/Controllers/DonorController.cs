using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
//using Crossroads.Service.Finance.Security;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Web.Auth.Controllers;
using Crossroads.Web.Auth.Models;
using Crossroads.Web.Common.Auth.Helpers;

namespace Crossroads.Service.Finance.Controllers
{
    [RequiresAuthorization]
    [Route("api/[controller]")]
    public class DonorController : AuthBaseController
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
        [HttpGet("{contactId}/recurring-gifts")]
        [ProducesResponseType(typeof(List<RecurringGiftDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetRecurringGifts(int contactId)
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            try
            {
                List<RecurringGiftDto> recurringGifts;
                var userContactId = authDto.UserInfo.Mp.ContactId;

                // override contact id if impersonating
                if (!String.IsNullOrEmpty(Request.Headers["ImpersonatedContactId"]))
                {
                    if (authDto.UserInfo.Mp.CanImpersonate == false)
                    {
                        throw new Exception("Impersonation Error");
                    }

                    userContactId = int.Parse(Request.Headers["ImpersonatedContactId"]);
                }

                if (userContactId == contactId)
                {
                    recurringGifts = await _donationService.GetRecurringGifts(userContactId);
                }
                else
                {
                    recurringGifts = await _donationService.GetRelatedContactRecurringGifts(userContactId, contactId);
                }

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
        [HttpGet("contact/{contactId}/pledges")]
        [ProducesResponseType(typeof(List<PledgeDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetMyPledges(int contactId)
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            try
            {
                List<PledgeDto> pledges;
                var userContactId = authDto.UserInfo.Mp.ContactId;

                // override contact id if impersonating
                if (!String.IsNullOrEmpty(Request.Headers["ImpersonatedContactId"]))
                {
                    if (authDto.UserInfo.Mp.CanImpersonate == false)
                    {
                        throw new Exception("Impersonation Error");
                    }

                    userContactId = int.Parse(Request.Headers["ImpersonatedContactId"]);
                }

                if (userContactId == contactId)
                {
                    pledges = await _donationService.GetPledges(contactId);
                }
                else
                {
                    pledges = await _donationService.GetRelatedContactPledge(userContactId, contactId);
                }

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
        [HttpGet("contact/{contactId}/donations")]
        [ProducesResponseType(typeof(List<DonationDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDonationHistory(int contactId)
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            var userContactId = authDto.UserInfo.Mp.ContactId;

            _logger.Info(("Getting donations"));

            try 
            { 
                // override contact id if impersonating
                if (!String.IsNullOrEmpty(Request.Headers["ImpersonatedContactId"]))
                {
                    if (authDto.UserInfo.Mp.CanImpersonate == false)
                    {
                        throw new Exception("Impersonation Error");
                    }

                    userContactId = int.Parse(Request.Headers["ImpersonatedContactId"]);
                }

                List<DonationDetailDto> donations;

                if (contactId == userContactId)
                {
                    // get logged in user's donations
                    donations = _donationService.GetDonations(userContactId).Result;
                }
                else
                {
                    // get related contact donations (minor child in household or active co-giver)
                    donations = await _donationService.GetRelatedContactDonations(userContactId, contactId);
                }
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

        [HttpGet("contacts/related")]
        [ProducesResponseType(typeof(List<ContactDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDonorRelatedContacts()
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            try
            {
                var contactId = authDto.UserInfo.Mp.ContactId;

                // override contact id if impersonating
                if (!String.IsNullOrEmpty(Request.Headers["ImpersonatedContactId"]))
                {
                    if (authDto.UserInfo.Mp.CanImpersonate == false)
                    {
                        throw new Exception("Impersonation Error");
                    }

                    contactId = int.Parse(Request.Headers["ImpersonatedContactId"]);
                }

                var userDonationVisibleContacts = await _contactService.GetDonorRelatedContacts(contactId);
                return Ok(userDonationVisibleContacts);
            }
            catch (Exception ex)
            {
                var msg = "DonationController: GetDonorRelatedContacts";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get donations (donation history) for a user
        /// </summary>
        /// <returns></returns>
        [HttpGet("{contactId}/othergifts")]
        [ProducesResponseType(typeof(List<DonationDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetOtherGifts(int contactId)
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            try
            {
                var userContactId = authDto.UserInfo.Mp.ContactId;

                // override contact id if impersonating
                if (!String.IsNullOrEmpty(Request.Headers["ImpersonatedContactId"]))
                {
                    if (authDto.UserInfo.Mp.CanImpersonate == false)
                    {
                        throw new Exception("Impersonation Error");
                    }

                    userContactId = int.Parse(Request.Headers["ImpersonatedContactId"]);
                }

                List<DonationDetailDto> otherGifts;

                if (contactId == userContactId)
                {
                    // get logged in user's other gifts
                    otherGifts = await _donationService.GetOtherGifts(contactId);
                }
                else
                {
                    // get related contact other gifts
                    otherGifts = await _donationService.GetRelatedContactOtherGifts(userContactId, contactId);
                }
                if (otherGifts == null || otherGifts.Count == 0)
                {
                    return NoContent();
                }

                return Ok(otherGifts);
            }
            catch (Exception ex)
            {
                var msg = "DonorController: GetOtherGifts";
                _logger.Error(msg, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}

