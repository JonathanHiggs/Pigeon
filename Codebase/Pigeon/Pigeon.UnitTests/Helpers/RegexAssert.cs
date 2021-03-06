﻿using System.Text.RegularExpressions;

using NUnit.Framework;

namespace Pigeon.UnitTests.Helpers
{
    public static class RegexAssert
    {
        public static void IsMatch(string actual, string pattern)
        {
            Assert.IsTrue(Regex.IsMatch(actual, pattern), $"Actual \"{actual}\" does not match regex pattern \"{pattern}\"");
        }


        public static void IsNotMatch(string actual, string pattern)
        {
            Assert.IsFalse(Regex.IsMatch(actual, pattern), $"Actual \"{actual}\" unexpectedly matches regex pattern \"{pattern}\"");
        }
    }
}
