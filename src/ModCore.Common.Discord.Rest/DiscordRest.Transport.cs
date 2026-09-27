using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        private static string EscapeRouteValue(object value) =>
            Uri.EscapeDataString(
                Convert.ToString(value, CultureInfo.InvariantCulture)
                    ?? throw new ArgumentNullException(nameof(value))
            );

        private async ValueTask<RestResponse<T>> SendRouteAsync<T>(
            HttpMethod method,
            string url,
            string route,
            object? body,
            object? query,
            string? auditLogReason,
            CancellationToken cancellationToken,
            bool asForm = false
        )
        {
            if (query != null)
            {
                var parameters = JsonSerializer.SerializeToElement(query, JsonSerializerOptions);
                var pairs = new List<string>();
                foreach (var property in parameters.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                        continue;
                    var value =
                        property.Value.ValueKind == JsonValueKind.Array
                            ? string.Join(',', property.Value.EnumerateArray().Select(QueryValue))
                            : QueryValue(property.Value);
                    pairs.Add(
                        Uri.EscapeDataString(property.Name) + "=" + Uri.EscapeDataString(value)
                    );
                }
                if (pairs.Count > 0)
                    url += "?" + string.Join('&', pairs);
            }
            var response = await RatelimitedRest.RequestAsync(
                method,
                route,
                url,
                body,
                asForm,
                auditLogReason,
                cancellationToken
            );
            T? valueResult = default;
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                if (typeof(T) == typeof(byte[]))
                    valueResult = (T)(object)bytes;
                else if (typeof(T) == typeof(string))
                    valueResult = (T)(object)Encoding.UTF8.GetString(bytes);
                else if (bytes.Length > 0)
                    valueResult = JsonSerializer.Deserialize<T>(bytes, JsonSerializerOptions);
            }
            else
                _logger?.LogWarning(
                    "Discord request failed with HTTP {StatusCode}.",
                    (int)response.StatusCode
                );
            return new RestResponse<T>(valueResult, response);
        }

        private static string QueryValue(JsonElement value) =>
            value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText();
    }
}
