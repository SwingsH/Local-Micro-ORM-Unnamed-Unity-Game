namespace TIZSoft.Extensions
{
    public static class NumerialExtensions
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
    }
}
