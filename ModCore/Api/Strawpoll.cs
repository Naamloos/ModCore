using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModCore.Api
{
    public class Strawpoll
    {
		private HttpClient _httpclient;
		public Strawpoll()
		{
			_httpclient = new HttpClient();
		}

		public async Task<string> CreatePollAsync(string title, params string[] options)
		{
			var payload = new StrawpollObject()
			{
				Title = title,
				Options = options,
			};
			Console.WriteLine(JsonConvert.SerializeObject(payload));
			var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
			Console.WriteLine($"{content.Headers.ContentType.MediaType} : {await content.ReadAsStringAsync()}");
			var response = await _httpclient.PostAsync("https://strawpoll.me/api/v2/polls", content);
			var res = await response.Content.ReadAsStringAsync();
			Console.WriteLine(res);
			var json = JsonConvert.DeserializeObject<StrawpollObject>(res);
			return $"https://www.strawpoll.me/{json.id}";
		}
    }

	internal class StrawpollObject
	{
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public int? id = null;

		[JsonProperty("title")]
		public string Title;

		[JsonProperty("options")]
		public string[] Options;
	}
}
