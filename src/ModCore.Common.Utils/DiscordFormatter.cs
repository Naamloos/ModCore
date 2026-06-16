using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.Utils
{
    public static class DiscordFormatter
    {
        public enum TimestampFormat
        {
            Default,
            ShortTime,
            LongTime,
            ShortDate,
            LongDate,
            ShortDateTime,
            LongDateTime,
            RelativeTime
        }

        public static string Timestamp(TimeSpan timeSpan, TimestampFormat format = TimestampFormat.Default)
        {
            return Timestamp(DateTime.UtcNow.Add(timeSpan), format);
        }

        public static string Timestamp(long unixTimeSeconds, TimestampFormat format = TimestampFormat.Default)
        {
            return Timestamp(DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).DateTime, format);
        }

        public static string Timestamp(DateTimeOffset dateTimeOffset, TimestampFormat format = TimestampFormat.Default)
        {
            return Timestamp(dateTimeOffset.DateTime, format);
        }

        public static string Timestamp(DateTime dateTime, TimestampFormat format = TimestampFormat.Default)
        {
            string suffix = "";

            switch(format)
            {
                case TimestampFormat.ShortTime:
                    suffix = ":t";
                    break;
                case TimestampFormat.LongTime:
                    suffix = ":T";
                    break;
                case TimestampFormat.ShortDate:
                    suffix = ":d";
                    break;
                case TimestampFormat.LongDate:
                    suffix = ":D";
                    break;
                case TimestampFormat.ShortDateTime:
                    suffix = ":f";
                    break;
                case TimestampFormat.LongDateTime:
                    suffix = ":F";
                    break;
                case TimestampFormat.RelativeTime:
                    suffix = ":R";
                    break;

                case TimestampFormat.Default:
                default:
                    break;
            }

            return $"<t:{new DateTimeOffset(dateTime).ToUnixTimeSeconds()}{suffix}>";
        }
    }
}
