namespace TIZSoft.Extensions
{
    public static class NumericalExtensions
    {
        const float IntegerToPercent = 0.01F;
        const int CentimeterPerMeter = 100;
        const float CentimeterPerMeterFloat = 100F;

        public static float Percent(this byte value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this sbyte value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this short value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this ushort value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this int value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this uint value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this long value)
        {
            return value*IntegerToPercent;
        }

        public static float Percent(this ulong value)
        {
            return value*IntegerToPercent;
        }

        public static int MinutesToSeconds(this int minutes)
        {
            return minutes*60;
        }

        public static int SecondsToMilliSeconds(this int seconds)
        {
            return seconds*1000;
        }

        public static int MetersToCentimeters(this int meters)
        {
            return meters*CentimeterPerMeter;
        }

        public static float MetersToCentimeters(this float meters)
        {
            return meters*CentimeterPerMeterFloat;
        }

        public static string KiloFormat(this long num)
        {
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 1000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString();
        }

        public static string TimeFormat(this float seconds)
        {
            if (seconds >= 604800)
                return (seconds / 604800).ToString("#,0W");

            if (seconds >= 86400)
                return (seconds / 86400).ToString("0.#") + "D";

            if (seconds >= 3600)
                return (seconds / 3600).ToString("#,0m");

            if (seconds >= 60)
                return (seconds / 60).ToString("0.#") + "s";

            return seconds.ToString("#,0");

        }

        public static int ToInt(this bool value)
        {
            return (value) ? 1 : 0;
        }
    }
}
