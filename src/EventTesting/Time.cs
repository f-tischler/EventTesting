using System;

namespace EventTesting
{
    public static class Time
    {
        public static WithinTimeVerifier Within(this IVerifier v, TimeSpan span)
        {
            return new WithinTimeVerifier(v, span);
        }
    }
}