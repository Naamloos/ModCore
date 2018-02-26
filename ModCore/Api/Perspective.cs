using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModCore.Api
{
    /// <summary>
    /// Using this for testing the perspective API
    /// </summary>
    public class PerspectiveTest
    {
        /*
        static void Main(params string[] args)
        {
            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            var settings = JsonConvert.DeserializeObject<Settings>(input);
            var p = new Perspective(settings.PerspectiveToken);
            Console.Write("Write a witty comment about my shoes: ");
            var c = Console.ReadLine();
            var a = p.RequestAnalysis(c).GetAwaiter().GetResult();
            Console.WriteLine($"analysis score: {a.AttributeScores.First().Value.SummaryScore.Value}");
            Console.ReadKey();
        }*/
    }

    public class Perspective
    {
        private string _token;
        private HttpClient _httpclient;
        public Perspective(string token)
        {
            this._token = token;
            this._httpclient = new HttpClient();
        }

        public async Task<PerspectiveAnalysisResponse> RequestAnalysis(string message)
        {
            var RequestPayload = new PerspectiveAnalysisRequest
            {
                Comment = new PerspectiveComment
                {
                    Text = message
                }
            };
            var Content = new StringContent(JsonConvert.SerializeObject(RequestPayload), Encoding.UTF8, "application/json");

            // pls don't blue thank you
            var response = await _httpclient.PostAsync("https://" + $"commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key={this._token}", Content);
            // score
            return JsonConvert.DeserializeObject<PerspectiveAnalysisResponse>(await response.Content.ReadAsStringAsync());
        }
    }

    #region Analysis request objects
    public class PerspectiveAnalysisRequest
    {
        [JsonProperty("comment")]
        public PerspectiveComment Comment = new PerspectiveComment();

        [JsonProperty("context")]
        public PerspectiveContext Context = new PerspectiveContext();

        [JsonProperty("requestedAttributes")]
        public Dictionary<string, PerspectiveAttributes> RequestedAttributes = new Dictionary<string, PerspectiveAttributes>
        {
            { "TOXICITY", new PerspectiveAttributes { ScoreTreshold = null, ScoreType = null } }
        };

        [JsonProperty("languages")]
        public List<string> Languages = new List<string> { "en" };

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
        [JsonProperty("scoreType", NullValueHandling = NullValueHandling.Ignore)]
        public string ScoreType = "PROBABILITY";

        [JsonProperty("scoreTreshold", NullValueHandling = NullValueHandling.Ignore)]
        public float? ScoreTreshold = 0.0f;
    }
    #endregion

    #region Analysis response objects
    public class PerspectiveAnalysisResponse
    {
        [JsonProperty("attributeScores")]
        public Dictionary<string, AttributeScore> AttributeScores = new Dictionary<string, AttributeScore>();
    }

    public class AttributeScore
    {
        [JsonProperty("spanScores")]
        List<SpanScore> SpanScores = new List<SpanScore>();

        [JsonProperty("summaryScore")]
        public Score SummaryScore = new Score();
    }

    public class SpanScore
    {
        [JsonProperty("begin")]
        public int Begin;
        [JsonProperty("end")]
        public int End;
        [JsonProperty("score")]
        public Score Score;
    }

    public class Score
    {
        [JsonProperty("value")]
        public float Value;

        [JsonProperty("type")]
        public string Type = "";
    }
    #endregion
}
