using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Congregations
{
    public interface ICongregationRepository
    {
        List<MpCongregation> GetCongregationByCongregationName(string congregationName);
    }
}
