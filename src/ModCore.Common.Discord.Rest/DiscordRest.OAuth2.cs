using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Responses;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<OAuth2Authorization>> GetCurrentAuthorizationInformationAsync(
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<OAuth2Authorization>(
                HttpMethod.Get,
                "oauth2/@me",
                "oauth2/@me",
                null,
                null,
                null,
                cancellationToken
            );

        public ValueTask<RestResponse<OAuth2TokenResponse>> ExchangeOAuth2CodeAsync(
            string clientId,
            string clientSecret,
            string code,
            string? redirectUri = null,
            string? codeVerifier = null,
            CancellationToken cancellationToken = default
        )
        {
            var form = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
            };
            if (redirectUri != null)
                form["redirect_uri"] = redirectUri;
            if (codeVerifier != null)
                form["code_verifier"] = codeVerifier;
            return SendRouteAsync<OAuth2TokenResponse>(
                HttpMethod.Post,
                "oauth2/token",
                "oauth2/token",
                form,
                null,
                null,
                cancellationToken,
                asForm: true
            );
        }

        public ValueTask<RestResponse<OAuth2TokenResponse>> RefreshOAuth2TokenAsync(
            string clientId,
            string clientSecret,
            string refreshToken,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<OAuth2TokenResponse>(
                HttpMethod.Post,
                "oauth2/token",
                "oauth2/token",
                new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = refreshToken,
                },
                null,
                null,
                cancellationToken,
                asForm: true
            );

        public ValueTask<RestResponse<OAuth2TokenResponse>> GetClientCredentialsTokenAsync(
            string clientId,
            string clientSecret,
            string scopes,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<OAuth2TokenResponse>(
                HttpMethod.Post,
                "oauth2/token",
                "oauth2/token",
                new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "client_credentials",
                    ["scope"] = scopes,
                },
                null,
                null,
                cancellationToken,
                asForm: true
            );

        public ValueTask<RestResponse<object>> RevokeOAuth2TokenAsync(
            string clientId,
            string clientSecret,
            string token,
            string? tokenTypeHint = null,
            CancellationToken cancellationToken = default
        )
        {
            var form = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["token"] = token,
            };
            if (tokenTypeHint != null)
                form["token_type_hint"] = tokenTypeHint;
            return SendRouteAsync<object>(
                HttpMethod.Post,
                "oauth2/token/revoke",
                "oauth2/token/revoke",
                form,
                null,
                null,
                cancellationToken,
                asForm: true
            );
        }
    }
}
