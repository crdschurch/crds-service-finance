using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpCongregationMock
    {
        public static MpCongregation Create() =>
            new MpCongregation
            {
                CongregationId = 22,
                CongregationName = "test_congregation"
            };

        public static List<MpCongregation> CreateList() =>
            new List<MpCongregation>
            {
                new MpCongregation
                {
                    CongregationId = 22,
                    CongregationName = "test_congregation"
                }
            };
    }
}
