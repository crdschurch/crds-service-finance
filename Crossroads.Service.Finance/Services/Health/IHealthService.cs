using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Health
{
    public interface IHealthService
    {
        Task<bool> GetHangfireStatus();
    }
}
