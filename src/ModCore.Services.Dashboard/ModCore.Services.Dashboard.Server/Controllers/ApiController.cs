using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Dashboard.Server.Controllers
{
    [ApiController]
    [Route("api/token")] // Explicitly maps this controller to match your React frontend fetch
    public class ApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // Inject IHttpClientFactory to safely make external API requests to Discord
        public ApiController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> hello()
        {
            return Ok("hello!");
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { error = "Authorization code is required." });
            }

            var client = _httpClientFactory.CreateClient();

            var dict = new Dictionary<string, string>
            {
                { "client_id", _configuration.GetValue<string>("discord_client_id")! },
                { "client_secret", _configuration.GetValue<string>("discord_client_secret")! },
                { "grant_type", "authorization_code" },
                { "code", request.Code }
            };

            var content = new FormUrlEncodedContent(dict);

            try
            {
                var response = await client.PostAsync("https://discord.com/api/oauth2/token", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, responseString);
                }

                // FIX: Return the raw Discord JSON string directly with the correct content type header
                return Content(responseString, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // Model class to bind the incoming JSON payload from your React frontend fetch
    public class TokenRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }
}