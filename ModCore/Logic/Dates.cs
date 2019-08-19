using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Chronic;
using ModCore.Logic.Utils;

namespace ModCore.Logic
{
    public static class Dates
    {
        private const bool Continue = true;
        private const bool Break = false;

        public static (TimeSpan duration, string text) ParseTime(string dataToParse)
        {
            var tokenizer = new StringTokenizer(dataToParse);
            var time = 0UL;
            
            var foundDelimiter = false;
            foreach (var s in tokenizer)
            {
                var result = _parseToken(tokenizer, s.Trim(), time);
                time = result.time;
                // if broken by Break, we found a delimiter, if exited the loop normally no delimiter was found
                if (!result.cont)
                {
                    foundDelimiter = true;
                    break;
                }
            }

            // if my parsing yielded any results, return it
            if (time != 0UL)
            {
                // if we didn't find a delimiter, the tokenizer be empty, so let's use all of the data instead
                // TODO maybe come up with a better message than just using all the data?
                var message = foundDelimiter
                    ? tokenizer.Current + " " + tokenizer.Remaining()
                    : dataToParse;

                // if no message was provided, use the full text as a message
                return string.IsNullOrWhiteSpace(message) 
                    ? (Ts(time), dataToParse) 
                    : (Ts(time), message);
            }

            // attempt parsing with NChronic
            if (TryParseChronic(dataToParse, out var atime, out var atext) && atime > 0)
                return (Ts(atime), atext);

            // if everything else fails...
            throw new Exception("Parsed fail, there was no result!");
        }
        
        // TODO i've noticed all my returns are just "time" or "time + X", why not make the return value a relative
        // instead of an absolute?

        /// <summary>
        /// Action for an individual token in the reminder text.
        /// </summary>
        /// <param name="tokenizer">The tokenizer instance</param>
        /// <param name="s">The token</param>
        /// <param name="time">The current time state</param>
        /// <returns>A <see cref="ValueTuple{T1}"/>&lt;<see cref="Boolean"/>, <see cref="UInt64"/>&gt; containing
        /// <c>true</c> to continue parsing execution or <c>false</c> to escape control flow. The returned value of
        /// <c>time</c> is stored and used for the next token, or for the return value if control flow is escaped.</returns>
        /// <exception cref="Exception"></exception>
        private static (bool cont, ulong time) _parseToken(StringTokenizer tokenizer, string s, ulong time)
        {
            // filler words
            if (DateLexer.IsFillerWord(s))
                return (Continue, time);

            if (s == "tomorrow")
                return (Continue, time + (ulong) Unit.Day);

            // TODO maybe restore "next X" functionality?
            if (DateTime.TryParse(s, out var dt))
                return (Continue, (ulong)dt.Subtract(DateTime.Now).TotalMilliseconds);

            // read values like "5 seconds"
            if (DateLexer.TryIsNumber(s, out var i))
            {
                // ended early, so assume it's talking about minutes (e.g remindme in 5)
                if (!tokenizer.Next(out var s2))
                    return (Continue, time + (i * (ulong)Unit.Minutes));
                
                // ending words, so assume it's talking about minutes (e.g remindme in 5 to ...)
                if (DateLexer.IsFinishingWord(s2))
                    return (Break, time + (i * (ulong)Unit.Minutes));
                
                // get the amount of ms that corresponds to the unit of time s2
                if (!Enum.TryParse<Unit>(s2.Trim(), true, out var tk))
                    throw new Exception($"Unknown amount of time '{i}' of '{s2}'. If you think this is an unaccounted-for scenario, notify the dev!");

                // return N * MsValue
                return (Continue, time + ((ulong) tk * i));
            }

            // read compound values like "15h14min" or "5s"
            if (TryParseCompound(s, out var j))
                return (Continue, time + j);
            
            // read values like "15:14" as 15h14min
            if (TimeSpan.TryParse(s, out var ts))
                return (Continue, time + (ulong) ts.TotalMilliseconds);

            // ending words, so break and make the rest into a message
            if (DateLexer.IsFinishingWord(s))
                return (Break, time);

            DebugWriteLine("####\n" +
                           $"Unrecognized token: {s}\n" +
                           $"In text: {tokenizer.String}\n" +
                           $"At pos:  {new string('-', Math.Max(0, (tokenizer.Index==-1?tokenizer.String.Length:tokenizer.Index)-tokenizer.Current.Length))}^ (semi-accurate)\n" + // this is bad
                           "####");
            
            // what to do with invalid tokens? break? continue? i guess break
            return (Break, time);
        }

