using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IDepositRepository
    {
        MpDeposit CreateDeposit(MpDeposit mpDeposit);
        MpDeposit GetDepositByProcessorTransferId(string processorTransferId);
        List<MpDeposit> GetDepositsByTransferIds(List<string> transferIds);
    }
}
