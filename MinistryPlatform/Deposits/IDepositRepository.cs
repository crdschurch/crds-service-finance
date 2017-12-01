using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Deposits
{
    public interface IDepositRepository
    {
        MpDeposit SaveDeposit(MpDeposit mpDeposit);
    }
}
