using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.PledgeCampaigns
{
    public interface IPledgeCampaignRepository
    {
        List<MpPledgeCampaign> GetCampaigns(int pledgeCampaignTypesId);
    }
}
