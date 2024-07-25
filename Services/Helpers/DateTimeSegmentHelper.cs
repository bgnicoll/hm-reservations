namespace Services.Helpers
{
    public static class DateTimeSegmentHelper
    {
        public static DateTime RoundUpToNearestSegment(DateTime dateTime)
        {
            var minutes = dateTime.Minute;
            var remainder = minutes % 15;
            if (remainder == 0)
                return dateTime.AddSeconds(-dateTime.Second).AddMilliseconds(-dateTime.Millisecond);

            return dateTime.AddMinutes(15 - remainder).AddSeconds(-dateTime.Second).AddMilliseconds(-dateTime.Millisecond);
        }

        public static DateTime RoundDownToNearestSegment(DateTime dateTime)
        {
            var minutes = dateTime.Minute;
            var remainder = minutes % 15;
            return dateTime.AddMinutes(-remainder).AddSeconds(-dateTime.Second).AddMilliseconds(-dateTime.Millisecond);
        }
    }
}
