using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ModCore.Logic
{
    public static class Dates
    {
        private static readonly TimeSpan ParsingError = TimeSpan.MinValue;
        
        public static async Task<(TimeSpan duration, string text)> ParseTime(string dataToParse)
        {
            try
            {
                return await _parseTime(dataToParse);
            }
            catch (Exception e)
            {
                DebugWriteLine($"{e}\n{e.StackTrace}");
                return (ParsingError, $"parsing errored with {e}");
            }
        }
    
        private static async Task<(TimeSpan duration, string text)> _parseTime(string preData)
        {
            preData = preData.Trim();
            var spaces = preData.Count(e => e == ' ');
            if (preData.Length <= 3)
            {
                return (ParsingError, "not enough data given to infer input.");
            }
            switch (spaces)
            {
                case 0:
                    DebugWriteLine("spaces: 0");
                    return (ParsingError, "please give both a time amount and a reason.");
                // fast-track if there's only one space
                case 1:
                    DebugWriteLine("spaces: 1");
                    var data = preData.Split(' ');
                    return (await ParseTimeSpan(data[0]), data[1]);
            }
            // try parsing regular timespan as first word. this allows the old syntax to continue working without side effects.
            var firstSpace = preData.IndexOf(' ');
            if (TimeSpan.TryParse(preData.Substring(0, firstSpace), out var ts))
            {
                return (ts, preData.Substring(firstSpace + 1));
            }
            // try split by 'to' or comma, whichever gives the lowest result
            var idx1 = preData.IndexOfInvariant(" to ");
            var idx2 = preData.IndexOfInvariant(", ");
            if (idx2 < idx1 && idx2 != -1)
            {
                DebugWriteLine("splitting by comma");
                return (await ParseTimeSpan(preData.Substring(0, idx2).Trim()), preData.Substring(idx2 + 2));
            }
            if (idx1 != -1)
            {
                DebugWriteLine("splitting by 'to'");
                return (await ParseTimeSpan(preData.Substring(0, idx1).Trim()), preData.Substring(idx1 + 4));
            }
            // take half
            var adata = preData.Split(' ');
            var l2 = (int)(adata.Length/2.0);
            return (await ParseTimeSpan(string.Join(' ', adata.SkipLast(l2))), string.Join(' ', adata.TakeLast(l2)));
        }

        private static readonly Regex AllNumbers = new Regex("^[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex NoneNumbers = new Regex("^[^0-9]+$", RegexOptions.Compiled);
        private static readonly Regex NumWord = new Regex("^([a-zA-Z]+)([0-9]+)$", RegexOptions.Compiled);

        private static async Task<TimeSpan> ParseTimeSpan(string data)
        {
            if (TimeSpan.TryParse(data, out var time))
            {
                return time;
            }
            var tokens = data.Split(' ').Where(e => !string.IsNullOrWhiteSpace(e)).Select(e =>
            {
                var t = e.Trim();
                switch (t)
                {
                    case "a":
                    case "an":
                        return "1";
                    case "two":
                        return "2";
                    case "three":
                        return "3";
                    case "four":
                        return "4";
                    case "five":
                        return "5";
                    case "six":
                        return "6";
                    case "seven":
                        return "7";
                    case "eight":
                        return "8";
                    case "nine":
                        return "9";
                    case "ten":
                        return "10";
                    case "eleven":
                        return "11";
                    case "twelve":
                        return "12";
                    default:
                        return t;
                }
            }).ToList();

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (tokens[0] == "in")
                tokens.RemoveAt(0);

            if (tokens[0] == "tomorrow")
            {
                if (tokens.Count > 1 && tokens[1] == "at")
                {
                    return await ParseTomorrow(tokens);
                }
                return TimeSpan.FromDays(1);
            }
            if (tokens[0] == "next")
            {
                // hour, month, year, monday, tuesday...
                if (tokens.Count == 1)
                    return ParsingError;

                var today = DateTime.Today;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (tokens[1] == "minute")
                    return TimeSpan.FromMinutes(1);
                if (tokens[1] == "hour")
                    return TimeSpan.FromHours(1);
                if (tokens[1] == "day")
                    return TimeSpan.FromDays(1);
                if (tokens[1] == "week")
                    return TimeSpan.FromDays(7);
                if (tokens[1] == "fortnight")
                    return TimeSpan.FromDays(14);
                if (tokens[1] == "weekday")
                    return GetNextWorkingDay(today) - today;
                if (tokens[1] == "month")
                    return today.AddMonths(1) - today;
                if (tokens[1] == "friedman")
                    return TimeSpan.FromDays(180);
                if (tokens[1] == "quarter")
                    return NextQuarter() - today;
                if (tokens[1] == "semester")
                    return (today.Month <= 6 ? new DateTime(today.Year, 7, 1) : new DateTime(today.Year + 1, 1, 1)) -
                           today;
                if (tokens[1] == "year")
                    return today.AddYears(1) - today;
                if (tokens[1] == "kilonazi")
                    return TimeSpan.FromDays(740_741);

                // none of these so remove "next"
                DebugWriteLine($@"Bad use of next: ""{tokens[0]}{tokens[1]}""");
                tokens.RemoveAt(0);
            }

            if (NoneNumbers.IsMatch(tokens[0]))
            {
                // we don't know the amount
                if (tokens[0].EndsWith('s'))
                {
                    DebugWriteLine($"Invalid first token {tokens[0]}");
                    tokens.RemoveAt(0);
                }
                else
                {
                    tokens.Insert(0, "1");
                }
            }

            tokens = tokens.SelectMany(PossibleTokenSplit).ToList();

            //var units = new Dictionary<Unit, int>();
            ulong millis = 0;
            for (var i = 0; i < tokens.Count; i += 2)
            {
                if (!Enum.TryParse<Unit>(tokens[i + 1], true, out var tk))
                {
                    DebugWriteLine($"Unknown unit {tokens[i + 1]} (value:{tokens[i]})");
                    continue;
                }
                millis += (ulong) tk * ulong.Parse(tokens[i]);
//                if (units.ContainsKey(tk))
//                {
//                    units[tk] += int.Parse(tokens[i]);
//                }
//                else
//                {
//                    units[tk] = int.Parse(tokens[i]);
//                }
            }
#if DEBUG
            if (tokens.Count % 2 != 0)
                DebugWriteLine($"Uneven tokens, {string.Join(";", tokens)}");
#endif

            return TimeSpan.FromMilliseconds(millis);
        }

        private static async Task<TimeSpan> ParseTomorrow(IReadOnlyList<string> tokens)
        {
            var data = string.Join(' ', tokens).Trim();
            var now = DateTime.UtcNow;
            
            // Using constants to keep redundant code concise
            var iv = CultureInfo.InvariantCulture;
            const DateTimeStyles adj = DateTimeStyles.AdjustToUniversal;
            
            if (DateTime.TryParseExact(data, "HH:mm", iv, adj, out var dt))
            {
                return dt.AddDays(1) - now;
            }
            if (DateTime.TryParseExact(data, "HH:mm:ss", iv, adj, out var dt2))
            {
                return dt2.AddDays(1) - now;
            }
            if (DateTime.TryParseExact(data, "HH:mm:ss z", iv, adj, out var dt3))
            {
                return dt3.AddDays(1) - now;
            }
            if (DateTime.TryParseExact(data, "HH:mm:ss zz", iv, adj, out var dt4))
            {
                return dt4.AddDays(1) - now;
            }
            if (DateTime.TryParseExact(data, "HH:mm:ss zzz", iv, adj, out var dt5))
            {
                return dt5.AddDays(1) - now;
            }
            var endTokens = string.Join(' ', tokens.Skip(1));
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(endTokens);
                if (DateTime.TryParseExact(tokens[0], "HH:mm", iv, adj, out var dt6))
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(dt6, tz).AddDays(1) - now;
                }
                if (DateTime.TryParseExact(tokens[0], "HH:mm:ss", iv, adj, out var dt7))
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(dt7, tz).AddDays(1) - now;
                }
            }
            catch (TimeZoneNotFoundException e)
            {
                DebugWriteLine($@"Failed getting time zone from ""{endTokens}"": {e}");
                try
                {
                    var local = await GetLocalDateTimeOffset(endTokens);

                    if (DateTime.TryParseExact(tokens[0], "HH:mm", iv, adj, out var dt10))
                    {
                        return ApplyOffset(dt10.AddDays(1), local) - now;
                    }
                    if (DateTime.TryParseExact(tokens[0], "HH:mm:ss", iv, adj, out var dt11))
                    {
                        return ApplyOffset(dt11.AddDays(1), local) - now;
                    }
                }
                catch (Exception e2)
                {
                    DebugWriteLine($@"Failed getting time zone ""{endTokens}"" from GAPI: {e2}");
                }
                if (DateTime.TryParseExact(tokens[0], "HH:mm", iv, adj, out var dt8))
                {
                    return dt8.AddDays(1) - now;
                }
                if (DateTime.TryParseExact(tokens[0], "HH:mm:ss", iv, adj, out var dt9))
                {
                    return dt9.AddDays(1) - now;
                }
            }
            return TimeSpan.FromDays(1);
        }

        private static async Task<double> GetLocalDateTimeOffset(string place)
        {
            var client = new RestClient("https://maps.googleapis.com");
            var request = new RestRequest("maps/api/geocode/json", Method.GET);
            request.AddParameter("address", place.Replace(' ', '+'));
            var aresponse = await client.ExecuteTaskAsync<GeocodeResponse>(request);
            var loc = aresponse.Data.Results.FirstOrDefault()?.Geometry?.Location;
            if (loc == null) throw new MissingMemberException("location n/a (no results)");
            var lat = loc.Lat;
            var lon = loc.Lng;

            request = new RestRequest("maps/api/timezone/json", Method.GET);
            request.AddParameter("location", lat + "," + lon);
            request.AddParameter("timestamp", ToTimestamp(DateTime.UtcNow));
            request.AddParameter("sensor", "false");
            var response = await client.ExecuteTaskAsync<GoogleTimeZone>(request);

            return response.Data.RawOffset + response.Data.DstOffset;
        }

        private static DateTime ApplyOffset(DateTime utcDate, double offset) => utcDate.AddSeconds(offset);

        private static double ToTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private static IEnumerable<string> PossibleTokenSplit(string e)
        {
            var match = NumWord.Match(e);
            return !match.Success ? new[] {e} : new[] {match.Groups[0].Value, match.Groups[1].Value};
        }

        // ReSharper disable UnusedMember.Local
        private enum Unit : ulong
        {
            Ms = Millisecond,
            Milis = Millisecond,
            Millis = Millisecond,
            Milisecond = Millisecond,
            Millisecond = 1,
            S = Second,
            Sec = Second,
            Second = 1000 * Milisecond,
            M = Minute,
            Min = Minute,
            Minute = 60 * Second,
            H = Hour,
            Hr = Hour,
            Hour = 60 * Minute,
            D = Day,
            Day = 24 * Hour,
            W = Week,
            Week = 7 * Day,
            Fortnight = 14 * Day, // 14 days
            Month = 30 * Day,
            Quarter = 3 * Month, // 3 months
            Semester = 6 * Month, // 6 months
            Year = 12 * Month,
            Kilonazi = 740741 * Day, // 740741 days 
            Astrosecond = 498 * Millisecond, //0.498 seconds
            Breem = 498 * Second, //8.3 minutes
            Cyberweek = 7 * Day, //7 days
            Cycle = 75 * Minute, //1.25 hours
            Decacycle = 21 * Day, //21 days
            Groon = 1 * Hour, //1 hour
            Klik = 72 * Second, //1.2 minutes
            Lightyear = 1 * Year, //1 year
            Megacycle = 156 * Minute, //2.6 hours
            Metacycle = 13 * Month, //13 months
            Nanocycle = 1 * Second, //1 second
            Nanoklik = 1200 * Millisecond, //1.2 second
            Orbital = 1 * Month, //1 month
            Quartex = 1 * Month, //1 month
            Stellarcycle = 225 * Day, //7.5 months
            Vorn = 83 * Year, //83 years
            Decivorn = 2988 * Day, //8.3 years
            Exapi = 3_141_590_400 * Millisecond //36.361 days
        }
        // ReSharper restore UnusedMember.Local

        private static DateTime NextQuarter()
        {
            var today = DateTime.Today;
            return today
                .AddMonths(3 - (today.Month - 1) % 3)
                .AddDays(-today.Day)
                .AddMonths(1);
        }

        private static bool IsHoliday(DateTime date) => date.Day == 1 && date.Month == 1
                                                        || date.Day == 5 && date.Month == 1
                                                        || date.Day == 10 && date.Month == 3
                                                        || date.Day == 25 && date.Month == 12;

        private static bool IsWeekEnd(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday
                                                        || date.DayOfWeek == DayOfWeek.Sunday;

        private static DateTime GetNextWorkingDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (IsHoliday(date) || IsWeekEnd(date));
            return date;
        }
        
        [Conditional("DEBUG")]
        private static void DebugWriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }

    internal class GoogleTimeZone
    {
        [JsonProperty("dstOffset")]
        public double DstOffset { get; set; }

        [JsonProperty("rawOffset")]
        public double RawOffset { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeZoneId")]
        public string TimeZoneId { get; set; }

        [JsonProperty("timeZoneName")]
        public string TimeZoneName { get; set; }
    }

    internal class GeocodeResponse
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    internal class Result
    {
        [JsonProperty("address_components")]
        public AddressComponent[] AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    internal class Geometry
    {
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public Bounds Viewport { get; set; }
    }

    internal class Bounds
    {
        [JsonProperty("northeast")]
        public Location Northeast { get; set; }

        [JsonProperty("southwest")]
        public Location Southwest { get; set; }
    }

    internal class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    internal class AddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }
}