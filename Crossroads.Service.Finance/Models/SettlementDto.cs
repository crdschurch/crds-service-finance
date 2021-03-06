﻿using System;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class SettlementDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("totalAmount")]
        public AmountDto TotalAmount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("totalPayments")]
        public int TotalPayments { get; set; }

        [JsonProperty("estimatedDepositDate")]
        public DateTime EstimatedDepositDate { get; set; }

        [JsonProperty("isReconciled")]
        public bool IsReconciled { get; set; }
    }
}
