using System;
using System.ComponentModel;
using System.Linq;
using Spectre.Cli;

namespace Aoc.Runner
{
    public class Cli
    {
        public static void Run(string[] args) =>
            CreateApp().Run(args);
        public static CommandApp CreateApp()
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.PropagateExceptions();
                config.AddBranch("test", branch =>
                {
                    branch.SetDescription("test cases defined in each day `Test` variable");
                    branch.AddDelegate<DayArgs>("one", TestOne).WithDescription("Run day <day>");
                    branch.AddDelegate<EmptyCommandSettings>("last", TestLast).WithDescription("Test only the last day");
                    branch.AddDelegate<EmptyCommandSettings>("all", TestAll).WithDescription("Test All");
                });

                config.AddBranch("run", branch =>
                {
                    branch.SetDescription("run on day inputs");
                    branch.AddDelegate<DayArgs>("one", RunOne);
                    branch.AddDelegate<EmptyCommandSettings>("last", RunLast).WithDescription("Run only the last day");
                    branch.AddDelegate<EmptyCommandSettings>("all", RunAll).WithDescription("Run All");
                });
            });

            return app;
        }

        public static int TestOne(CommandContext _context, DayArgs args)
        {
            try
            {
                AocRunner.TestOne(args.ProgramString());
            }
            catch (NoDayFound)
            {
                System.Console.WriteLine($@"Could not find day {args.ProgramString()}
                possible days: {string.Join(", ", AocRunner.Days().Select(day => day.GetType().Name))}
                ");
            }
            return 0;
        }

        public static int TestLast(CommandContext _context)
        {
            AocRunner.TestLast();
            return 0;
        }

        public static int RunOne(CommandContext _context, DayArgs args)
        {
            try
            {
                AocRunner.RunOne(args.ProgramString());
            }
            catch (NoDayFound)
            {
                System.Console.WriteLine($@"Could not find day {args.ProgramString()}
                possible days: {string.Join(", ", AocRunner.Days().Select(day => day.GetType().Name))}
                ");
            }
            return 0;
        }

        public static int RunLast(CommandContext _context)
        {
            AocRunner.RunLast();
            return 0;
        }

        public static int RunAll(CommandContext _context)
        {
            AocRunner.RunAll();
            return 0;
        }

        public static int TestAll(CommandContext _context)
        {
            AocRunner.TestAll();
            return 0;
        }

        public sealed class DayArgs : CommandSettings
        {
            [CommandArgument(0, "<Day>")]
            [Description("Day or as either the number or the full name")]
            public string Day { get; set; } = "unset";

            public string ProgramString()
            {
                try
                {
                    return "Day" + int.Parse(Day).ToString();
                }
                catch
                {
                    return Day;
                }
            }
        }
    }
}