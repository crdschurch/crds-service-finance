using System;
using System.Collections.Generic;
using System.Text;

namespace MinistryPlatform.Models
{
    public class MpDonationDistribution
    {
        public int donationDistributionId { get; set; }
        public int donationId { get; set; }
        public int donationDistributionAmt { get; set; }
        public string donationDistributionProgram { get; set; }
        public int? PledgeId { get; set; }
    }
}
