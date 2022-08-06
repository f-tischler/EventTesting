using System;
using System.Collections.Generic;
using System.Text;
using EventTesting.Verifiers;

namespace EventTesting
{
    public static class Called
    {
        public static IVerifier Exactly(int times)
        {
            return new ExactVerifier(times);
        }

        public static IVerifier Never()
        {
            return Exactly(0);
        }

        public static IVerifier Once()
        {
            return Exactly(1);
        }

        public static IVerifier Twice()
        {
            return Exactly(2);
        }

        public static IVerifier AtLeast(int minimum)
        {
            return new MinimumVerifier(minimum);
        }

        public static IVerifier AtMost(int maximum)
        {
            return new MaximumVerifier(maximum);
        }
    }
}
