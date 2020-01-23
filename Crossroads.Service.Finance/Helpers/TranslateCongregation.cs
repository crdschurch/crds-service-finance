using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Helpers
{
    public static class TranslateCongregation
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
    }
}
