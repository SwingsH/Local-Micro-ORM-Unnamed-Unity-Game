using System;

namespace TIZSoft.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan timeSpan, int multiplier)
        {
            return new TimeSpan(timeSpan.Ticks*multiplier);
        }

        public static TimeSpan Multiply(this TimeSpan timeSpan, long multiplier)
        {
            return new TimeSpan(timeSpan.Ticks*multiplier);
        }

        public static TimeSpan Multiply(this TimeSpan timeSpan, float multiplier)
        {
            return new TimeSpan((long)(timeSpan.Ticks*multiplier));
        }

        public static TimeSpan Multiply(this TimeSpan timeSpan, double multiplier)
        {
            return new TimeSpan((long)(timeSpan.Ticks*multiplier));
        }

        public static TimeSpan Divide(this TimeSpan timeSpan, int divisor)
        {
            return new TimeSpan(timeSpan.Ticks/divisor);
        }

        public static TimeSpan Divide(this TimeSpan timeSpan, long divisor)
        {
            return new TimeSpan(timeSpan.Ticks/divisor);
        }

        public static TimeSpan Divide(this TimeSpan timeSpan, float divisor)
        {
            return new TimeSpan((long)(timeSpan.Ticks/divisor));
        }

        public static TimeSpan Divide(this TimeSpan timeSpan, double divisor)
        {
            return new TimeSpan((long)(timeSpan.Ticks/divisor));
        }
    }
}