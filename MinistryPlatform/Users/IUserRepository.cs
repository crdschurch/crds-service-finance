using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MinistryPlatform.Users
{
    public interface IUserRepository
    {
        Task<int> GetUserByEmailAddress(string emailAddress);
    }
}
