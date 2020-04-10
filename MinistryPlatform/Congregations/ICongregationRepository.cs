using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Congregations
{
    public interface ICongregationRepository
    {
        Task<List<MpCongregation>> GetCongregationByCongregationName(string congregationName);
    }
}
