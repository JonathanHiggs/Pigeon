using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageRouter.Sandbox
{
    public class RegexTest
    {
        public static void Run()
        {
            var pattern = @"^tcp://((([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])|[\*]):([\d]{1,5})$";

            var tests = new List<string>
            {
                "tcp://something",
                "tcp://facebook.com:1",
                "tcp://a.b.c.s:12",
                "tcp://localhost:123",
                "tcp://localhost:123456",
                "tcp://*:1234",
                "tcp://som-s:12345",
                "tcp://kajsh@3",
                "tcp://010123",
                "tcp://_kdjhsad",
                "somethign.somc",
                "tcp://something",
                "tcp://facebook.com",
                "tcp://a.b.c.s:",
                "tcp://localhost:",
                "tcp://localhost:",
                "tcp://*:",
                "tcp://*as:",
                "tcp://as*as:",
                "tcp://som-s:",
                "tcp://kajsh@3",
                "tcp://010123",
                "tcp://_kdjhsad",
                "somethign.somc"
            };


            foreach (var test in tests)
            {
                Console.WriteLine($"{Regex.IsMatch(test, pattern)}\t{test}");
            }
        }
    }
}
