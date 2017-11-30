using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class DepositMock
    {
        public static DepositDto Create() =>
            new DepositDto
            {
                Id = 22
            };

        public static DepositDto CreateEmpty() => new DepositDto { };
    }
}
