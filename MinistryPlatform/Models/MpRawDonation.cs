using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
	[MpRestApiTable(Name = "cr_RawPushpayDonations")]
	public class MpRawDonation
	{
		[JsonProperty(PropertyName = "DonationId")]
		public int DonationId { get; set; }

		[JsonProperty(PropertyName = "RawJson")]
		public string RawJson { get; set; }

		[JsonProperty(PropertyName = "IsProcessed")]
		public bool IsProcessed { get; set; }

		[JsonProperty(PropertyName = "TimeCreated")]
		public DateTime TimeCreated { get; set; }
	}
}