        /// <summary>
        /// Attempt to parse a length of time in a single word in <c>15h5m</c> format
        /// </summary>
        /// <param name="s">The token to parse</param>
        /// <param name="time">The resulting time to set</param>
        /// <returns>True if there was anything to parse, false otherwise</returns>
        /// <exception cref="Exception">If an invalid unit is encountered</exception>
        private static bool TryParseCompound(string s, out ulong time)
        {
            time = 0UL;
            var re = new Regex("([0-9]+)([a-zA-Z]+)");
            foreach (Match match in re.Matches(s))
            {
                var i = uint.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value;

                // get the amount of ms that corresponds to the unit of time unit
                if (!Enum.TryParse<Unit>(unit.Trim(), true, out var tk))
                    throw new Exception($"Unknown amount of time '{i}' of '{unit}'. If you think this is an unaccounted-for scenario, notify the dev!");

                // add N * MsValue
                time += (ulong) tk * i;
            }
            
            // we succeed if we matched/parsed anything, so the output is >0
            return time != 0UL;
        }

        /// <summary>
        /// Attempt to parse data in <c>in x to y</c> or <c>x ... y</c> format using NChronic <see cref="Parser"/>
        /// </summary>
        /// <param name="dataToParse">The input data to parse</param>
        /// <param name="time">The amount of time from now until the trigger point</param>
        /// <param name="text">The message left after the time</param>
        /// <returns>True if succeeded, false otherwise</returns>
        private static bool TryParseChronic(string dataToParse, out double time, out string text)
        {
            var parser = new Parser(new Options
            {
                Clock = () => DateTime.Now,
            });

            try
            {
                var start = parser.Parse(dataToParse).Start;
                if (start.HasValue && start != DateTime.Now)
                {
                    time = start.Value.Subtract(DateTime.Now).TotalMilliseconds;
                    text = dataToParse;
                    return true;
                }
            }
            catch(Exception e)
            {
                DebugWriteLine($"NChronic parsing failed step 1 with: {e}");
            }

            try
            {
                if (dataToParse.Contains("to"))
                {
                    var whereTo = dataToParse.IndexOf("to", StringComparison.Ordinal);

                    var lhs = dataToParse.Substring(0, whereTo);
                    var rhs = dataToParse.Substring(whereTo);

                    var start = parser.Parse(lhs).Start;
                    if (start.HasValue && start != DateTime.Now)
                    {
                        time = start.Value.Subtract(DateTime.Now).TotalMilliseconds;
                        text = rhs;
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                DebugWriteLine($"NChronic parsing failed step 2 with: {e}");
            }

            time = 0UL;
            text = null;
            return false;
        }

        /*
            if (tokens[0] == "next")
            {
                // hour, month, year, monday, tuesday...
                if (tokens.Count == 1)
                    return ParsingError;

                var today = DateTime.Today;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                switch (tokens[1])
                {
                    case "minute":
                        DebugWriteLine("Returning: next minute");
                        return TimeSpan.FromMinutes(1);
                    case "hour":
                        DebugWriteLine("Returning: next hour");
                        return TimeSpan.FromHours(1);
                    case "day":
                        DebugWriteLine("Returning: next day");
                        return TimeSpan.FromDays(1);
                    case "week":
                        DebugWriteLine("Returning: next week");
                        return TimeSpan.FromDays(7);
                    case "fortnight":
                        DebugWriteLine("Returning: next fortnight");
                        return TimeSpan.FromDays(14);
                    case "weekday":
                        DebugWriteLine("Returning: next work day");
                        return GetNextWorkingDay(today) - today;
                    case "month":
                        DebugWriteLine("Returning: next month");
                        return today.AddMonths(1) - today;
                    case "friedman":
                        DebugWriteLine("Returning: next 3 months");
                        return TimeSpan.FromDays(180);
                    case "quarter":
                        DebugWriteLine("Returning: next quarter");
                        return NextQuarter() - today;
                    case "semester":
                        DebugWriteLine("Returning: start of next semester");
                        return (today.Month <= 6
                                   ? new DateTime(today.Year, 7, 1)
                                   : new DateTime(today.Year + 1, 1, 1)) -
                               today;
                    case "year":
                        DebugWriteLine("Returning: next year");
                        return today.AddYears(1) - today;
                    case "kilonazi":
                        DebugWriteLine("Returning: 740,471 days from now");
                        return TimeSpan.FromDays(740_741);
                }

                // none of these so remove "next"
                DebugWriteLine($@"Bad use of next: ""{tokens[0]}{tokens[1]}""");
                tokens.RemoveAt(0);
            }
            
            
            
            
            
        private static DateTime NextQuarter()
        {
            var today = DateTime.Today;
            return today
                .AddMonths(3 - (today.Month - 1) % 3)
                .AddDays(-today.Day)
                .AddMonths(1);
        }

        private static DateTime GetNextWorkingDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (IsHoliday(date) || IsWeekEnd(date));

            return date;
        }
        
        private static bool IsHoliday(DateTime date) => date.Day == 1 && date.Month == 1
                                                        || date.Day == 5 && date.Month == 1
                                                        || date.Day == 10 && date.Month == 3
                                                        || date.Day == 25 && date.Month == 12;

        private static bool IsWeekEnd(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday
                                                        || date.DayOfWeek == DayOfWeek.Sunday;

        */
        
        // ReSharper disable UnusedMember.Local
        private enum Unit : ulong
        {
            Ms = 1,
            Milis = Ms,
            Millis = Ms,
            Milisecond = Ms,
            Miliseconds = Ms,
            Millisecond = Ms,
            Milliseconds = Ms,

            S = 1000 * Milisecond,
            Sec = S,
            Second = S,
            Secs = S,
            Seconds = S,

            M = 60 * Second,
            Min = M,
            Minute = M,
            Mins = M,
            Minutes = M,

            H = 60 * Minute,
            Hr = H,
            Hrs = H,
            Hour = H,
            Hours = H,

            D = 24 * Hour,
            Ds = D,
            Day = D,
            Days = D,

            W = 7 * Day,
            Ws = W,
            Week = W,
            Weeks = W,

            // Uncommon / fictional units

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
            Groon = 70 * Hour, //1 hour
            Klik = 72 * Second, //1.2 minutes
            Lightyear = 100 * Year, //1 year
            Megacycle = 156 * Minute, //2.6 hours
            Metacycle = 13 * Month, //13 months
            Nanocycle = 1 * Second, //1 second
            Nanoklik = 1200 * Millisecond, //1.2 second
            Orbital = 10 * Month, //1 month
            Quartex = 15 * Month, //1 month
            Stellarcycle = 225 * Day, //7.5 months
            Vorn = 83 * Year, //83 years
            Decivorn = 2988 * Day, //8.3 years
            Exapi = 3_141_590_400 * Millisecond, //36.361 days

            // Plural forms, ditto

            Fortnights = Fortnight,
            Months = Month,
            Quarters = Quarter,
            Semesters = Semester,
            Years = Year,
            Kilonazis = Kilonazi,
            Astroseconds = Astrosecond,
            Breems = Breem,
            Cyberweeks = Cyberweek,
            Cycles = Cycle,
            Decacycles = Decacycle,
            Groons = Groon,
            Kliks = Klik,
            Lightyears = Lightyear,
            Megacycles = Megacycle,
            Metacycles = Metacycle,
            Nanocycles = Nanocycle,
            Nanokliks = Nanoklik,
            Orbitals = Orbital,
            Quartexs = Quartex,
            Quartexes = Quartex,
            Stellarcycles = Stellarcycle,
            Vorns = Vorn,
            Decivorns = Decivorn,
            Exapis = Exapi
        }
        // ReSharper restore UnusedMember.Local

        [Conditional("DEBUG")]
        private static void DebugWriteLine(string text) => Console.WriteLine(text);

        /// <summary>
        /// Turn a length of milliseconds into a <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="time">The amount of milliseconds to convert</param>
        /// <returns>The newly created <see cref="TimeSpan"/></returns>
        private static TimeSpan Ts(double time) => TimeSpan.FromMilliseconds(time);
    }
}