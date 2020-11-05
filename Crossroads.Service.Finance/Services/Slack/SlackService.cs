using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services.Slack
{
	public class SlackService : ISlackService
	{
		private readonly IRestClient _restClient;

		public SlackService()
		{
			_restClient = new RestClient();
		}

		public async void SendSlackAlert(string resource, string channel, string title, string message)
		{
			Dictionary<string, string> body = new Dictionary<string, string>
			{
				{ "channel", channel },
				{ "title", title},
				{ "text", message }
			};

			string jsonBody = JsonConvert.SerializeObject(body, Formatting.Indented);

			var request = new RestRequest(Method.POST)
			{
				//Resource = "https://hooks.slack.com/services/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"
				Resource = resource
			};

			request.AddJsonBody(jsonBody);
			request.AddHeader("content-type", "application/json");

			var response = _restClient.Execute(request);
		}
	}
}
