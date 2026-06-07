using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Discord.Rest;
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
        private readonly DiscordRest _rest;

        // Inject IHttpClientFactory to safely make external API requests to Discord
        public ApiController(IHttpClientFactory httpClientFactory, IConfiguration configuration, DiscordRest rest)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _rest = rest;
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

            var clientId = _configuration.GetValue<string>("discord_client_id")!;
            var clientSecret = _configuration.GetValue<string>("discord_client_secret")!;
            
            var oauthResponse = await _rest.AuthenticateOAuth2Token(clientId, clientSecret, request.Code);
            if(oauthResponse.Success)
            {
                return Content(oauthResponse.RawBody, "application/json");
            }

            return StatusCode((int)oauthResponse.HttpResponse.StatusCode, oauthResponse.HttpResponse.Content);
        }
    }

    // Model class to bind the incoming JSON payload from your React frontend fetch
    public class TokenRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }
}