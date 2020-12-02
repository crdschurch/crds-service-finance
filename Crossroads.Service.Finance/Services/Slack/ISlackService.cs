using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Slack
{
	public interface ISlackService
	{
		void SendSlackAlert(string resource, string channel, string title, string message);
	}
}
