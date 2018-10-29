using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IWebhooksRepository
    {
        void Create(MpPushpayWebhook webhookData);
    }
}
