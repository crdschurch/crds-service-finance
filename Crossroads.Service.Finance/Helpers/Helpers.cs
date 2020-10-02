using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Helpers
{
    public static class Helpers
    {
        public const string AnywhereName = "Anywhere/Online";

        public static string Translate(string pushpayCongregation)
        {
            return pushpayCongregation.IndexOf("Anywhere") >= 0 ? AnywhereName : pushpayCongregation;
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
