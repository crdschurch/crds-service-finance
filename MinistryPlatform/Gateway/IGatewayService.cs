namespace MinistryPlatform.Interfaces
{
    public interface IGatewayService
    {
        void CancelStripeRecurringGift(string stripeSubscriptionId);
    }
}
