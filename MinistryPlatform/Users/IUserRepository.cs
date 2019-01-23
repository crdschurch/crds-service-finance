using System;
using System.Collections.Generic;
using System.Text;

namespace MinistryPlatform.Users
{
    public interface IUserRepository
    {
        int GetUserByEmailAddress(string emailAddress);
    }
}
