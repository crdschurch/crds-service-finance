using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Helpers
{
    public static class Helpers
    {
        public static string Translate(string pushpayCongregation)
        {
            switch (pushpayCongregation)
            {
                case "Anywhere (Online)":
                    return "Anywhere";
                default:
                    return pushpayCongregation;
            }
        }

        public static string GetCardBrand(string pushpayCardBrand)
        {
	        switch (pushpayCardBrand)
	        {
		        case "VISA":
			        return "Visa";
		        case "Discover":
			        return "Discover";
		        case "Amex":
			        return "AmericanExpress";
		        case "MasterCard":
			        return "MasterCard";
		        default:
			        return "";
	        }
        }
    }
}
