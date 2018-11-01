using Polly;
using System;
using System.Threading;

namespace PollyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test Retry ...");
            PollyWaitRetry();
            Console.ReadLine();
        }

        public static void PollyImmediatelyRetry()
        {
            var handle = PolicyBuilderFactory(nameof(PollyImmediatelyRetry));

            //立刻重试
            handle.Retry(3, (ex, count) =>
            {
                Console.WriteLine($"Retry count:{count},Exception:{ex.Message}");
            })
            .Execute(() =>
            {
                Test();
            });
        }

        public static void PollyWaitRetry()
        {
            var handle = PolicyBuilderFactory(nameof(PollyWaitRetry));
            //间隔重试
            handle.WaitAndRetry(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4),
            }, OnRetry)
            .Execute(() =>
            {
                Test();
            });
        }

        private static PolicyBuilder PolicyBuilderFactory(string callStack = "")
        {
            var builder = Policy.Handle<TimeoutException>();
            builder.Fallback(() =>
            {
                Console.WriteLine($"{callStack} Fallback");
            });
            return builder;
        }

        private static void OnRetry(Exception ex, TimeSpan timeSpan)
        {
            Console.WriteLine("TimeSpan:" + timeSpan.TotalSeconds);
        }

        public static void Test(int count = 0)
        {
            Console.WriteLine("Test Count:" + count);
            Thread.Sleep(1000);
            throw new TimeoutException("Time out.");
        }
    }
}
