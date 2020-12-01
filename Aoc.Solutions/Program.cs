using System;
using Aoc.Runner;
using System.Linq;

System.Console.WriteLine(string.Join(",", AocRunner.Days().Select(day => day.Number())));
AocRunner.Run(args);