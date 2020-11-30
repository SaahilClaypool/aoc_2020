using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Spectre.Cli;

namespace Aoc.Runner
{
    /// Main entrypoint class to run solutions
    public class AocRunner
    {
        public static CommandApp CreateApp()
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddBranch("test", branch =>
                {
                    branch.SetDescription("test cases defined in each day `Test` variable");
                    branch.AddDelegate<DayArgs>("one", (context, args) =>
                    {
                        var arg = args.Day;
                        try
                        {
                            arg = "Day" + int.Parse(arg).ToString();
                        }
                        catch
                        {

                        }
                        TestOne(arg);
                        return 0;
                    }).WithDescription("Run day <day>");
                    branch.AddDelegate<EmptyCommandSettings>("last", (_) =>
                    {
                        TestLast();
                        return 0;
                    }).WithDescription("Test only the last day");
                    branch.AddDelegate<EmptyCommandSettings>("all", (_) =>
                    {
                        TestAll();
                        return 0;
                    }).WithDescription("Test All");
                });

                config.AddBranch("run", branch =>
                {
                    branch.SetDescription("run on day inputs");
                    branch.AddDelegate<DayArgs>("one", (context, args) =>
                    {
                        var arg = args.Day;
                        try
                        {
                            arg = "Day" + int.Parse(arg).ToString();
                        }
                        catch
                        {

                        }
                        RunOne(arg);
                        return 0;
                    });
                    branch.AddDelegate<EmptyCommandSettings>("last", (_) =>
                    {
                        RunAll();
                        return 0;
                    }).WithDescription("Run only the last day");
                    branch.AddDelegate<EmptyCommandSettings>("all", (_) =>
                    {
                        RunAll();
                        return 0;
                    }).WithDescription("Run All");
                });
            });

            return app;
        }


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

        public static bool TestAll() =>
            !Days().Where(day => TestDay(day)).Any();

        public static bool TestLast() =>
            !TestDay(Days().Last());

        public static bool TestOne(string day) =>
            !TestDay(Days().Where(dayClass => dayClass.GetType().Name == day).Last());

        public static bool RunAll() =>
            !Days().Where(day => RunDay(day)).Any();

        public static bool RunLast() =>
            !RunDay(Days().Last());

        public static bool RunOne(string day) =>
            !RunDay(Days().Where(dayClass => dayClass.GetType().Name == day).Last());

        public sealed class DayArgs : CommandSettings
        {
            [CommandArgument(0, "<Day>")]
            [Description("Day or as either the number or the full name")]
            public string Day { get; set; } = "unset";
        }
    }
}
