using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    // TODO: Determine if this can be canned - Dustin sez yes
    public class PaymentDto
    {
        [JsonProperty(PropertyName = "paymentId")]
        public int PaymentId { get; set; }

        [JsonProperty(PropertyName = "invoiceId")]
        public int InvoiceId { get; set; }

        [JsonProperty(PropertyName = "contactId")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "stripeTransactionId")]
        public string StripeTransactionId { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "paymentTypeId")]
        public int PaymentTypeId { get; set; }

        [JsonProperty(PropertyName = "batchId")]
        public int? BatchId { get; set; }

        [JsonProperty(PropertyName = "processorFee")]
        public decimal ProcessorFee { get; set; }

        [JsonProperty(PropertyName = "paymentStatus")]
        public DonationStatus Status { get; set; }
    }
}
