﻿using System;

namespace EventTesting
{
    public class VerificationException : Exception
    {
        public VerificationException(string message) : base(message)
        {
        }

        public VerificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}