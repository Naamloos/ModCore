using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModCore.Api
{
    public class Perspective
    {
        private string _token;
        private HttpClient _httpclient;
        public Perspective(string token)
        {
            this._token = token;
            this._httpclient = new HttpClient();
        }

        public async Task RequestAnalysis(string message)
        {
            var RequestPayload = new PerspectiveAnalysisRequest()
            {
                Comment = new PerspectiveComment()
                {
                    Text = message
                }
            };
            var Content = new StringContent(JObject.FromObject(RequestPayload).ToString(), Encoding.UTF8, "application/json");

            var Response = await _httpclient.PostAsync("https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze", Content);

#warning TODO: Parse response to objects, https://github.com/conversationai/perspectiveapi/blob/master/api_reference.md#analyzecomment-response
        }
    }

    public class PerspectiveAnalysisRequest
    {
        [JsonProperty("comment")]
        public PerspectiveComment Comment = new PerspectiveComment();

        [JsonProperty("context")]
        public PerspectiveContext Context = new PerspectiveContext();

        [JsonProperty("requestedAttributes")]
        public Dictionary<string, PerspectiveContextEntry> RequestedAttributes = new Dictionary<string, PerspectiveContextEntry>();

        [JsonProperty("languages")]
        public List<string> Languages = new List<string>() { "en" };

        [JsonProperty("doNotStore")]
        public bool DoNotStore = true;

        [JsonProperty("clientToken")]
        public string Token = "";

        [JsonProperty("sessionId")]
        public string SessionId = "";
    }

    public class PerspectiveComment
    {
        [JsonProperty("text")]
        public string Text = "";

        [JsonProperty("type")]
        public string Type = "PLAIN_TEXT";
    }

    public class PerspectiveContext
    {
        [JsonProperty("entries")]
        public List<PerspectiveContextEntry> Entries = new List<PerspectiveContextEntry>();
    }

    public class PerspectiveContextEntry
    {
        [JsonProperty("text")]
        public string Text = "";

        [JsonProperty("type")]
        public string Type = "PLAIN_TEXT";
    }

    public class PerspectiveAttributes
    {
        [JsonProperty("scoreType")]
        public string ScoreType = "PROBABILITY";

        [JsonProperty("scoreTreshold")]
        public float ScoreTreshold;
    }
}
