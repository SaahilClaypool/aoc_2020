using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aoc.Runner
{
    /// Main entrypoint class to run solutions
    public class AocRunner
    {

        public static void Run(string[] args) =>
            Cli.Run(args);

        public static IEnumerable<Day> Days()
        {
            var assembly = Assembly.GetEntryAssembly()!;
            return assembly.GetTypes()
                .Where(type => typeof(Day).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(type => (Day)Activator.CreateInstance(type)!)
                .OrderByDescending(day => day.Number());
        }

        public static bool TestDay(Day day)
        {
            var failedTests = day.Tests.Where(test =>
            {
                var failed = !test.Run();
                if (!failed)
                {
                    Console.WriteLine($"Day {day.NumberString()}: Test {test.Name} passed!");
                }
                return failed;
            }).ToList();
            foreach (var test in failedTests)
            {
                Console.Write($"Test {test.Name} Failed:\n\tExpected: {test.ExpectedOutput}\n\tReceived: {test.Output}");
            }
            return !failedTests.Any();
        }

        public static bool RunDay(Day day)
        {
            try
            {
                var a = day.SolveA();
                Console.WriteLine($"Part A: {a}");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine($"Part A for Day {day.NumberString()} is not implmented!");
            }

            try
            {
                var b = day.SolveB();
                Console.WriteLine($"Part B: {b}");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine($"Part B for Day {day.NumberString()} is not implmented!");
            }
            return true;
        }

        public static int TestAll() =>
            Days().Where(day => TestDay(day)).Count();

        public static bool TestLast() =>
            !TestDay(Days().Last());

        public static bool TestOne(string day) =>
            !TestDay(Find(day));

        public static int RunAll() =>
            Days().Where(day => RunDay(day)).Count();

        public static bool RunLast() =>
            !RunDay(Days().Last());

        public static bool RunOne(string day) =>
            !RunDay(Find(day));

        private static Day Find(string day) =>
            Days().Where(dayClass => dayClass.GetType().Name == day).LastOrDefault() ?? throw new NoDayFound();
    }
    public class NoDayFound : Exception { }
}
